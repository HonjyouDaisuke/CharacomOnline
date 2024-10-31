using CharacomOnline.Repositories;
using CharacomOnline.Service;
using Microsoft.AspNetCore.Components.Forms;

namespace CharacomOnline.ViewModel;

public class CharaDataViewModel
{
	public CharaDataRepository CharaDataRepository { get; set; } = new CharaDataRepository();

	public async Task DataInput(FileUploadService fileService, IBrowserFile file)
	{
		var uploadData = await fileService.FileUploadAsync(file);
		if (uploadData == null)
		{
			return;
		}

		CharaDataRepository.AddAppraisals(uploadData);
	}
}
