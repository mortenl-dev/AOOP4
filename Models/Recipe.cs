using System.Collections.Generic;

namespace AOOP4.Models;

public class Recipe
{
    public string Name { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public List<string> Equipment { get; set; } = new();
    public List<Step> Steps { get; set; } = new();
}

public class Step
{
    public string StepDescription { get; set; } = string.Empty;
    public int Duration { get; set; }
} 