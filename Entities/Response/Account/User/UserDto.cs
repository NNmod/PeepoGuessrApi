namespace PeepoGuessrApi.Entities.Response.Account.User;

public class UserDto
{
    public int Id { get; set; }
    public string TwitchId { get; set; }
    public string TwitchName { get; set; }
    public string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Score { get; set; }
    public int Wins { get; set; }
    public string? GameCode { get; set; }
    public DateTime? GameExpire { get; set; }

    public UserDto(int id, string twitchId, string twitchName, string imageUrl, int divisionId, int score, int wins,
        string? gameCode, DateTime? gameExpire)
    {
        Id = id;
        TwitchId = twitchId;
        TwitchName = twitchName;
        ImageUrl = imageUrl;
        DivisionId = divisionId;
        Score = score;
        Wins = wins;
        GameCode = gameCode;
        GameExpire = gameExpire;
    }
}