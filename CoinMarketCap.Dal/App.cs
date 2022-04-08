using System.ComponentModel.DataAnnotations.Schema;

namespace CoinMarketCap.Dal
{
    public class App
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Login { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Key { get; set; }
    }
}