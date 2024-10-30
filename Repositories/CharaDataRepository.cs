using System.Collections.ObjectModel;
using CharacomOnline;

public class CharaDataRepository
{
    public ObservableCollection<CharaDataClass> appraisals { get; set; } =
        new ObservableCollection<CharaDataClass>();
    List<CharaDataClass> targets = new List<CharaDataClass>();

    public void addApprisals(CharaDataClass data)
    {
        appraisals.Add(data);
    }
}
