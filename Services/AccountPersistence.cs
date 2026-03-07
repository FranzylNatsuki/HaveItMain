using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HaveItMain.Services;

public class AccountPersistenceService
{
    private const string FileName = "accounts.json";
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

// This loads the WHOLE list, but returns the one that is currently "Signed In"
    public List<Account> Load() // Change return type to List<Account>
    {
        if (!File.Exists(FileName)) return new List<Account>();

        try
        {
            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<Account>>(json, Options) ?? new List<Account>();
        }
        catch 
        {
            return new List<Account>(); 
        }
    }
    public void Save(List<Account> accounts) // Changed from 'Account' to 'List<Account>'
    {
        try
        {
            var json = JsonSerializer.Serialize(accounts, Options);
            File.WriteAllText(FileName, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save failed: {ex.Message}");
        }
    }
}