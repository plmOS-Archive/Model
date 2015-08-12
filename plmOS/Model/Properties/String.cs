﻿/*  
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

namespace plmOS.Model.Properties
{
    public class String : Property, Database.Properties.IString
    {
        public System.Int32 Length
        {
            get
            {
                return ((PropertyTypes.String)this.PropertyType).Length;
            }
        }

        public System.String Value
        {
            get
            {
                if (this.Object == null)
                {
                    return null;
                }
                else
                {
                    return (System.String)this.Object;
                }
            }
            set
            {
                this.Object = value;
            }
        }

        internal override void SetObject(Object Object)
        {
            if (Object == null)
            {
                base.SetObject(Object);
            }
            else
            {
                if (Object is System.String)
                {
                    if (((System.String)Object).Length <= this.Length)
                    {
                        base.SetObject(Object);
                    }
                    else
                    {
                        throw new Exceptions.StringLengthException(this.Length);
                    }
                }
                else
                {
                    throw new ArgumentException("Value must be of type: System.String");
                }
            }
        }

        internal String (Model.Item Item, PropertyTypes.String PropertyType)
            : base(Item, PropertyType)
        {
        }
    }
}
