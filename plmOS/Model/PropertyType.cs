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
    public enum PropertyTypeValues { String, Double, Item, DateTime, List, Boolean };

    public abstract class PropertyType
    {
        public ItemType ItemType { get; private set; }

        public ItemType DefinitionItemType
        {
            get
            {
                if (this.BasePropertyType == null)
                {
                    return this.ItemType;
                }
                else
                {
                    return this.BasePropertyType.DefinitionItemType;
                }
            }
        }

        public String Name
        {
            get
            {
                return this.PropertyInfo.Name;
            }
        }

        public abstract PropertyTypeValues Type { get; }
 
        public Boolean Required
        {
            get
            {
                return this.AttributeInfo.Required;
            }
        }

        public Boolean ReadOnly
        {
            get
            {
                return this.AttributeInfo.ReadOnly;
            }
        }

        public Boolean IsInherited
        {
            get
            {
                if (this.BasePropertyType != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public PropertyType BasePropertyType { get; private set; }

        internal System.Reflection.PropertyInfo PropertyInfo { get; private set; }

        internal PropertyAttribute AttributeInfo { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }

        internal PropertyType(ItemType ItemType, System.Reflection.PropertyInfo PropertyInfo, PropertyAttribute AttributeInfo, PropertyType BasePropertyType)
        {
            this.ItemType = ItemType;
            this.PropertyInfo = PropertyInfo;
            this.AttributeInfo = AttributeInfo;
            this.BasePropertyType = BasePropertyType;
        }
    }
}
