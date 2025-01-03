using Microsoft.AspNetCore.Components;

namespace CharacomOnline.Components;

public class ProgressDialogParameters
{
  public string Message { get; set; }
  public int MaxValue { get; set; }
  public int Value { get; set; }
  public string Unit { get; set; }
  // キャンセル通知用のデリゲート
  public EventCallback OnCancel { get; set; }

  // 処理のキャンセルを制御するトークン
  public CancellationToken Token { get; set; }
}
