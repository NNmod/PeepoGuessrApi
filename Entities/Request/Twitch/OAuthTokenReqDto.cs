namespace PeepoGuessrApi.Entities.Request.Twitch;

public class OAuthTokenReqDto
{
    public string Client_Id { get; set; }
    public string Client_Secret { get; set; }
    public string Grant_Type { get; set; }

    public OAuthTokenReqDto(string clientId, string clientSecret, string grantType)
    {
        Client_Id = clientId;
        Client_Secret = clientSecret;
        Grant_Type = grantType;
    }
}