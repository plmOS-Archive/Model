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
using System.ComponentModel;

namespace plmOS.Model
{
    public enum PropertyTypeValues { String, Double, Item, DateTime, List, Boolean };

    public abstract class Property : Database.IProperty, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String Name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(Name);
                this.PropertyChanged(this, args);
            }
        }

        public Item Item { get; private set; }

        public PropertyType PropertyType { get; private set; }

        private Boolean _required;

        internal void SetRequired(Boolean Value)
        {
            if (this._required != Value)
            {
                this._required = Value;
                this.OnPropertyChanged("Required");
            }
        }

        public Boolean Required
        {
            get
            {
                return this._required;
            }
        }

        private Boolean _readOnly;

        internal void SetReadOnly(Boolean Value)
        {
            if (this._readOnly != Value)
            {
                this._readOnly = Value;
                this.OnPropertyChanged("ReadOnly");
            }
        }

        public Boolean ReadOnly 
        {
            get
            {
                return this._readOnly;
            }
        }

        private Object _object;

        internal virtual void SetObject(Object Object)
        {
            if (this._object != Object)
            {
                this._object = Object;
                this.OnPropertyChanged("Object");
            }
        }

        public Object Object
        {
            get
            {
                return this._object;
            }
            set
            {
                if (!this.ReadOnly)
                {
                    this.SetObject(value);
                }
                else
                {
                    throw new Exceptions.ReadOnlyException();
                }
            }
        }

        public override string ToString()
        {
            if (this._object == null)
            {
                return "null";
            }
            else
            {
                return this._object.ToString();
            }
        }

        internal Property (Item Item, PropertyType PropertyType)
        {
            this.Item = Item;
            this.PropertyType = PropertyType;

            // Set Default ReadOnly
            this.SetReadOnly(this.PropertyType.ReadOnly);

            // Set Default Required
            this.SetRequired(this.PropertyType.Required);
        }
    }
}
