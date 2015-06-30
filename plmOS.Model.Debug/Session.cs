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

namespace plmOS.Model.Debug
{
    public class Session
    {

        public void Execute()
        {
            Database.Memory.Session database = new Database.Memory.Session();
            Model.Store store = new Model.Store(database);
            store.LoadAssembly("C:\\dev\\plmOS\\Model\\plmOS.Model.Debug\\bin\\Debug\\plmOS.Design.dll");

            Model.Session session = store.Login();

            ItemType parttype = store.ItemType("plmOS.Design.Part");

            using (Transaction transaction = session.BeginTransaction())
            {
                Item part = transaction.Create(parttype);
            }
        }

        public Session()
        {

        }
    }
}
