using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Zimmergren.AksFileShareDemo.Entities;

namespace Zimmergren.AksFileShareDemo
{
    public class SqliteHelper
    {
        private string _connectionString;
        public SqliteHelper(string connectionString)
        {
            _connectionString = $"Data Source={connectionString}";
        }

        // just return a new dumb object, to show the scalability and distributed workload in action.
        // this is not optimized, batched or suitable for any type of production scenarios. 
        public DummyQueryData QueryDatabaseAndReturnANewObject()
        {
            // Mode=ReadOnly to avoid concurrency issues with writing to the file share. We only need to process read scenarios.
            var connection = new SqliteConnection($"{_connectionString};Mode=ReadOnly");
            connection.Open();

            int indexKey = new Random().Next(1, 1000000);

            string sql = $"SELECT * FROM contact WHERE Id = {indexKey}";
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        watch.Stop();


                        return new DummyQueryData(watch.Elapsed.TotalSeconds, Environment.MachineName);
                    }
                }
            }

            connection.Close();
            return null;
        }

        public void CreateSampleCustomersTable()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText = $"CREATE TABLE contact(id INTEGER PRIMARY KEY, name varchar, email varchar);";
                command.ExecuteNonQuery();

                transaction.Commit();
            }


            connection.Close();
        }

        public void AddItems_Sqlite(int count)
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                var contacts = GetContacts(count);
                foreach (var contact in contacts)
                {
                    command.CommandText = $"INSERT INTO contact(name, email) VALUES('{contact.Name}', '{contact.Email}');";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }


            connection.Close();
        }
        private static List<Contact> GetContacts(int count)
        {
            List<Contact> contacts = new List<Contact>();
            for (int ii = 0; ii < count; ii++)
            {
                contacts.Add(new Contact($"Name {ii} {Guid.NewGuid()}", $"Email {ii} {Guid.NewGuid()}"));
            }

            return contacts;
        }
    }
}
