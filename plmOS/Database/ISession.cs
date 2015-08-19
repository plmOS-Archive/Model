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

namespace plmOS.Database
{
    public interface ISession: IDisposable
    {
        void Create(Model.ItemType ItemType);

        void Create(Model.RelationshipType RelationshipType);

        void Create(IItem Item, ITransaction Transaction);

        void Create(IRelationship Relationship, ITransaction Transaction);

        void Create(IFile File, ITransaction Transaction);

        void Supercede(IItem Item, ITransaction Transaction);

        IItem Get(Model.ItemType ItemType, Guid BranchID);

        IEnumerable<IItem> Get(Model.Queries.Item Query);

        IEnumerable<IRelationship> Get(Model.Queries.Relationship Query);

        FileStream ReadFromVault(IFile File);

        FileStream WriteToVault(IFile File);

        ITransaction BeginTransaction();
    }
}
