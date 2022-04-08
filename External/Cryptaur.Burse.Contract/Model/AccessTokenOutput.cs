using System;

namespace Cryptaur.Burse.Contract.Model
{
    public class AccessTokenOutput
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
    }
}