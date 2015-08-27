


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
    public abstract class List : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<Int32, ListValue> _values;
        public IEnumerable<ListValue> Values
        {
            get
            {
                return this._values.Values;
            }
        }

        public ListValue Default { get; private set; }

        private ListValue _selected;
        public ListValue Selected
        {
            get
            {
                return this._selected;
            }
            set
            {
                if (value == null)
                {
                    this._selected = null;
                }
                else
                {
                    if (value.List.Equals(this))
                    {
                        if(!value.Equals(this._selected))
                        {
                            this._selected = value;

                            if (this.PropertyChanged != null)
                            {
                                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Selected"));
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid ListValue");
                    }
                }
            }
        }

        public Int32 SelectedIndex
        {
            get
            {
                if (this.Selected != null)
                {
                    return this.Selected.Index;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (this._values.ContainsKey(value))
                {
                    this.Selected = this._values[value];
                }
                else
                {
                    this.Selected = null;
                }
            }
        }

        private Int32 Index;

        protected void AddValue(String Label, Boolean Default)
        {
            this._values[Index] = new ListValue(this, Index, Label);
  
            if (Default)
            {
                this.Default = this._values[Index];
                this.Selected = this.Default;
            }

            this.Index++;
        }

        public List()
        {
            this._values = new Dictionary<Int32, ListValue>();
            this.Index = 0;
        }
    }
}
