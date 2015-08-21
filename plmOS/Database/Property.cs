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
    internal class Property : IProperty
    {
        internal Model.Property ModelProperty { get; private set; }

        public Model.PropertyType PropertyType
        {
            get
            {
                return this.ModelProperty.PropertyType;
            }
        }

        public Object Object
        {
            get
            {
                switch(this.PropertyType.Type)
                {
                    case Model.PropertyTypeValues.Double:
                    case Model.PropertyTypeValues.String:
                    case Model.PropertyTypeValues.DateTime:
                        return this.ModelProperty.Object;
                    case Model.PropertyTypeValues.List:

                        if (((Model.Properties.List)this.ModelProperty).Value == null)
                        {
                            return null;
                        }
                        else
                        {
                            return ((Model.Properties.List)this.ModelProperty).Value.Index;
                        }

                    case Model.PropertyTypeValues.Item:

                        if (((Model.Properties.Item)this.ModelProperty).Value == null)
                        {
                            return null;
                        }
                        else
                        {
                            return ((Model.Properties.Item)this.ModelProperty).Value.BranchID;
                        }

                    default:
                        throw new NotImplementedException("PropertyType not implemented: " + this.PropertyType.Type);
                }
            }
        }

        internal Property (Model.Property ModelProperty)
        {
            this.ModelProperty = ModelProperty;
        }
    }
}
