using System.IO;
using System.Text.Json;

namespace HaveItMain.Services;

public class SessionData
{
    public bool IsSignedIn { get; set; }
    public string LastUsername { get; set; } = "";
}

public class SessionService
{
    private const string FilePath = "session.json";

    public void SaveSession(bool isSignedIn, string username)
    {
        var data = new SessionData { IsSignedIn = isSignedIn, LastUsername = username };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(data));
    }

    public SessionData LoadSession()
    {
        if (!File.Exists(FilePath)) return new SessionData { IsSignedIn = false };
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<SessionData>(json) ?? new SessionData();
    }
    
    public void ClearSession()
    {
        if (File.Exists("session.json"))
        {
            // We just overwrite it with a 'not signed in' state
            SaveSession(false, "");
        }
    }
}