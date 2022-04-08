using System.ComponentModel.DataAnnotations;

namespace Cryptaur.Burse.Contract.Model
{
    public class AccessTokenInput
    {
        [Required]
        public string Key { get; set; }
    }
}