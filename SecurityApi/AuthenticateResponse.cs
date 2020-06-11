namespace SecurityApi
{
    public class AuthenticateResponse
    {
        public int Id { get; }
        public string UserName { get; }
        public string DisplayName { get; }
        public string Token { get; }

        public AuthenticateResponse(User user, string token)
        {
            Id          = user.Id;
            UserName    = user.UserName;
            DisplayName = user.DisplayName;
            Token       = token;
        }
    }
}
