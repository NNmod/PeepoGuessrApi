namespace PeepoGuessrApi.Entities.Response.Hubs;

public class MaintenanceDto
{
    public DateTime WorksExpire { get; set; }

    public MaintenanceDto(DateTime worksExpire)
    {
        WorksExpire = worksExpire;
    }
}