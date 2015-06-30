using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model
{
    public class Transaction : IDisposable
    {
        public Session Session { get; private set; }

        public void Commit()
        {

        }

        public void Rollback()
        {

        }

        public void Dispose()
        {

        }

        private Dictionary<Guid, Item> ItemCache;

        public Item Create(ItemType ItemType)
        {
            Item item = this.Session.Store.Create(ItemType);
            this.ItemCache[item.ID] = item;
            return item;
        }

        internal Transaction(Session Session)
        {
            this.Session = Session;
            this.ItemCache = new Dictionary<Guid, Item>();
        }
    }
}
