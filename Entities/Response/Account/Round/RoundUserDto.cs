namespace PeepoGuessrApi.Entities.Response.Account.Round;

public class RoundUserDto
{
    public int Id { get; set; }
    public string TwitchId { get; set; }
    public string TwitchName { get; set; }
    public string ImageUrl { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public double Distance { get; set; }
    public double? PosX { get; set; }
    public double? PosY { get; set; }

    public RoundUserDto(int id, string twitchId, string twitchName, string imageUrl, int health, int damage,
        double distance, double? posX, double? posY)
    {
        Id = id;
        TwitchId = twitchId;
        TwitchName = twitchName;
        ImageUrl = imageUrl;
        Health = health;
        Damage = damage;
        Distance = distance;
        PosX = posX;
        PosY = posY;
    }
}