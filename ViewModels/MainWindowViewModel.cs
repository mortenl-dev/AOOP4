using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using System.Linq;
using AOOP4.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Timers;

namespace AOOP4.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Recipe> recipes = new();

    [ObservableProperty]
    private Recipe? selectedRecipe;

    [ObservableProperty]
    private bool isCooking;

    [ObservableProperty]
    private int currentStepIndex = -1;

    [ObservableProperty]
    private double cookingProgress;

    [ObservableProperty]
    private string currentStepDescription = string.Empty;

    [ObservableProperty]
    private int totalCookingTime;

    [ObservableProperty]
    private int currentCookingTime;

    public MainWindowViewModel()
    {
        LoadRecipes();
    }

    [RelayCommand]
    private async Task StartCookingAsync()
    {
        if (SelectedRecipe == null || IsCooking) return;

        IsCooking = true;
        CurrentStepIndex = -1;
        CookingProgress = 0;
        CurrentStepDescription = "Preparing to cook...";
        
        TotalCookingTime = SelectedRecipe.Steps.Sum(s => s.Duration);
        CurrentCookingTime = 0;

        foreach (var step in SelectedRecipe.Steps.Select((step, index) => new { step, index }))
        {
            CurrentStepIndex = step.index;
            CurrentStepDescription = step.step.StepDescription;
            
            // Simulate the step duration
            for (int i = 0; i < step.step.Duration; i++)
            {
                CurrentCookingTime++;
                CookingProgress = (double)CurrentCookingTime / TotalCookingTime * 100;
                await Task.Delay(1000); // Each minute is simulated as 1 second
            }
        }

        CurrentStepDescription = "Recipe completed!";
        await Task.Delay(2000);
        
        // Reset cooking state
        IsCooking = false;
        CurrentStepIndex = -1;
        CookingProgress = 0;
        CurrentStepDescription = string.Empty;
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
