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
        Dictionary<string, Dictionary<string, string>> GetData(string SelectStatement, string keyColumn);
    }
}
