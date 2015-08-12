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
using System.IO;

namespace plmOS.Model.Debug
{
    public class Session
    {

        private Model.Session Login()
        {
            Auth.Windows.Manager auth = new Auth.Windows.Manager();
            Database.SQLServer.Session database = new Database.SQLServer.Session(Properties.Settings.Default.ConnectionString);

            using (Model.Store store = new Model.Store(auth, database))
            {
                store.LoadAssembly(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\plmOS.Design.dll");



                Auth.Windows.Credentials credentials = new Auth.Windows.Credentials();

                return store.Login(credentials);
            }
        }

        private void CreateBOM()
        {
            using (Model.Session session = this.Login())
            {
                ItemType parttype = session.Store.ItemType("plmOS.Design.Part");
                RelationshipType bomlinetype = parttype.RelationshipType("plmOS.Design.BOMLine");

                Design.Part part1 = null;
                Design.Part part2 = null;
                Design.BOMLine bomline = null;

                using (Transaction transaction = session.BeginTransaction())
                {
                    part1 = (Design.Part)session.Create(parttype, transaction);
                    part1.Number.Value = "1234";
                    part1.Revision.Value = "01";
                    part1.Name.Value = "Test Assembly";

                    part2 = (Design.Part)session.Create(parttype, transaction);
                    part2.Number.Value = "5678";
                    part2.Revision.Value = "01";
                    part2.Name.Value = "Test Part";

                    bomline = (Design.BOMLine)session.Create(bomlinetype, part1, part2, transaction);
                    bomline.Quantity.Value = 3.0;

                    transaction.Commit();
                }
            }
        }

        private void GetBOM()
        {
            using (Model.Session session = this.Login())
            {
                ItemType parttype = session.Store.ItemType("plmOS.Design.Part");
                RelationshipType bomlinetype = parttype.RelationshipType("plmOS.Design.BOMLine");

                Queries.Item query = session.Create(parttype);
                query.Condition = new Conditions.Property(parttype.PropertyType("Number"), Conditions.Operators.eq, "1234");
                query.Execute();

                foreach(Item item in query.Items)
                {
                    String test = ((Design.Part)item).Number.Value;
                }

            }
        }

        public void Execute()
        {
            this.GetBOM();
        }

        public Session()
        {

        }
    }
}
