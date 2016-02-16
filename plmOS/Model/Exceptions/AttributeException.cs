using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model.Exceptions
{
    public class AttributeException : Exception
    {
        internal AttributeException(String Message)
            :base(Message)
        {
        }
    }
}
