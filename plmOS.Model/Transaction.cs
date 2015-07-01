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

        private List<Item> ItemCache;

        internal void AddItem(Item Item)
        {
            if (Item.LockedBy != null && !Item.LockedBy.Equals(this))
            {
                throw new Exceptions.ItemLockedException(Item);
            }
            else
            {
                this.ItemCache.Add(Item);
                Item.LockedBy = this;
            }
        }

        internal Transaction(Session Session)
        {
            this.Session = Session;
            this.ItemCache = new List<Item>();
        }
    }
}
