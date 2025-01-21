﻿namespace CharacomOnline.Repositories;

public class SelectingItemsRepository
{
  public readonly List<string> Characters = new();
  public readonly List<string> Materials = new();


  /// <summary>
  /// 個別文字群をクリア
  /// </summary>
  /// <returns></returns>
  public void ClearCharacters()
  {
    Characters.Clear();
  }

  /// <summary>
  /// 資料名をクリア
  /// </summary>
  /// <returns></returns>
  public void ClearMaterials()
  {
    Materials.Clear();
  }

  /// <summary>
  /// 個別文字を追加
  /// </summary>
  /// <param name="chara">個別文字</param>
  public void AddCharacters(string chara)
  {
    Characters.Add(chara);
  }

  /// <summary>
  /// 資料名を追加
  /// </summary>
  /// <param name="material">資料名(1～5文字ぐらい)</param>
  public void AddMaterials(string material)
  {
    Materials.Add(material);
  }


}
