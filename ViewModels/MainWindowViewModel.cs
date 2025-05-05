using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using System.Linq;
using AOOP4.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AOOP4.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Recipe> recipes = new();

    [ObservableProperty]
    private Recipe? selectedRecipe;

    public MainWindowViewModel()
    {
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        var jsonContent = File.ReadAllText("ExerciseJSON.json");
        var jsonDocument = JsonDocument.Parse(jsonContent);
        var recipesArray = jsonDocument.RootElement.GetProperty("recipes");
        
        foreach (var recipeElement in recipesArray.EnumerateArray())
        {
            var recipe = new Recipe
            {
                Name = recipeElement.GetProperty("name").GetString() ?? string.Empty,
                Difficulty = recipeElement.GetProperty("difficulty").GetString() ?? string.Empty,
                Equipment = recipeElement.GetProperty("equipment").EnumerateArray()
                    .Select(e => e.GetString() ?? string.Empty)
                    .ToList(),
                Steps = recipeElement.GetProperty("steps").EnumerateArray()
                    .Select(s => new Step 
                    { 
                        StepDescription = s.GetProperty("step").GetString() ?? string.Empty,
                        Duration = s.GetProperty("duration").GetInt32()
                    })
                    .ToList()
            };
            Recipes.Add(recipe);
        }
    }
}
