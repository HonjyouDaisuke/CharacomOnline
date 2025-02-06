namespace CharacomOnline.Shared;

public class CreateProjectDialogResults
{
  public bool IsCanceled { get; set; }
  public string Title { get; set; } = "";
  public string Description { get; set; } = "";
  public string FolderId { get; set; } = "";
  public string CharaFolderId { get; set; } = "";
}
