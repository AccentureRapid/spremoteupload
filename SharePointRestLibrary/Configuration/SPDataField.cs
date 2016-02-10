using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Configuration
{
    public class SPDataField    
    {
        public string SPColumnInternalName { get; set; }
        public string SPDataType { get; set; }
        public string SPValue { get; set; }
    }

    public class SPDataRecord : List<SPDataField>
    {
        public string FileName { get; set; }

        public SPDataRecord(string fileName)
        {
            FileName = fileName;
        }
    }
}
