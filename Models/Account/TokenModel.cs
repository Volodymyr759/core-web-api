using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Models.Account
{
    public class TokenModel
    {
        [Required]
        public string accessToken { get; set; }

        [Required]
        public string refreshToken { get; set; }
    }
}
