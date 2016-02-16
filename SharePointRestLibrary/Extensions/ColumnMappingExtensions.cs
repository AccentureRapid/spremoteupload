using SharePointRestLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Extensions
{
    public static class ColumnMappingExtensions
    {
        public static IEnumerable<SPDataRecord> ToSPDataRecords(this DBRowCollection inboundDBData, SPColumnMappings mappings)
        {
            var listOut = new List<SPDataRecord>();
            foreach (string key in inboundDBData.Keys)
            {
                var fieldCollection = (DBFieldCollection)inboundDBData[key];
                var dataRecord = fieldCollection.ToSPDataRecord(mappings, key);
                listOut.Add(dataRecord);
            }

            return listOut;
        }

        public static SPDataRecord ToSPDataRecord(this DBFieldCollection fieldData, SPColumnMappings mapping, string keyValue)
        {
            var dataRecordOut = new SPDataRecord(keyValue);

            foreach (SPColumnMapping item in mapping)
            {
                var spfield = new SPDataField();
                spfield.SPColumnInternalName = item.SharePointDestinationField;
                spfield.SPDataType = item.SharePointDataType;

                try {
                    spfield.SPValue = fieldData[item.DBSourceField].ToString();
                } catch {
                    spfield.SPValue = string.Empty;
                }

                dataRecordOut.Add(spfield);
            }
            
            return dataRecordOut;
        }
    }
}
