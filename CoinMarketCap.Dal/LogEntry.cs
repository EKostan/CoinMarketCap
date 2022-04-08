using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoinMarketCap.Dal
{
    public class LogEntry
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Login { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string UserIp { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Type { get; set; }

        public string Value { get; set; }
    }
}