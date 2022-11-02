namespace CoreWebApi.Models.Account
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}
