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

        private ItemType _baseItemType;
        public ItemType BaseItemType
        {
            get
            {
                return this._baseItemType;
            }
        }

        private Dictionary<String, RelationshipType> RelationshipTypeCache;

        internal void AddRelationshipType(RelationshipType RelationshipType)
        {
            this.RelationshipTypeCache[RelationshipType.Name] = RelationshipType;
        }

        public RelationshipType RelationshipType(String Name)
        {
            return this.RelationshipTypeCache[Name];
        }

        public IEnumerable<RelationshipType> RelationshipTypes
        {
            get
            {
                return this.RelationshipTypeCache.Values;
            }
        }

        private Dictionary<String, PropertyType> _propertyTypeCache;
        private Dictionary<String, PropertyType> PropertyTypeCache
        {
            get
            {
                if (this._propertyTypeCache == null)
                {
                    this._propertyTypeCache = new Dictionary<String, PropertyType>();

                    if (this.BaseItemType != null)
                    {
                        foreach(PropertyType proptype in this.BaseItemType.PropertyTypes)
                        {
                            this._propertyTypeCache[proptype.Name] = proptype;
                        }
                    }

                    foreach (System.Reflection.PropertyInfo propinfo in this.Type.GetProperties())
                    {
                        if (propinfo.DeclaringType.Equals(this.Type) && propinfo.PropertyType.BaseType != null && propinfo.PropertyType.BaseType.Equals(typeof(Model.Property)))
                        {
                            foreach(object custatt in propinfo.GetCustomAttributes(true))
                            {
                                if (custatt.GetType().BaseType.Equals(typeof(Model.PropertyAttribute)))
                                {
                                    switch (custatt.GetType().Name)
                                    {
               
                                        case "DoublePropertyAttribute":
                                            this._propertyTypeCache[propinfo.Name] = new PropertyTypes.Double(this, propinfo, (PropertyAttributes.DoublePropertyAttribute)custatt);
                                            break;

                                        case "ItemPropertyAttribute":
                                            this._propertyTypeCache[propinfo.Name] = new PropertyTypes.Item(this, propinfo, (PropertyAttributes.ItemPropertyAttribute)custatt);
                                            break;

                                        case "StringPropertyAttribute":
                                            this._propertyTypeCache[propinfo.Name] = new PropertyTypes.String(this, propinfo, (PropertyAttributes.StringPropertyAttribute)custatt);
                                            break;

                                        default:
                                            throw new NotImplementedException("Property Attribute Type not implemented: " + custatt.GetType().Name);
                                    }
                                }
                            }
                        }
                    }
                }

                return this._propertyTypeCache;
            }
        }

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
                }
                else
                {
                    this._baseItemType = this.Store.AllItemType(this.Type.BaseType.FullName);
                }

                // Enssure created in Database
                this.Create();

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
        }
    }
}
