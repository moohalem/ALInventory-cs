using Services;
using Models;

static class Program
{
    // The service is instantiated once and shared across the application.
    private static readonly IngredientsService _service = new IngredientsService();

    static void Main()
    {
        // The Init() method is no longer needed here.
        // The IngredientsService constructor now handles creating the Database/Data.json file.

        bool run = true;
        
        Console.WriteLine("Welcome to the ALIBOGA Inventory Management");
        Console.WriteLine("Press any key to enter the main menu...");
        Console.ReadKey();

        while (run)
        {
            string choice = ShowMenu();
            if (string.IsNullOrWhiteSpace(choice))
            {
                Console.WriteLine("\nInvalid input. Please enter a number. Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            switch (choice)
            {
                case "1":
                    ListAllMenu();
                    break;
                case "2":
                    AddMenu();
                    break;
                case "3":
                    EditMenu();
                    break;
                case "4":
                    DeleteMenu();
                    break;
                case "5":
                    Console.Clear();
                    Console.WriteLine("Thank you for using the ALIBOGA Inventory Management");
                    Thread.Sleep(2000); // Wait for 2 seconds before closing.
                    run = false; // Exit the loop.
                    break;
                default:
                    Console.WriteLine("\nInvalid choice. Please select a valid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static string ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine(" ALIBOGA INVENTORY MANAGEMENT");
        Console.WriteLine("========================================");
        Console.WriteLine("Enter menu option below:");
        Console.WriteLine("  1. List all ingredients");
        Console.WriteLine("  2. Add new ingredient");
        Console.WriteLine("  3. Edit an ingredient");
        Console.WriteLine("  4. Delete an ingredient");
        Console.WriteLine("  5. Exit");
        Console.WriteLine("----------------------------------------");
        Console.Write("Enter your choice: ");
        return Console.ReadLine();
    }

    /// <summary>
    /// A helper method to display a list of ingredients in a formatted way.
    /// This avoids duplicating code in other menu methods.
    /// </summary>
    private static void PrintIngredients(IReadOnlyList<Ingredients> ingredients)
    {
        Console.WriteLine("----------------------------------------");
        if (ingredients == null || ingredients.Count == 0)
        {
            Console.WriteLine("The ingredient list is empty.");
        }
        else
        {
            Console.WriteLine("ID | Name                 | Quantity");
            Console.WriteLine("---|----------------------|----------");
            foreach (var ingredient in ingredients)
            {
                // {ID, -3} means left-align ID in a 3-character space.
                // {Name, -20} means left-align Name in a 20-character space.
                Console.WriteLine($"{ingredient.Id,-3}| {ingredient.Name,-20}| {ingredient.Quantity} grams");
            }
        }
        Console.WriteLine("----------------------------------------");
    }

    private static void ListAllMenu()
    {
        Console.Clear();
        Console.WriteLine("Fetching all ingredients...");
        
        // 1. Get data from the service.
        var ingredients = _service.GetAllIngredients();

        // 2. The UI layer handles the presentation.
        PrintIngredients(ingredients);
        
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void AddMenu()
    {
        Console.Clear();
        Console.WriteLine("--- Add New Ingredient ---");
        
        Console.Write("Enter ingredient name: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nError: Name cannot be empty. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter ingredient quantity (in grams): ");
        if (!int.TryParse(Console.ReadLine(), out int quantityValue) || quantityValue < 0)
        {
            Console.WriteLine("\nError: Invalid quantity. Please enter a non-negative number. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (_service.AddIngredient(name, quantityValue))
        {
            Console.WriteLine("\nIngredient added successfully!");
        }
        else
        {
            Console.WriteLine($"\nError: An ingredient named '{name}' already exists.");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void EditMenu()
    {
        Console.Clear();
        Console.WriteLine("--- Edit an Ingredient ---");
        
        // First, show the user the list of available ingredients.
        var currentIngredients = _service.GetAllIngredients();
        PrintIngredients(currentIngredients);

        if (currentIngredients.Count == 0)
        {
             Console.WriteLine("\nNothing to edit. Press any key to return to the menu...");
             Console.ReadKey();
             return;
        }

        Console.Write("Enter the ID of the ingredient you want to edit: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nInvalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        // Check if the ingredient exists before asking for more details.
        var ingredientToEdit = _service.GetIngredientById(id);
        if (ingredientToEdit == null)
        {
            Console.WriteLine($"\nError: Ingredient with ID {id} not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write($"Enter new name for '{ingredientToEdit.Name}' (or press Enter to skip): ");
        string newName = Console.ReadLine();
        // If the user just presses Enter, newName will be empty. We treat it as null for the service.
        if (string.IsNullOrWhiteSpace(newName))
        {
            newName = null;
        }

        Console.Write($"Enter new quantity for '{ingredientToEdit.Name}' (or press Enter to skip): ");
        string newQuantityInput = Console.ReadLine();
        int? newQuantity = null; // Use a nullable int.
        if (int.TryParse(newQuantityInput, out int quantityValue))
        {
            newQuantity = quantityValue;
        }

        if (_service.EditIngredient(id, newName, newQuantity))
        {
            Console.WriteLine("\nEdit successful!");
            // Show the updated ingredient details.
            var updatedIngredient = _service.GetIngredientById(id);
            Console.WriteLine($"Updated: {updatedIngredient.Id}. {updatedIngredient.Name} => {updatedIngredient.Quantity} grams");
        }
        else
        {
            Console.WriteLine("\nNo changes were made.");
        }
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void DeleteMenu()
    {
        Console.Clear();
        Console.WriteLine("--- Delete an Ingredient ---");

        // Show the list so the user knows which ID to enter.
        var ingredients = _service.GetAllIngredients();
        PrintIngredients(ingredients);

        if (ingredients.Count == 0)
        {
             Console.WriteLine("\nNothing to delete. Press any key to return to the menu...");
             Console.ReadKey();
             return;
        }

        Console.Write("Enter the ID of the ingredient to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nInvalid ID format. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (_service.DeleteIngredient(id))
        {
            Console.WriteLine("\nIngredient deleted successfully.");
        }
        else
        {
            Console.WriteLine($"\nError: Ingredient with ID {id} not found.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}




