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

namespace plmOS.Database
{
    internal class Item : IItem
    {
        internal Model.Item ModelItem { get; private set; }

        public Model.ItemType ItemType 
        { 
            get
            {
                return this.ModelItem.ItemType;
            }
        }

        public Guid ItemID
        {
            get
            {
                return this.ModelItem.ItemID;
            }
        }

        public Guid BranchID
        {
            get
            {
                return this.ModelItem.BranchID;
            }
        }

        public Guid VersionID 
        { 
            get
            {
                return this.ModelItem.VersionID;
            }
        }

        public Int64 Branched 
        { 
            get
            {
                return this.ModelItem.Branched;
            }
        }

        public Int64 Versioned 
        { 
            get
            {
                return this.ModelItem.Versioned;
            }
        }

        public Int64 Superceded 
        { 
            get
            {
                return this.ModelItem.Superceded;
            }
        }

        private Dictionary<String, Property> _properties;
        public IEnumerable<IProperty> Properties 
        { 
            get
            {
                return this._properties.Values;
            }
        }

        public IProperty Property(Model.PropertyType PropertyType)
        {
            return this._properties[PropertyType.Name];
        }

        internal Item(Model.Item ModelItem)
        {
            this.ModelItem = ModelItem;
            this._properties = new Dictionary<String, Property>();

            foreach(Model.Property prop in this.ModelItem.Properties)
            {
                this._properties[prop.PropertyType.Name] = new Property(prop);
            }
        }
    }
}
