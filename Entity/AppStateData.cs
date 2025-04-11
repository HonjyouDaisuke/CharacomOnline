namespace CharacomOnline.Entity;

public class AppStateData
{
  public bool IsAppLogin { get; set; } = false;
  public bool IsBoxLogin { get; set; } = false;
  public Guid? UserId { get; set; }
  public string? UserName { get; set; }
  public string? UserPictureUrl { get; set; }
  public string? UserRole { get; set; }
  public Guid? ProjectId { get; set; }
  public string? ProjectName { get; set; }

  public override string ToString()
  {
    return $"userId {UserId} userName {UserName} pict {UserPictureUrl} userRole {UserRole} projectId {ProjectId} projectName {ProjectName}";
  }
}
