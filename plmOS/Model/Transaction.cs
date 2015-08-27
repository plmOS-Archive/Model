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
            // Make Required Changes to Database
            using(Database.ITransaction databasetransaction = this.Session.Store.Database.BeginTransaction())
            {
                foreach(Lock thislock in this.LockCache)
                {
                    if (thislock.Item is Model.Relationship)
                    {
                        Database.Relationship databaserelationship = new Database.Relationship((Relationship)thislock.Item);

                        switch (thislock.Action)
                        {
                            case LockActions.Create:
                                this.Session.Store.Database.Create(databaserelationship, databasetransaction);
                                break;
                            case LockActions.Supercede:
                                this.Session.Store.Database.Supercede(databaserelationship, databasetransaction);
                                break;
                        }
                    }
                    else if (thislock.Item is Model.File)
                    {
                        Database.File databasefile = new Database.File((Model.File)thislock.Item);

                        switch (thislock.Action)
                        {
                            case LockActions.Create:
                                this.Session.Store.Database.Create(databasefile, databasetransaction);
                                break;
                            case LockActions.Supercede:
                                this.Session.Store.Database.Supercede(databasefile, databasetransaction);
                                break;
                        }
                    }
                    else
                    {
                        Database.Item databaseitem = new Database.Item(thislock.Item);

                        switch (thislock.Action)
                        {
                            case LockActions.Create:
                                this.Session.Store.Database.Create(databaseitem, databasetransaction);
                                break;
                            case LockActions.Supercede:
                                this.Session.Store.Database.Supercede(databaseitem, databasetransaction);
                                break;
                        }
                    }
                }

                databasetransaction.Commit();
            }

            // Remove Locks from Items
            foreach(Lock thislock in this.LockCache)
            {
                thislock.Item.Lock = null;
            }
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
