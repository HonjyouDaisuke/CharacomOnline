using CharacomOnline.Entity;
using System.Collections.ObjectModel;

namespace CharacomOnline.Repositories;

public class CharaDataRepository
{
	private readonly List<CharaDataClass> targets = new();

	public ObservableCollection<CharaDataClass> Appraisals { get; set; } =
			new ObservableCollection<CharaDataClass>();

	public async Task AddAppraisalsAsync(List<CharaDataClass>? data)
	{
		if (data == null) return;

		await Task.Run(() =>
		{
			foreach (var item in data)
			{
				if (item == null) continue;
				lock (Appraisals)
				{
					Appraisals.Add(item); // スレッドセーフに操作
				}
			}
		});
	}

	public void ClearAppraisals()
	{
		Appraisals.Clear();
	}
}
