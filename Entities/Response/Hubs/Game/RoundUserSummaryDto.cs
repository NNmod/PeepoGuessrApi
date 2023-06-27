namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class RoundUserSummaryDto
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public double Distance { get; set; }
    public double? PosX { get; set; }
    public double? PosY { get; set; }

    public RoundUserSummaryDto(int userId, string name, string imageUrl, int divisionId, int health, int damage, 
        double distance, double? posX, double? posY)
    {
        UserId = userId;
        Name = name;
        ImageUrl = imageUrl;
        DivisionId = divisionId;
        Health = health;
        Damage = damage;
        Distance = distance;
        PosX = posX;
        PosY = posY;
    }
}