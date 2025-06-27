using ALInventory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ALInventory.Services
{
    public class IngredientsService
    {
        // Define the path to our JSON database file.
        private const string DbPath = "Database/Data.json";

        public IngredientsService()
        {
            // Ensure the database file and its directory exist when the service is created.
            EnsureDatabaseFileExists();
        }

        // --- Public Data-Access Methods ---

        public IReadOnlyList<Ingredients> GetAllIngredients()
        {
            return LoadIngredients();
        }

        public Ingredients GetIngredientById(int id)
        {
            var ingredients = LoadIngredients();
            return ingredients.FirstOrDefault(i => i.Id == id);
        }

        // --- Public Data-Modification Methods ---

        public bool AddIngredient(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity < 0)
                return false;

            // Load the current list of ingredients from the file.
            var ingredients = LoadIngredients();

            if (ingredients.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                // Ingredient with this name already exists.
                return false;
            }

            // The Max() approach for ID generation is acceptable here, as we are
            // already loading the entire list from the file.
            int nextId = ingredients.Any() ? ingredients.Max(i => i.Id) + 1 : 1;

            var newIngredient = new Ingredients
            {
                Id = nextId,
                Name = name,
                Quantity = quantity
            };

            ingredients.Add(newIngredient);

            // Save the entire updated list back to the file.
            SaveChanges(ingredients);
            return true;
        }

        public bool EditIngredient(int id, string newName, int? newQuantity)
        {
            var ingredients = LoadIngredients();
            var ingredientToEdit = ingredients.FirstOrDefault(i => i.Id == id);

            if (ingredientToEdit == null)
                return false;

            bool hasBeenModified = false;
            if (!string.IsNullOrWhiteSpace(newName))
            {
                ingredientToEdit.Name = newName;
                hasBeenModified = true;
            }

            if (newQuantity.HasValue && newQuantity.Value >= 0)
            {
                ingredientToEdit.Quantity = newQuantity.Value;
                hasBeenModified = true;
            }

            if (hasBeenModified)
            {
                SaveChanges(ingredients);
            }

            return hasBeenModified;
        }

        public bool DeleteIngredient(int id)
        {
            var ingredients = LoadIngredients();
            var ingredientToRemove = ingredients.FirstOrDefault(i => i.Id == id);

            if (ingredientToRemove == null)
                return false;
            
            ingredients.Remove(ingredientToRemove);
            SaveChanges(ingredients);
            
            return true;
        }


        // --- Private Helper Methods for File I/O ---

        /// <summary>
        /// Reads all ingredients from the JSON file.
        /// </summary>
        /// <returns>A list of ingredients.</returns>
        private List<Ingredients> LoadIngredients()
        {
            // If the file doesn't exist, return an empty list.
            if (!File.Exists(DbPath))
            {
                return new List<Ingredients>();
            }

            var json = File.ReadAllText(DbPath);

            // If the file is empty or just whitespace, return an empty list.
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Ingredients>();
            }
            
            // Deserialize the JSON from the file into a List of Ingredients.
            return JsonSerializer.Deserialize<List<Ingredients>>(json);
        }

        /// <summary>
        /// Writes the entire list of ingredients to the JSON file, overwriting it.
        /// </summary>
        /// <param name="ingredients">The list of ingredients to save.</param>
        private void SaveChanges(List<Ingredients> ingredients)
        {
            // Configure serializer to write indented JSON for human readability.
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(ingredients, options);
            File.WriteAllText(DbPath, json);
        }

        /// <summary>
        /// Checks if the database file and directory exist, creating them if they don't.
        /// </summary>
        private void EnsureDatabaseFileExists()
        {
            var directory = Path.GetDirectoryName(DbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(DbPath))
            {
                // Create an empty JSON array `[]` in the new file.
                File.WriteAllText(DbPath, "[]");
            }
        }
    }
}