namespace PeepoGuessrApi.Entities.Response.Http.Cdn;

public class MapSettingsDto
{
    public List<int> MinPos { get; set; }
    public List<int> MaxPos { get; set; }

    public MapSettingsDto()
    {
        MinPos = new List<int>();
        MaxPos = new List<int>();
    }
}