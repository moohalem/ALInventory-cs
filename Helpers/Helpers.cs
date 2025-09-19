using System.Text.Json;

namespace Helpers;

using Models;
public static class HelperFunctions
{
    private const string DbPath = "Database/Data.json";

    /// Checks if the database file and directory exist, creating them if they don't.
    public static void DbPathChecker()
    {
        var directory = Path.GetDirectoryName(DbPath);
        // Add null/empty check for directory path
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(DbPath))
        {
            File.WriteAllText(DbPath, "[]");
        }
    }

    public static List<Ingredients> LoadIngredients()
    {
        if (!File.Exists(DbPath))
        {
            return new List<Ingredients>();
        }

        var json = File.ReadAllText(DbPath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Ingredients>();
        }

        // Use null-coalescing to ensure a non-null List is always returned
        return JsonSerializer.Deserialize<List<Ingredients>>(json) ?? new List<Ingredients>();
    }
    
    public static void SaveChanges(List<Ingredients> ingredients)
    {
        // Configure serializer to write indented JSON for human readability.
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(ingredients, options);
        File.WriteAllText(DbPath, json);
    }

}