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
    public class Session : IDisposable
    {
        public Store Store { get; private set; }

        public Guid ID { get; private set; }

        public Auth.IIdentity Identity { get; private set; }

        public Transaction BeginTransaction()
        {
            return new Transaction(this);
        }

        public Item Create(ItemType ItemType, Transaction Transaction)
        {
            // Create Item
            Item item = (Item)Activator.CreateInstance(ItemType.Type, new object[] { ItemType });

            // Add to Transaction
            Transaction.LockItem(item, LockActions.Create);

            // Add to Item Store Cache
            this.Store.AddItemToCache(item);

            return item;
        }

        public Relationship Create(RelationshipType RelationshipType, Item Parent, Item Child, Transaction Transaction)
        {
            // Create Relationship
            Relationship relationship = (Relationship)Activator.CreateInstance(RelationshipType.Type, new object[] { RelationshipType, Parent, Child });

            // Add to Transaction
            Transaction.LockItem(relationship, LockActions.Create);

            // Add to Item Store Cache
            this.Store.AddItemToCache(relationship);

            return relationship;
        }

        public Queries.Item Create(ItemType ItemType)
        {
            return new Queries.Item(this, ItemType);
        }

        public Queries.Relationship Create(Item Parent, RelationshipType RelationshipType)
        {
            if (Parent.ItemType.RelationshipTypes.Contains(RelationshipType))
            {
                return new Queries.Relationship(this, Parent, RelationshipType);
            }
            else
            {
                throw new ArgumentException("Invalid RelationshipType");
            }
        }

        public override string ToString()
        {
            return this.Identity.Name;
        }

        public void Dispose()
        {

        }

        internal Session(Store Store, Auth.IIdentity Identity)
        {
            this.ID = Guid.NewGuid();
            this.Store = Store;
            this.Identity = Identity;
        }
    }
}
