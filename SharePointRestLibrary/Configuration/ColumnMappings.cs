using SharePointRestLibrary.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Configuration
{
    public class ColumnMappings : Dictionary<string, SPColumnMapping>, IColumnMappings
    {
        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
