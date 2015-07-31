using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model.Exceptions
{
    public class ReadOnlyException : Exception
    {
        internal ReadOnlyException()
            :base("ReadOnly Property")
        {
        }
    }
}
