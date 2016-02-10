using SharePointRestLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Data
{
    public interface ISQLManager
    {
        List<string> GetColumnNames(string SelectStatement, string keyColumn);
        DBRowCollection GetData(string SelectStatement, string keyColumn);
    }
}
