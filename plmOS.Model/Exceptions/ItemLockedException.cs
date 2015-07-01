using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model.Exceptions
{
    public class ItemLockedException : Exception
    {
        public Item Item { get; private set; }

        internal ItemLockedException(Item Item)
            :base("Item Locked")
        {
            this.Item = Item;
        }
    }
}
