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
        public Session Session { get; private set; }

        public ItemType ItemType
        {
            get
            {
                return this.Session.Store.AllItemType(this.GetType().FullName);
            }
        }

        public Guid ItemID { get; private set; }

        public Guid BranchID { get; private set; }

        public Guid VersionID { get; private set; }

        public Int64 Branched { get; private set; }

        public Int64 Versioned { get; private set; }

        public Int64 Superceded { get; internal set; }

        public Lock Lock { get; internal set; }

        public Boolean LockedForCreate
        {
            get
            {
                if (this.Lock != null)
                {
                    if (this.Lock.Action == LockActions.Create)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean IsRelationship
        {
            get
            {
                return this.ItemType.IsRelationshipType;
            }
        }

        private Dictionary<PropertyType, Property> PropertiesCache;

        public IEnumerable<Property> Properties
        {
            get
            {
                return this.PropertiesCache.Values;
            }
        }

        public Property Property(PropertyType PropertyType)
        {
            foreach(PropertyType proptype in this.PropertiesCache.Keys)
            {
                if (proptype.Name.Equals(PropertyType.Name))
                {
                    return this.PropertiesCache[proptype];
                }
            }

            throw new ArgumentException("Invalid PropertyType");
        }

        private void CopyProperties(Item Item)
        {
            foreach(PropertyType proptype in this.ItemType.PropertyTypes)
            {
                switch(proptype.Type)
                {
                    case PropertyTypeValues.String:
                        ((Properties.String)proptype.PropertyInfo.GetValue(Item)).SetObject(((Properties.String)proptype.PropertyInfo.GetValue(this)).Object);
                        break;
                    case PropertyTypeValues.Item:
                        ((Properties.Item)proptype.PropertyInfo.GetValue(Item)).SetObject(((Properties.Item)proptype.PropertyInfo.GetValue(this)).Object);
                        break;
                    case PropertyTypeValues.Double:
                        ((Properties.Double)proptype.PropertyInfo.GetValue(Item)).SetObject(((Properties.Double)proptype.PropertyInfo.GetValue(this)).Object);
                        break;
                    case PropertyTypeValues.List:
                        ((Properties.List)proptype.PropertyInfo.GetValue(Item)).SetObject(((Properties.List)proptype.PropertyInfo.GetValue(this)).Object);
                        break;
                    default:
                        throw new NotImplementedException("PropertyType not implemented: " + proptype.Type);
                }
            }
        }

        public Item Branch(Transaction Transaction)
        {
            // Create new Branch
            Item newitem = (Item)Activator.CreateInstance(this.ItemType.Type, new object[] { this.Session });
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

            // Add to Item Cache
            this.Session.AddItemToCache(newitem);

            return newitem;
        }

        protected virtual Item Create()
        {
            return (Item)Activator.CreateInstance(this.ItemType.Type, new object[] { this.Session });
        }

        public Item Version(Transaction Transaction)
        {
            // Create new Version
            Item newitem = this.Create();
            newitem.ItemID = this.ItemID;
            newitem.BranchID = this.BranchID;
            newitem.VersionID = Guid.NewGuid();
            newitem.Versioned = DateTime.UtcNow.Ticks;
            newitem.Superceded = -1;
            this.CopyProperties(newitem);

            // Add new version to Transaction
            Transaction.LockItem(newitem, LockActions.Create);

            // Add to Item Cache
            this.Session.AddItemToCache(newitem);

            // Supercede this Item
            this.Superceded = newitem.Versioned - 1;
            Transaction.LockItem(this, LockActions.Supercede);

            return newitem;
        }

        public void Delete(Transaction Transaction)
        {
            // Add this Item to Transaction
            this.Superceded = DateTime.UtcNow.Ticks;
            Transaction.LockItem(this, LockActions.Supercede);
        }

        protected void Initialise()
        {
            foreach (PropertyType proptype in this.ItemType.PropertyTypes)
            {
                switch (proptype.Type)
                {
                    case PropertyTypeValues.Double:
                        this.PropertiesCache[proptype] = new Properties.Double(this, (PropertyTypes.Double)proptype);
                        break;
                    case PropertyTypeValues.Item:
                        this.PropertiesCache[proptype] = new Properties.Item(this, (PropertyTypes.Item)proptype);
                        break;
                    case PropertyTypeValues.String:
                        this.PropertiesCache[proptype] = new Properties.String(this, (PropertyTypes.String)proptype);
                        break;
                    case PropertyTypeValues.DateTime:
                        this.PropertiesCache[proptype] = new Properties.DateTime(this, (PropertyTypes.DateTime)proptype);
                        break;
                    case PropertyTypeValues.List:
                        this.PropertiesCache[proptype] = new Properties.List(this, (PropertyTypes.List)proptype);
                        break;
                    case PropertyTypeValues.Boolean:
                        this.PropertiesCache[proptype] = new Properties.Boolean(this, (PropertyTypes.Boolean)proptype);
                        break;
                    default:
                        throw new NotImplementedException("PropertyType not implemented: " + proptype.Type);
                }

                proptype.PropertyInfo.SetValue(this, this.PropertiesCache[proptype]);
            }
        }

        private static Database.IProperty DatabaseProperty(Database.IItem DatabaseItem, PropertyType PropertyType)
        {
            foreach(Database.IProperty prop in DatabaseItem.Properties)
            {
                if (prop.PropertyType.Name.Equals(PropertyType.Name))
                {
                    return prop;
                }
            }

            return null;
        }

        protected void Initialise(Database.IItem DatabaseItem)
        {
            foreach (PropertyType proptype in this.ItemType.PropertyTypes)
            {
                Database.IProperty databaseprop = DatabaseProperty(DatabaseItem, proptype);

                switch (proptype.Type)
                {
                    case PropertyTypeValues.Boolean:
                        this.PropertiesCache[proptype] = new Properties.Boolean(this, (PropertyTypes.Boolean)proptype);
                        this.PropertiesCache[proptype].SetObject(databaseprop.Object);
                        break;
                    case PropertyTypeValues.Double:
                        this.PropertiesCache[proptype] = new Properties.Double(this, (PropertyTypes.Double)proptype);
                        this.PropertiesCache[proptype].SetObject(databaseprop.Object);
                        break;
                    case PropertyTypeValues.Item:
                        this.PropertiesCache[proptype] = new Properties.Item(this, (PropertyTypes.Item)proptype);

                        if (databaseprop.Object != null)
                        {
                            Database.IItem databasepropitem = this.Session.Store.Database.Get(((PropertyTypes.Item)proptype).PropertyItemType, (Guid)databaseprop.Object);

                            this.PropertiesCache[proptype].SetObject(this.Session.Create(databasepropitem));
                        }
                        else
                        {
                            this.PropertiesCache[proptype].SetObject(null);
                        }

                        break;
                    case PropertyTypeValues.String:
                        this.PropertiesCache[proptype] = new Properties.String(this, (PropertyTypes.String)proptype);
                        this.PropertiesCache[proptype].SetObject(databaseprop.Object);
                        break;
                    case PropertyTypeValues.DateTime:
                        this.PropertiesCache[proptype] = new Properties.DateTime(this, (PropertyTypes.DateTime)proptype);
                        this.PropertiesCache[proptype].SetObject(databaseprop.Object);
                        break;
                    case PropertyTypeValues.List:
                        this.PropertiesCache[proptype] = new Properties.List(this, (PropertyTypes.List)proptype);

                        if (databaseprop.Object != null)
                        {
                            this.PropertiesCache[proptype].SetObject(((PropertyTypes.List)proptype).PropertyList.Value((Int32)databaseprop.Object));
                        }
                        else
                        {
                            this.PropertiesCache[proptype].SetObject(null);
                        }

                        break;
                    default:
                        throw new NotImplementedException("PropertyType not implemented: " + proptype.Type);
                }

                proptype.PropertyInfo.SetValue(this, this.PropertiesCache[proptype]);
            }
        }

        public Item(Session Session)
        {
            this.Session = Session;
            this.ItemID = Guid.NewGuid();
            this.BranchID = Guid.NewGuid();
            this.VersionID = Guid.NewGuid();
            this.Versioned = DateTime.UtcNow.Ticks;
            this.Branched = this.Versioned;
            this.Superceded = -1;
            this.PropertiesCache = new Dictionary<PropertyType, Property>();
        }

        public Item(Session Session, Database.IItem DatabaseItem)
        {
            this.Session = Session;
            this.VersionID = DatabaseItem.VersionID;
            this.BranchID = DatabaseItem.BranchID;
            this.ItemID = DatabaseItem.ItemID;
            this.Branched = DatabaseItem.Branched;
            this.Versioned = DatabaseItem.Versioned;
            this.Superceded = DatabaseItem.Superceded;
            this.PropertiesCache = new Dictionary<PropertyType, Property>();
        }
    }
}
