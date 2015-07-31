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
    public abstract class Item
    {
        public ItemType ItemType { get; private set; }

        public Guid ItemID { get; internal set; }

        public Guid BranchID { get; internal set; }

        public Guid VersionID { get; internal set; }

        public Int64 Created { get; internal set; }

        public Int64 Superceded { get; internal set; }

        public Transaction LockedBy { get; internal set; }

        public Properties.String String { get; private set; }

        private Dictionary<String, System.Reflection.PropertyInfo> _propertyInfoCache;
        private Dictionary<String, System.Reflection.PropertyInfo> PropertyInfoCache
        {
            get
            {
                if (this._propertyInfoCache == null)
                {
                    this._propertyInfoCache = new Dictionary<String, System.Reflection.PropertyInfo>();

                    foreach (System.Reflection.PropertyInfo propinfo in this.GetType().GetProperties())
                    {
                        if (propinfo.PropertyType.IsSubclassOf(typeof(Property<>)))
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

        private void CopyProperties(Item Item)
        {

        }

        public Item Version(Transaction Transaction)
        {
            // Add this Item to Transaction
            Transaction.AddItem(this);

            // Create new Version
            Item item = (Item)Activator.CreateInstance(this.ItemType.Type, new object[] { this.ItemType });
            item.ItemID = this.ItemID;
            item.BranchID = this.BranchID;
            item.VersionID = Guid.NewGuid();
            item.Created = DateTime.UtcNow.Ticks;

            if (item.Created <= this.Created)
            {
                item.Created = this.Created + 1;
            }

            item.Superceded = -1;
            this.CopyProperties(item);

            // Add new version to Transaction
            Transaction.AddItem(item);

            // Add to Cache
            this.ItemType.Store.AddItemToCache(item);

            // Supercede this Item
            this.Superceded = item.Created - 1;

            return item;
        }

        public Item Branch(Transaction Transaction)
        {
            // Create new Branch
            Item item = (Item)Activator.CreateInstance(this.ItemType.Type, new object[] { this.ItemType });
            item.ItemID = this.ItemID;
            item.BranchID = Guid.NewGuid();
            item.VersionID = Guid.NewGuid();
            item.Created = DateTime.UtcNow.Ticks;

            if (item.Created <= this.Created)
            {
                item.Created = this.Created + 1;
            }

            item.Superceded = -1;
            this.CopyProperties(item);

            // Add new Branch to Transaction
            Transaction.AddItem(item);

            // Add to Cache
            this.ItemType.Store.AddItemToCache(item);

            return item;
        }

        public void Delete(Transaction Transaction)
        {
            // Add this Item to Transaction
            Transaction.AddItem(this);

            // Set Superceded
            this.Superceded = DateTime.UtcNow.Ticks;
        }

        public Item(ItemType ItemType)
        {
            this.ItemType = ItemType;
            this.String = new Properties.String(this, true, 32);
 
        }
    }
}
