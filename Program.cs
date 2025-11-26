using Spectre.Console;
using Spectre.Console;
using Spectre.Console.Rendering;

public static class Program
{
    public static async Task Main(string[] args)
    {
        AnsiConsole.Clear();
        WeatherService weatherService = new WeatherService();
        UserPreferences preferences = ConfigService.LoadPreferences();
        string location;


            if (args[0] == "--c" || args[0] == "-c")
            {

                var menu = AnsiConsole.Prompt(
                        new SelectionPrompt<string>().
                        Title("Select your option!")
                        .AddChoices(new[]{
                    "Enter Location",
                    "Preferences",
                    "Exit"
                            })
                        );
                switch (menu)
                {
                    case "Enter Location":
                        location = AnsiConsole.Prompt(new TextPrompt<string>("Enter your province or city! [blue](Buenos Aires, Caballito, Avellaneda)[/]:"));
                        await setLocation(location);
                        break;
                    case "Preferences":
                        setPreferences(preferences);
                        break;
                    case "Exit":
                        return;
                }
            }

        if (preferences.City == null)
        {
            location = AnsiConsole.Prompt(new TextPrompt<string>("Enter your province or city! [blue](Buenos Aires, Caballito, Avellaneda)[/]:"));
            await setLocation(location);
            return;
        }

        await weatherService.ShowWeather();
    }


    public static void setPreferences(UserPreferences preferences)
    {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select your preferences")
            .AddChoices("Theme", "Unit of measurement")
        );

        switch (option)
        {
            case "Theme":
                var themes = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("Choose visual style")
                        .NotRequired()
                        .InstructionsText(
            "[grey](Press [blue]<space>[/] to toggle a option, " +
            "[green]<enter>[/] to accept)[/]")
                        .AddChoices("Colorful", "Compact design")
                );


                preferences.Theme = themes.Contains("Colorful")
                    ? Theme.Colored
                    : Theme.Plain;


                preferences.Mode = themes.Contains("Compact design")
                    ? Verbosity.Compact
                    : Verbosity.Extended;

                break;

            case "Unit of measurement":
                var unit = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose unit")
                        .AddChoices("Celsius", "Fahrenheit")
                );

                preferences.Unit = unit == "Celsius"
                    ? Unit.Celsius
                    : Unit.Fahrenheit;

                break;
        }

        ConfigService.SavePreferences(preferences);

        AnsiConsole.MarkupLine("[green]Preferences updated[/]");
    }

    public static async Task setLocation(string location)
    {
        WeatherService weatherService = new();

        try
        {
            var result = await weatherService.GetCurrentWeatherAsync(location, true);


            if (result?.Hourly == null)
            {
                AnsiConsole.MarkupLine("[red]No weather data found.[/]");
                return;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Unexpected error:[/] {ex.Message}");
        }
    }
}
