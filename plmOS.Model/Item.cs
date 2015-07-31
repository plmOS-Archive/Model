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

        public Int64 Branched { get; internal set; }

        public Int64 Versioned { get; internal set; }

        public Int64 Superceded { get; internal set; }

        public Lock Lock { get; internal set; }

        public String PropertyStringValue(String Name)
        {
            return ((Properties.String)this.ItemType.PropertyInfo(Name).GetValue(this)).Value;
        }

        public Double? PropertyDoubleValue(String Name)
        {
            return ((Properties.Double)this.ItemType.PropertyInfo(Name).GetValue(this)).Value;
        }

        public Item PropertyItemValue(String Name)
        {
            return ((Properties.Item)this.ItemType.PropertyInfo(Name).GetValue(this)).Value;
        }

        internal void CopyProperties(Item Item)
        {

        }

        public Item Branch(Transaction Transaction)
        {
            // Create new Branch
            Item newitem = (Item)Activator.CreateInstance(this.ItemType.Type, new object[] { this.ItemType });
            newitem.ItemID = this.ItemID;
            newitem.BranchID = Guid.NewGuid();
            newitem.VersionID = Guid.NewGuid();
            newitem.Versioned = DateTime.UtcNow.Ticks;

            if (newitem.Versioned <= this.Versioned)
            {
                newitem.Versioned = this.Versioned + 1;
            }

            newitem.Branched = newitem.Versioned;
            newitem.Superceded = -1;
            this.CopyProperties(newitem);

            // Add new Branch to Transaction
            Transaction.LockItem(newitem, LockActions.Create);

            // Add to Item Store Cache
            this.ItemType.Store.AddItemToCache(newitem);

            return newitem;
        }

        public Item Version(Transaction Transaction)
        {
            // Add Item to Transaction
            Transaction.LockItem(this, LockActions.Supercede);

            // Create new Version
            Item newitem = (Item)Activator.CreateInstance(this.ItemType.Type, new object[] { this.ItemType });
            newitem.ItemID = this.ItemID;
            newitem.BranchID = this.BranchID;
            newitem.VersionID = Guid.NewGuid();
            newitem.Versioned = DateTime.UtcNow.Ticks;

            if (newitem.Versioned <= this.Versioned)
            {
                newitem.Versioned = this.Versioned + 1;
            }

            newitem.Superceded = -1;
            this.CopyProperties(newitem);

            // Add new version to Transaction
            Transaction.LockItem(newitem, LockActions.Create);

            // Add to Item Store Cache
            this.ItemType.Store.AddItemToCache(newitem);

            return newitem;
        }

        public void Delete(Transaction Transaction)
        {
            // Add this Item to Transaction
            Transaction.LockItem(this, LockActions.Supercede);
        }

        public Item(ItemType ItemType)
        {
            this.ItemType = ItemType;
        }
    }
}
