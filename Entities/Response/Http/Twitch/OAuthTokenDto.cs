namespace PeepoGuessrApi.Entities.Response.Http.Twitch;

public class OAuthTokenDto
{
    public required string Access_Token { get; set; }
    public int Expires_In { get; set; }
}