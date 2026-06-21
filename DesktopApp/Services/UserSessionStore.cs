using DesktopApp.Models;
using System.IO;
using System.Text.Json;

namespace DesktopApp.Services;

public static class UserSessionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private static readonly string SessionDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ReadCity");

    private static readonly string SessionFilePath = Path.Combine(SessionDirectory, "session.json");

    public static AuthenticatedUser? Load()
    {
        try
        {
            if (!File.Exists(SessionFilePath))
            {
                return null;
            }

            var json = File.ReadAllText(SessionFilePath);
            return JsonSerializer.Deserialize<AuthenticatedUser>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public static void Save(AuthenticatedUser user)
    {
        Directory.CreateDirectory(SessionDirectory);
        var json = JsonSerializer.Serialize(user, JsonOptions);
        File.WriteAllText(SessionFilePath, json);
    }

    public static void Clear()
    {
        if (File.Exists(SessionFilePath))
        {
            File.Delete(SessionFilePath);
        }
    }
}
