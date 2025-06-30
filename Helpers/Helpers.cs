using System.Text.Json;

namespace Helpers;


public static class HelperFunctions
{
    public static string DbPath = "Database/Data.json";

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

    public static List<Models.Ingredients> LoadIngredients()
    {
        if (!File.Exists(DbPath))
        {
            return new List<Models.Ingredients>();
        }

        var json = File.ReadAllText(DbPath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Models.Ingredients>();
        }

        // Use null-coalescing to ensure a non-null List is always returned
        return JsonSerializer.Deserialize<List<Models.Ingredients>>(json) ?? new List<Models.Ingredients>();
    }
    
    public static void SaveChanges(List<Models.Ingredients> ingredients)
    {
        // Configure serializer to write indented JSON for human readability.
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(ingredients, options);
        File.WriteAllText(DbPath, json);
    }

}