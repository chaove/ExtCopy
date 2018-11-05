using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtCopy.common
{
    public class ApplicationException : Exception
    {
        public ApplicationException() : base() { }

        public ApplicationException(string message) : base(message) { }
    }
}
