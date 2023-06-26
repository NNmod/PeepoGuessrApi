namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class RoundPromotionDto
{
    public DateTime RoundExpire { get; set; }

    public RoundPromotionDto(DateTime roundExpire)
    {
        RoundExpire = roundExpire;
    }
}