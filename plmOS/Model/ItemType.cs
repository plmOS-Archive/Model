/*  
  plmOS Model provides a .NET client library for managing PLM (Product Lifecycle Management) data.

  Copyright (C) 2015 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model
{
    public class ItemType : IEquatable<ItemType>
    {
        public Store Store { get; private set; }

        internal Type Type { get; private set; }

        public String Name
        {
            get
            {
                return this.Type.FullName;
            }
        }

        public Boolean IsAbstract
        {
            get
            {
                return this.Type.IsAbstract;
            }
        }

        public virtual Boolean IsRelationshipType
        {
            get
            {
                return false;
            }
        }

        public bool Equals(ItemType other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.Type.Equals(other.Type);
            }
        }

        public Boolean IsSubclassOf(ItemType ItemType)
        {
            if (ItemType == null)
            {
                return false;
            }
            else
            {
                return this.Type.IsSubclassOf(ItemType.Type);
            }
        }

        private ItemType _rootItemType;
        public ItemType RootItemType
        {
            get
            {
                return this._rootItemType;
            }
        }

        private ItemType _baseItemType;
        public ItemType BaseItemType
        {
            get
            {
                return this._baseItemType;
            }
        }

        private Boolean BaseRelationshipsLoaded;

        private Dictionary<String, RelationshipType> RelationshipTypeCache;

        internal void AddRelationshipType(RelationshipType RelationshipType)
        {
            this.RelationshipTypeCache[RelationshipType.Name] = RelationshipType;
        }

        private void LoadBaseRelationshipTypes()
        {
            if (!this.BaseRelationshipsLoaded)
            {
                if (this.BaseItemType != null)
                {
                    foreach(RelationshipType reltype in this.BaseItemType.RelationshipTypes)
                    {
                        this.RelationshipTypeCache[reltype.Name] = reltype;
                    }
                }

                this.BaseRelationshipsLoaded = true;
            }
        }

        public RelationshipType RelationshipType(String Name)
        {
            this.LoadBaseRelationshipTypes();
            return this.RelationshipTypeCache[Name];
        }

        public IEnumerable<RelationshipType> RelationshipTypes
        {
            get
            {
                this.LoadBaseRelationshipTypes();
                return this.RelationshipTypeCache.Values;
            }
        }

        private Dictionary<String, PropertyType> PropertyTypeCache;

        public IEnumerable<PropertyType> PropertyTypes
        {
            get
            {
                return this.PropertyTypeCache.Values;
            }
        }

        public Boolean HasPropertyType(String Name)
        {
            return this.PropertyTypeCache.ContainsKey(Name);
        }

        public PropertyType PropertyType(String Name)
        {
            return this.PropertyTypeCache[Name];
        }

        protected Boolean Loaded { get; private set; }

        internal virtual void Load()
        {
            if (!this.Loaded)
            {
                if (this.Type.Equals(typeof(Item)))
                {
                    this._baseItemType = null;
                    this._rootItemType = this;
                }
                else
                {
                    this._baseItemType = this.Store.AllItemType(this.Type.BaseType.FullName);
                    this._rootItemType = this._baseItemType.RootItemType;
                }

                // Load PropertyTypes
                this.PropertyTypeCache = new Dictionary<String, PropertyType>();

                this.PropertyTypeCache = new Dictionary<String, PropertyType>();

                if (this.BaseItemType != null)
                {
                    foreach (PropertyType proptype in this.BaseItemType.PropertyTypes)
                    {
                        switch (proptype.Type)
                        {
                            case PropertyTypeValues.Double:
                                this.PropertyTypeCache[proptype.Name] = new PropertyTypes.Double(this, proptype.PropertyInfo, (PropertyAttributes.DoublePropertyAttribute)proptype.AttributeInfo, (PropertyTypes.Double)proptype);
                                break;
                            case PropertyTypeValues.Item:
                                this.PropertyTypeCache[proptype.Name] = new PropertyTypes.Item(this, proptype.PropertyInfo, (PropertyAttributes.ItemPropertyAttribute)proptype.AttributeInfo, (PropertyTypes.Item)proptype);
                                break;
                            case PropertyTypeValues.String:
                                this.PropertyTypeCache[proptype.Name] = new PropertyTypes.String(this, proptype.PropertyInfo, (PropertyAttributes.StringPropertyAttribute)proptype.AttributeInfo, (PropertyTypes.String)proptype);
                                break;
                            case PropertyTypeValues.DateTime:
                                this.PropertyTypeCache[proptype.Name] = new PropertyTypes.DateTime(this, proptype.PropertyInfo, (PropertyAttributes.DateTimePropertyAttribute)proptype.AttributeInfo, (PropertyTypes.DateTime)proptype);
                                break;
                            default:
                                throw new NotImplementedException("PropertyType not implemented: " + proptype.Type);
                        }
                    }
                }

                foreach (System.Reflection.PropertyInfo propinfo in this.Type.GetProperties())
                {
                    if (propinfo.DeclaringType.Equals(this.Type))
                    {
                        foreach (object custatt in propinfo.GetCustomAttributes(true))
                        {
                            if (custatt.GetType().BaseType.Equals(typeof(Model.PropertyAttribute)))
                            {
                                switch (custatt.GetType().Name)
                                {
                                    case "DoublePropertyAttribute":
                                        this.PropertyTypeCache[propinfo.Name] = new PropertyTypes.Double(this, propinfo, (PropertyAttributes.DoublePropertyAttribute)custatt, null);
                                        break;
                                    case "ItemPropertyAttribute":
                                        this.PropertyTypeCache[propinfo.Name] = new PropertyTypes.Item(this, propinfo, (PropertyAttributes.ItemPropertyAttribute)custatt, null);
                                        break;
                                    case "StringPropertyAttribute":
                                        this.PropertyTypeCache[propinfo.Name] = new PropertyTypes.String(this, propinfo, (PropertyAttributes.StringPropertyAttribute)custatt, null);
                                        break;
                                    case "DateTimePropertyAttribute":
                                        this.PropertyTypeCache[propinfo.Name] = new PropertyTypes.DateTime(this, propinfo, (PropertyAttributes.DateTimePropertyAttribute)custatt, null);
                                        break;
                                    default:
                                        throw new NotImplementedException("Property Attribute Type not implemented: " + custatt.GetType().Name);
                                }
                            }
                        }
                    }
                }

                this.Loaded = true;
            }
        }

        internal virtual void Create()
        {
            this.Store.Database.Create(this);
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal ItemType(Store Store, Type Type)
        {
            this.RelationshipTypeCache = new Dictionary<String, RelationshipType>();
            this.Store = Store;
            this.Type = Type;
            this.Loaded = false;
            this.BaseRelationshipsLoaded = false;
        }
    }
}
