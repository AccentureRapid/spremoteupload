using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Configuration
{
    public class DBFieldCollection : Dictionary<string, string>
    {
    }
    public class DBRowCollection : Dictionary<string, DBFieldCollection>
    {

    }
}
