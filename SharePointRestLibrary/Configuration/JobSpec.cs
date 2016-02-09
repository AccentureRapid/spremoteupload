using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net;
using System.IO;
using SharePointRestLibrary.Configuration;
using SharePointRestLibrary.Exception;

namespace SharePointRestLibrary
{
    [Serializable]
    public class JobSpec
    {
        public string SourceFilesPath { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string SQLSelectStatement { get; set; }
        public ColumnMappings ColumnMapping { get; set; }
        public string SharePointLibraryLocation { get; set; }
        public ICredentials SPCredentials { get; set; }
        public bool RaiseDebugMessages { get; set; }
        public string LogFilePath { get; set; }
        public string FailedFilesListPath { get; set; }
        public bool LoadFromFaildFilesListOnly { get; set; }

        public JobSpec(string sourceFilesPath, 
            string databaseConnectionString,
            string sqlSelectStatement, 
            ColumnMappings columnMapping,
            string sharePointLibraryLocation, 
            ICredentials spCredentials, 
            string failedFilesListPath = ".\\failedList.csv",
            bool raiseDebug = false, 
            bool loadFromFailedFilesListOnly = false, 
            string logFilePath = ".\\logFile.csv")
        {
            if (ColumnMapping.IsValid())
            {
                SourceFilesPath = sourceFilesPath;
                DatabaseConnectionString = databaseConnectionString;
                SQLSelectStatement = sqlSelectStatement;
                ColumnMapping = columnMapping;
                SharePointLibraryLocation = sharePointLibraryLocation;
                SPCredentials = spCredentials;
                RaiseDebugMessages = raiseDebug;
                FailedFilesListPath = failedFilesListPath;
                LoadFromFaildFilesListOnly = loadFromFailedFilesListOnly;
                LogFilePath = logFilePath;
            }
            else { throw new InvalidMappingException("SomeSource","SomeDestination"); }
        }

    }
}
