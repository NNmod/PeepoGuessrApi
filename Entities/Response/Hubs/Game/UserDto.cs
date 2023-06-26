namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class UserDto
{
    public string ConnectionId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Health { get; set; }
    public int GuessLeft { get; set; }

    public UserDto(string connectionId, int userId, string name, string imageUrl, int divisionId, int health, int guessLeft)
    {
        ConnectionId = connectionId;
        UserId = userId;
        Name = name;
        ImageUrl = imageUrl;
        DivisionId = divisionId;
        Health = health;
        GuessLeft = guessLeft;
    }
}