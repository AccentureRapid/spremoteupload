using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Configuration
{
    public class SPColumnMapping
    {
        public string DBSourceField;
        public string SharePointDestinationField;
        public string SharePointDataType;
        public SPColumnMapping(){}
        public SPColumnMapping(string datasourceField, string spDestinationField, string spDataType)
        {
            DBSourceField = datasourceField;
            SharePointDestinationField = spDestinationField;
            SharePointDataType = spDataType;
        }
    }

    public class SPColumnMappings : List<SPColumnMapping>
    {
        public string DBKeyField { get; set; }

        public SPColumnMappings() {}
        public SPColumnMappings(string keyField){
            DBKeyField = keyField;
        }

        public void AddMapping(string dbField, string spField, string spDataType) {
            this.Add(new SPColumnMapping(dbField, spField, spDataType));
        }
    }
}
