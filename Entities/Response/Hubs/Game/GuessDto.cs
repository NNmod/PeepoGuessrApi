namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class GuessDto
{
    public int UserId { get; set; }
    public int GuessLeft { get; set; }
    public double Distance { get; set; }

    public GuessDto(int userId, int guessLeft, double distance)
    {
        UserId = userId;
        GuessLeft = guessLeft;
        Distance = distance;
    }
}