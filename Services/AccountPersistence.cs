using System;
using System.IO;
using System.Text.Json;

namespace HaveItMain.Services;

public class AccountPersistenceService
{
    private const string FileName = "account.json";
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public Account Load()
    {
        // 1. Check if the file exists
        if (!File.Exists(FileName))
        {
            // 2. Create a default "Starter" account
            var defaultAccount = new Account 
            { 
                Username = "NewUser", 
                FirstName = "Guest", 
                LastName = "User",
                IsSignedIn = false 
            };
            
            // 3. Save it immediately so the file exists for next time
            Save(defaultAccount);
            return defaultAccount;
        }

        try
        {
            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<Account>(json, Options) ?? new Account();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load failed: {ex.Message}");
            return new Account { Username = "Guest" };
        }
    }

    public void Save(Account account)
    {
        try
        {
            var json = JsonSerializer.Serialize(account, Options);
            File.WriteAllText(FileName, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save failed: {ex.Message}");
        }
    }
}