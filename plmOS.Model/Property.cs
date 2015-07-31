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
    public abstract class Property<T> : INotifyPropertyChanged
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

        private Boolean _readOnly;
        public Boolean ReadOnly 
        {
            get
            {
                return this._readOnly;
            }
            internal set
            {
                if (this._readOnly != value)
                {
                    this._readOnly = value;
                    this.OnPropertyChanged("ReadOnly");
                }
            }
        }

        private T _value;

        internal virtual void SetValue(T Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");
        }

        public T Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (!this.ReadOnly)
                {
                    this.SetValue(value);
                }
                else
                {
                    throw new Exceptions.ReadOnlyException();
                }
            }
        }

        public override string ToString()
        {
            if (this._value == null)
            {
                return "null";
            }
            else
            {
                return this._value.ToString();
            }
        }

        public Property (Item Item, Boolean ReadOnly)
        {
            this.Item = Item;
            this._readOnly = ReadOnly;
        }
    }
}
