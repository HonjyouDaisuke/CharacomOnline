using CharacomOnline.Entity;
using System.Collections.ObjectModel;

namespace CharacomOnline.Repositories;

public class CharaDataRepository
{
	private readonly List<CharaDataClass> targets = new();

	public ObservableCollection<CharaDataClass> Appraisals { get; set; } =
			new ObservableCollection<CharaDataClass>();

	public void AddAppraisals(CharaDataClass data)
	{
		Appraisals.Add(data);
	}
}
