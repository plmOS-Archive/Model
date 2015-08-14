using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model.Exceptions
{
    public class ItemNotLockedException : Exception
    {
        public Item Item { get; private set; }

        internal ItemNotLockedException(Item Item)
            :base("Item Not Locked")
        {
            this.Item = Item;
        }
    }
}
