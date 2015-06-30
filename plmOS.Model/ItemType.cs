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
        public Store Server { get; private set; }

        internal Type Type { get; private set; }

        public String Name
        {
            get
            {
                return this.Type.FullName;
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

        public override string ToString()
        {
            return this.Name;
        }

        internal ItemType(Store Server, Type Type)
        {
            this.RelationshipTypeCache = new Dictionary<String, RelationshipType>();
            this.Server = Server;
            this.Type = Type;
        }
    }
}
