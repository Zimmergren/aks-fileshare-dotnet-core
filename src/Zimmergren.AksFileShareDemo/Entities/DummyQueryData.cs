using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Zimmergren.AksFileShareDemo.Entities
{
    public class DummyQueryData : TableEntity
    {
        public DummyQueryData(double queryExecutionTime, string machineName)
        {
            this.RowKey = Guid.NewGuid().ToString("N");
            this.ETag = "*";
            this.PartitionKey = "dummy";

            this.ExecutionTime = queryExecutionTime;
            this.MachineName = machineName;
        }

        public string MachineName { get; set; }
        public double ExecutionTime { get; set; }
    }
}
