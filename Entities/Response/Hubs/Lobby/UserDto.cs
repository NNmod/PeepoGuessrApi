namespace PeepoGuessrApi.Entities.Response.Hubs.Lobby;

public class UserDto
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Score { get; set; }

    public UserDto(int userId, string name, string imageUrl, int divisionId, int score)
    {
        UserId = userId;
        Name = name;
        ImageUrl = imageUrl;
        DivisionId = divisionId;
        Score = score;
    }
}