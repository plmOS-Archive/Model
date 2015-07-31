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
using System.Reflection;

namespace plmOS.Model
{
    public class RelationshipType : ItemType
    {
        private ItemType _parentItemType;
        public ItemType ParentItemType
        {
            get
            {
                return this._parentItemType;
            }
        }

        private ItemType _childItemType;
        public ItemType ChildItemType
        {
            get
            {
                return this._childItemType;
            }
        }

        internal override void Load()
        {
            if (!this.Loaded)
            {

                foreach (ConstructorInfo constructorinfo in Type.GetConstructors())
                {
                    ParameterInfo[] parameters = constructorinfo.GetParameters();

                    if ((parameters.Length == 2) && (parameters[0].ParameterType.Equals(typeof(RelationshipType))) && ((parameters[1].ParameterType.IsSubclassOf(typeof(Item))) || (parameters[1].ParameterType.Equals(typeof(Item)))))
                    {
                        this._parentItemType = this.Store.AllItemType(parameters[1].ParameterType.FullName);
                        this._parentItemType.AddRelationshipType(this);
                        this._childItemType = null;
                    }
                    else if ((parameters.Length == 3) && (parameters[0].ParameterType.Equals(typeof(RelationshipType))) && (parameters[1].ParameterType.IsSubclassOf(typeof(Item))) && (parameters[2].ParameterType.IsSubclassOf(typeof(Item))))
                    {
                        this._parentItemType = this.Store.AllItemType(parameters[1].ParameterType.FullName);
                        this._parentItemType.AddRelationshipType(this);
                        this._childItemType = this.Store.AllItemType(parameters[2].ParameterType.FullName);
                    }
                }

                base.Load();
            }
        }

        internal Database.IRelationshipType DatabaseRelationshipType
        {
            get
            {
                return (Database.IRelationshipType)this.DatabaseItemType;
            }
        }

        internal override void Create()
        {
            if (this.DatabaseItemType == null)
            {
                // Create Parent ItemType
                this.ParentItemType.Create();

                // Create Database ItemType
                if (this.BaseItemType.Name != "plmOS.Model.Item")
                {
                    this.BaseItemType.Create();

                    if (this.ChildItemType != null)
                    {
                        this.ChildItemType.Create();
                        this.DatabaseItemType = this.Store.Database.CreateRelationshipType((Database.IRelationshipType)this.BaseItemType.DatabaseItemType, this.Name, this.ParentItemType.DatabaseItemType, this.ChildItemType.DatabaseItemType);
                    }
                    else
                    {
                        this.DatabaseItemType = this.Store.Database.CreateRelationshipType((Database.IRelationshipType)this.BaseItemType.DatabaseItemType, this.Name, this.ParentItemType.DatabaseItemType, null);
                    }
                }
                else
                {
                    if (this.ChildItemType != null)
                    {
                        this.ChildItemType.Create();
                        this.DatabaseItemType = this.Store.Database.CreateRelationshipType(this.BaseItemType.DatabaseItemType, this.Name, this.ParentItemType.DatabaseItemType, this.ChildItemType.DatabaseItemType);
                    }
                    else
                    {
                        this.DatabaseItemType = this.Store.Database.CreateRelationshipType(this.BaseItemType.DatabaseItemType, this.Name, this.ParentItemType.DatabaseItemType, null);
                    }
                }

                // Add Database PropertyTypes
                this.CreatePropertyTypes();
            }
        }

        internal RelationshipType(Store Server, Type Type)
            :base(Server, Type)
        {

        }
    }
}
