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

        private Boolean Loaded;

        internal void Load()
        {
            if (!this.Loaded)
            {
                foreach (ConstructorInfo constructorinfo in Type.GetConstructors())
                {
                    ParameterInfo[] parameters = constructorinfo.GetParameters();

                    if ((parameters.Length == 2) && (parameters[0].ParameterType.Equals(typeof(Session))) && (parameters[1].ParameterType.IsSubclassOf(typeof(Item))))
                    {
                        this._parentItemType = this.Server.AllItemType(parameters[1].ParameterType.FullName);
                        this._parentItemType.AddRelationshipType(this);
                        this._childItemType = null;
                    }
                    else if ((parameters.Length == 3) && (parameters[0].ParameterType.Equals(typeof(Session))) && (parameters[1].ParameterType.IsSubclassOf(typeof(Item))) && (parameters[2].ParameterType.IsSubclassOf(typeof(Item))))
                    {
                        this._parentItemType = this.Server.AllItemType(parameters[1].ParameterType.FullName);
                        this._parentItemType.AddRelationshipType(this);
                        this._childItemType = this.Server.AllItemType(parameters[2].ParameterType.FullName);
                    }
                }

                this.Loaded = true;
            }
        }

        internal RelationshipType(Server Server, Type Type)
            :base(Server, Type)
        {
            this.Loaded = false;
        }
    }
}
