using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plmOS.Model.Exceptions
{
    public class StringLengthException : Exception
    {
        internal StringLengthException(System.Int32 Length)
            :base("String Length Exceeded: " + Length)
        {
        }
    }
}
