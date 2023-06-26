namespace PeepoGuessrApi.Entities.Response;

public class ListDto<T> where T : class
{
    public int Pages { get; set; }
    public List<T> Items { get; set; }

    public ListDto()
    {
        Items = new List<T>();
    }
}