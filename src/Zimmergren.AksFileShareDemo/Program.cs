using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zimmergren.AksFileShareDemo
{
    public class Program
    {
        //private static string sqlitefile = "/myawesomefileshare/huge-sqlite-database.sqlite";
        private static string sqlitefile = @"C:\Code\sqlitefiles\huge-sqlite-database.sqlite";

        // For demo. Don't put your connection details here in any type of real project.
        private const string AzureStorageConnectionString = "your test/dev, non-production connection string...";
        private static CloudTableClient _tableClient;
        private static CloudTable _table;

        private static SqliteHelper _sqliteHelper;

        static void Main(string[] args)
        {
            // if you need to create a table first in the sqlite database.
            //CreateTable();

            #region Just for demo insertions to Azure Storage Table

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AzureStorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference("demotable");
            _table.CreateIfNotExistsAsync().Wait();

             #endregion

            _sqliteHelper = new SqliteHelper(sqlitefile);
            while (true)
            {
                // Write: When you want to pre-populate the sqlite database with the dummy data.
                //ProcessWriteMany();

                // Read: When we start reading. This is what we do in the distributed processing setup from multiple containers.
                ProcessReadOnce();
            }
        }
        
        private static void ProcessReadOnce()
        {
            var newDummyObject = _sqliteHelper.QueryDatabaseAndReturnANewObject();

            // Insert dummy object into Azure Storage Table in order to transition from the sqlite database into the table storage.
            // Dummy data to just prove that the distributed processing of reading the sqlite database works.
            TableOperation insertOperation = TableOperation.Insert(newDummyObject);
            _table.ExecuteAsync(insertOperation).Wait();

            // Simple logging to console, if you want to view this running distributed in the cloud.
            // Aadded machine name to see when it's ran by different containers, which will display the container that executed the command.
            string output = $"{DateTime.UtcNow:u}. QUERY: {newDummyObject.ExecutionTime} sec. MACHINE: {newDummyObject.MachineName}";
            Console.WriteLine(output);
        }

        private static void ProcessWriteMany()
        {
            _sqliteHelper.AddItems_Sqlite(300000);

        }
    }

}
