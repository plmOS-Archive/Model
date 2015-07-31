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
    public class ItemType
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

        private Dictionary<String, System.Reflection.PropertyInfo> _propertyInfoCache;
        private Dictionary<String, System.Reflection.PropertyInfo> PropertyInfoCache
        {
            get
            {
                if (this._propertyInfoCache == null)
                {
                    this._propertyInfoCache = new Dictionary<String, System.Reflection.PropertyInfo>();

                    foreach (System.Reflection.PropertyInfo propinfo in this.Type.GetProperties())
                    {
                        if (propinfo.PropertyType.BaseType != null && propinfo.PropertyType.BaseType.IsGenericType && propinfo.PropertyType.BaseType.GetGenericTypeDefinition().Equals(typeof(Property<>)))
                        {
                            this._propertyInfoCache[propinfo.Name] = propinfo;
                        }
                    }
                }

                return this._propertyInfoCache;
            }
        }

        public IEnumerable<String> Properties
        {
            get
            {
                return this.PropertyInfoCache.Keys;
            }
        }

        public Boolean HasProperty(String Name)
        {
            return this.PropertyInfoCache.ContainsKey(Name);
        }

        internal System.Reflection.PropertyInfo PropertyInfo(String Name)
        {
            return this.PropertyInfoCache[Name];
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

                this.Loaded = true;
            }
        }

        internal Database.IItemType DatabaseItemType { get; set; }

        internal virtual void Create()
        {
            if (this.DatabaseItemType == null)
            {
                // Create Database ItemType
                if (this.BaseItemType != null)
                {
                    this.BaseItemType.Create();
                    this.DatabaseItemType = this.Store.Database.CreateItemType(this.BaseItemType.DatabaseItemType, this.Name);
                }
                else
                {
                    this.DatabaseItemType = this.Store.Database.CreateItemType(this.Name);
                }

                // Add Database PropertyTypes
                this.CreatePropertyTypes();
            }
        }

        protected void CreatePropertyTypes()
        {
            foreach (String name in this.Properties)
            {
                switch (this.PropertyInfo(name).PropertyType.Name)
                {
                    case "Item":

                        if (name != "Parent" && name != "Child")
                        {
                            this.DatabaseItemType.AddPropertyType(name, Database.PropertyValueTypes.Item);
                        }

                        break;
                    case "String":
                        this.DatabaseItemType.AddPropertyType(name, Database.PropertyValueTypes.String);
                        break;
                    case "Double":
                        this.DatabaseItemType.AddPropertyType(name, Database.PropertyValueTypes.Double);
                        break;
                    default:
                        throw new NotImplementedException("PropertyType not implemented: " + this.PropertyInfo(name).PropertyType.Name);
                }
            }
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
