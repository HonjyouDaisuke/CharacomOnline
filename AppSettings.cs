namespace CharacomOnline;

#pragma warning disable CS8618
public class AppSettings
{
  public string SUPABASE_URL { get; set; }
  public string ANON_KEY { get; set; }
  public string BOX_CLIENT_ID { get; set; }
  public string BOX_CLIENT_SECRET { get; set; }
}

public class Common
{
  public string BaseAppUrl { get; set; }
}
#pragma warning restore CS8618