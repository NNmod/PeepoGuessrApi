namespace PeepoGuessrApi.Entities.Request.Twitch;

public class UserAuthTokenDto
{
    public string Client_Id { get; set; }
    public string Client_Secret { get; set; }
    public string Code { get; set; }
    public string Grant_Type { get; set; }
    public string Redirect_Uri { get; set; }

    public UserAuthTokenDto(string clientId, string clientSecret, string code, string grantType, string redirectUri)
    {
        Client_Id = clientId;
        Client_Secret = clientSecret;
        Code = code;
        Grant_Type = grantType;
        Redirect_Uri = redirectUri;
    }
}