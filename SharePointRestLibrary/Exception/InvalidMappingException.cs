using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Exception
{
    public class InvalidMappingException : ApplicationException
    {
        public InvalidMappingException(string sourceField, string destinationField, string message = "")
            : base(string.Format("{0} Source Field: {1} - Destination Field: {2}"))
        {
        }
    }
}
