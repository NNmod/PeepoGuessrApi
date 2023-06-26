using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Game;

public class User
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? ConnectionId { get; set; }
    [ForeignKey("Game")]
    public int GameId { get; set; }
    public virtual Game? Game { get; set; }
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Health { get; set; }
    public int GuessAvailable { get; set; }
    public double? PosX { get; set; }
    public double? PosY { get; set; }
    public double Distance { get; set; }
}