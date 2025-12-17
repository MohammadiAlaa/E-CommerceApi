namespace E_CommerceApi.DTOs
{
    public class AuthModel
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }


        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Massege { get; set; }

    }
}
