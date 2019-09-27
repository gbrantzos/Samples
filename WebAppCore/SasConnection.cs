using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore
{
    public class SasConnection
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DbAlias { get; set; }
        public string CompanyCode { get; set; }
        public string WarehouseCode { get; set; }
        public string WorkingDate { get; set; }
        public string FiyDate { get; set; }

        public SasConnection()
        {
            Host = "localhost";
            Port = 400;
            User = "sng";
            Password = "sng";
            DbAlias = "SENDATA_SEN_DB";
            CompanyCode = "001";
            WarehouseCode = "001";
            WorkingDate = "-1";
            FiyDate = "-1";
        }
    }
}
