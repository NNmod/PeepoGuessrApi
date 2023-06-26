using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Maintenance;

public class Access
{
    [Key]
    public int Id { get; set; }
    public string? Token { get; set; }
    public DateTime Expire { get; set; }

    public Access()
    {
        Expire = DateTime.UtcNow;
    }
}