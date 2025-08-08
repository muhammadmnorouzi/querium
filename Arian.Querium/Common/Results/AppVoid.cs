namespace Arian.Querium.Common.Results;

public class AppVoid
{
    private static readonly Lazy<AppVoid> _instance = new(() => new());
    public static AppVoid Instance => _instance.Value;

    private AppVoid()
    {

    }
}
