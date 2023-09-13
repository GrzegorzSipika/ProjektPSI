namespace LoginProject.Common;

public class EntityBase
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime Created { get; set; }
    public int StatusId { get; set; }
}