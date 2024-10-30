using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;

namespace CharacomOnline;

public class CharaDataViewModel
{
    public CharaDataRepository charaDataRepository { get; set; } = new CharaDataRepository();

    public async Task DataInput(FileUploadService fileService, IBrowserFile file)
    {
        var uploadData = await fileService.FileUploadAsync(file);
        if (uploadData == null)
            return;

        charaDataRepository.addApprisals(uploadData);
    }
}
