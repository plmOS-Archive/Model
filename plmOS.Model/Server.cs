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
using System.IO;
using System.Reflection;

namespace plmOS.Model
{
    public class Server
    {
        private Dictionary<String, ItemType> AllItemTypeCache;

        private Dictionary<String, ItemType> ItemTypeCache;

        private Dictionary<String, RelationshipType> RelationshipTypeCache;

        internal ItemType AllItemType(String Name)
        {
            return this.AllItemTypeCache[Name];
        }

        public ItemType ItemType(String Name)
        {
            return this.ItemTypeCache[Name];
        }

        public IEnumerable<ItemType> ItemTypes
        {
            get
            {
                return this.ItemTypeCache.Values;
            }
        }

        public void LoadAssembly(String AssemblyFilename)
        {
            this.LoadAssembly(new FileInfo(AssemblyFilename));
        }

        public void LoadAssembly(FileInfo AssemblyFile)
        {
            Assembly assembly = Assembly.LoadFrom(AssemblyFile.FullName);

            // Find all ItemTypes and Relationships

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Relationship)))
                {
                    RelationshipType reltype = new RelationshipType(this, type);
                    this.AllItemTypeCache[reltype.Name] = reltype;
                    this.RelationshipTypeCache[reltype.Name] = reltype;
                }
                else if (type.IsSubclassOf(typeof(Item)))
                {
                    ItemType itemtype = new ItemType(this, type);
                    this.AllItemTypeCache[itemtype.Name] = itemtype;
                    this.ItemTypeCache[itemtype.Name] = itemtype;
                }
            }

            // Load RelationshipTypes

            foreach(RelationshipType reltype in this.RelationshipTypeCache.Values)
            {
                reltype.Load();
            }
        }

        public Session Login()
        {
            return new Session(this);
        }

        public Server()
        {
            this.AllItemTypeCache = new Dictionary<String, ItemType>();
            this.ItemTypeCache = new Dictionary<String, ItemType>();
            this.RelationshipTypeCache = new Dictionary<String, RelationshipType>();
        }
    }
}
