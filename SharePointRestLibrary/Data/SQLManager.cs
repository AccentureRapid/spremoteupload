using SharePointRestLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestLibrary.Data
{
    public class SQLManager : ISQLManager, IDisposable
    {
        private SqlConnection _connection;
        private bool _isDisposing = false;
        
        public SQLManager(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            
        }

        public List<string> GetColumnNames(string selectStatement, string keyColumn)
        {
            var listOut = new List<string>();
            var dataTable = GetDataTable(selectStatement);
            
            foreach (DataColumn column in dataTable.Columns)
            {
                listOut.Add(column.ColumnName);
            }
            if (listOut.Contains(keyColumn, StringComparer.InvariantCultureIgnoreCase))
                return listOut;
            else
                throw new ApplicationException("Key column was not found in the select statement.  Did you forget to alias a column?");
        }

        public DBRowCollection GetData(string selectStatement, string keyColumn)
        {
            var dataOut = new DBRowCollection();
            var dataTable = GetDataTable(selectStatement);
            
            foreach (DataRow row in dataTable.Rows)
	        {
                try
                {
                    var newDataRow = new DBFieldCollection();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (!column.ColumnName.Equals(keyColumn, StringComparison.InvariantCultureIgnoreCase))
                            newDataRow.Add(column.ColumnName, row[column.ColumnName].ToString());
                    }
                    dataOut.Add(row[keyColumn].ToString(), newDataRow);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("The file {0} was already found in the dataset.  Please ensure the database is returning distinct filenames.", row[keyColumn].ToString()));
                }
            }
            return dataOut;
        }

        private DataTable GetDataTable(string selectStatement)
        {
   
            var dataTableOut = new DataTable();
            _connection.Open();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectStatement, _connection))
            {
                sqlDataAdapter.Fill(dataTableOut);
            }
            _connection.Close();
            return dataTableOut;
        }

        public void Dispose()
        {
            try
            {
                _isDisposing = true;

                if (!_isDisposing)
                {
                    _connection.Close();
                    _connection = null;
                }
            }
            catch { }
        }
    }
}
