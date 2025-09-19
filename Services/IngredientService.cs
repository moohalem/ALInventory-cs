using Models;
using Helpers;


namespace Services
{
    public class IngredientsService
    {
        private List<Ingredients> _ingredients = new List<Ingredients>();
        
        public IngredientsService()
        {
            // Ensure the database file and its directory exist when the service is created.
            HelperFunctions.DbPathChecker();
            _ingredients = HelperFunctions.LoadIngredients();
        }
        
        public IReadOnlyList<Ingredients> GetAllIngredients()
        {
            return HelperFunctions.LoadIngredients();
        }

        public Ingredients? GetIngredientById(int id) // Change return type to nullable
        {
            _ingredients = HelperFunctions.LoadIngredients();
            return _ingredients.FirstOrDefault(i => i.Id == id);
        }
        
        public bool AddIngredient(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity < 0)
                return false;

            _ingredients = HelperFunctions.LoadIngredients();

            if (_ingredients.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                // Ingredient with this name already exists.
                return false;
            }

            // The Max() approach for ID generation is acceptable here, as we are
            // already loading the entire list from the file.
            int nextId = _ingredients.Any() ? _ingredients.Max(i => i.Id) + 1 : 1;

            var newIngredient = new Ingredients
            {
                Id = nextId,
                Name = name,
                Quantity = quantity
            };

            _ingredients.Add(newIngredient);

            // Save the entire updated list back to the file.
            HelperFunctions.SaveChanges(_ingredients);
            return true;
        }

        public bool EditIngredient(int id, string newName, int? newQuantity)
        {
            _ingredients = HelperFunctions.LoadIngredients();
            var ingredientToEdit = _ingredients.FirstOrDefault(i => i.Id == id);

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
                HelperFunctions.SaveChanges(_ingredients);
            }

            return hasBeenModified;
        }

        public bool DeleteIngredient(int id)
        {
            _ingredients = HelperFunctions.LoadIngredients();
            var ingredientToRemove = _ingredients.FirstOrDefault(i => i.Id == id);

            if (ingredientToRemove == null)
                return false;
            
            _ingredients.Remove(ingredientToRemove);
            HelperFunctions.SaveChanges(_ingredients);
            
            return true;
        }
    }
}