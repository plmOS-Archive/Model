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
    public class Transaction : IDisposable
    {
        public Session Session { get; private set; }

        public Guid ID { get; private set; }

        public void Commit()
        {
            // Get Transaction Time
            Int64 transactiontime = DateTime.UtcNow.Ticks;

            /*
            // Make Required Changes to Database
            using(Database.ITransaction databasetransaction = this.Session.Store.Database.BeginTransaction())
            {
                foreach(Lock thislock in this.LockCache)
                {
                    switch(thislock.Action)
                    {
                        case LockActions.Create:
                            Database.IItem databaseitem = this.Session.Store.Database.Create(thislock.Item.ItemType.DatabaseItemType, thislock.Item.ItemID, thislock.Item.BranchID, thislock.Item.VersionID, thislock.Item.Branched, thislock.Item.Versioned, databasetransaction);

                            foreach(PropertyType proptype in thislock.Item.ItemType.PropertyTypes)
                            {
                                Database.IPropertyType databaseproptype = databaseitem.Type.PropertyType(proptype.Name);

                                switch (databaseproptype.ValueType)
                                {
                                    case Database.PropertyValueTypes.Double:
                                        databaseitem.AddProperty(databaseproptype, thislock.Item.PropertyDoubleValue(proptype.Name));
                                        break;
                                    case Database.PropertyValueTypes.Item:
                                        Item propitem = thislock.Item.PropertyItemValue(proptype.Name);

                                        if (propitem != null)
                                        {
                                            databaseitem.AddProperty(databaseproptype, propitem.VersionID);
                                        }
                                        else
                                        {
                                            databaseitem.AddProperty(databaseproptype, null);
                                        }

                                        break;
                                    case Database.PropertyValueTypes.String:
                                        databaseitem.AddProperty(databaseproptype, thislock.Item.PropertyStringValue(proptype.Name));
                                        break;
                                }
                            }

                            break;
                        case LockActions.Supercede:
                            this.Session.Store.Database.Supercede(thislock.Item.VersionID, transactiontime, databasetransaction);
                            break;
                    }
                }

                databasetransaction.Commit();
            }

            // Update Items
            foreach (Lock thislock in this.LockCache)
            {
                switch (thislock.Action)
                {
                    case LockActions.Create:
                        break;
                    case LockActions.Supercede:
                        thislock.Item.Superceded = transactiontime;
                        break;
                }

                thislock.Item.Lock = null;
            }*/
        }

        public void Rollback()
        {

        }

        public void Dispose()
        {

        }

        private List<Lock> LockCache;

        internal Lock LockItem(Item Item, LockActions Action)
        {
            if (Item.Lock == null)
            {
                Lock newlock = new Lock(this, Action, Item);
                Item.Lock = newlock;
                this.LockCache.Add(newlock);
                return newlock;
            }
            else
            {
                if (Item.Lock.Transaction.Equals(this) && Item.Lock.Action.Equals(Action))
                {
                    return Item.Lock;
                }
                else
                {
                    throw new Exceptions.ItemLockedException(Item);
                }
            }
        }

        internal Transaction(Session Session)
        {
            this.Session = Session;
            this.ID = Guid.NewGuid();
            this.LockCache = new List<Lock>();
        }
    }
}
