using CharacomOnline.Entity;

namespace CharacomOnline.Repositories;

public class GlobalSettingRepository
{
  public List<StandardMaster> StandardMaster = new();
  public List<StrokeMaster> StrokeMaster = new();

  public void AddStandard(StandardMaster item)
  {
    StandardMaster.Add(item);
  }

  public void ClearStandard()
  {
    StandardMaster.Clear();
  }

  public void AddStroke(StrokeMaster item)
  {
    StrokeMaster.Add(item);
  }

  public void ClearStroke()
  {
    StrokeMaster.Clear();
  }
}
