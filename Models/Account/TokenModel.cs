using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Models.Account
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
