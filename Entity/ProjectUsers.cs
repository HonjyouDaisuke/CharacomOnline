namespace CharacomOnline.Entity;

public class ProjectUsers
{
  public Guid Id { get; set; }
  public string Name { get; set; } = "";
  public string Email { get; set; } = "";
  public string PictureUrl { get; set; } = "";
  public string Role { get; set; } = "";
}
