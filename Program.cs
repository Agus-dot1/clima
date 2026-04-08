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

        if (args.Length > 0)
        {
            if (args[0] == "--c" || args[0] == "-c")
            {
                var menu = AnsiConsole.Prompt(
                        new SelectionPrompt<string>().
                        Title(preferences.Language == Language.Spanish ? "¡Selecciona tu opción!" : "Select your option!")
                        .AddChoices(new[]{
                                preferences.Language == Language.Spanish ? "Ingresar Ubicación" : "Enter Location",
                                preferences.Language == Language.Spanish ? "Preferencias" : "Preferences",
                                preferences.Language == Language.Spanish ? "Salir" : "Exit"
                            })
                        );
                if (menu == "Enter Location" || menu == "Ingresar Ubicación")
                {
                    location = AnsiConsole.Prompt(new TextPrompt<string>(preferences.Language == Language.Spanish ? "¡Ingresa tu provincia o ciudad! [blue](Buenos Aires, Caballito, Avellaneda)[/]:" : "Enter your province or city! [blue](Buenos Aires, Caballito, Avellaneda)[/]:"));
                    await setLocation(location);
                }
                else if (menu == "Preferences" || menu == "Preferencias")
                {
                    setPreferences(preferences);
                }
                else if (menu == "Exit" || menu == "Salir")
                {
                    return;
                }
            }

        }
        if (preferences.City == null)
        {
            location = AnsiConsole.Prompt(new TextPrompt<string>(preferences.Language == Language.Spanish ? "¡Ingresa tu provincia o ciudad! [blue](Buenos Aires, Caballito, Avellaneda)[/]:" : "Enter your province or city! [blue](Buenos Aires, Caballito, Avellaneda)[/]:"));
            await setLocation(location);
            return;
        }

        await weatherService.ShowWeather();
    }


    public static void setPreferences(UserPreferences preferences)
    {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title(preferences.Language == Language.Spanish ? "Selecciona tus preferencias" : "Select your preferences")
            .AddChoices(
                preferences.Language == Language.Spanish ? "Tema" : "Theme",
                preferences.Language == Language.Spanish ? "Unidad de medida" : "Unit of measurement",
                preferences.Language == Language.Spanish ? "Idioma" : "Language"
            )
        );

        if (option == "Theme" || option == "Tema")
        {
            var themes = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title(preferences.Language == Language.Spanish ? "Elige el estilo visual" : "Choose visual style")
                    .NotRequired()
                    .InstructionsText(
        preferences.Language == Language.Spanish
            ? "[grey](Presiona [blue]<espacio>[/] para alternar, [green]<enter>[/] para aceptar)[/]"
            : "[grey](Press [blue]<space>[/] to toggle a option, [green]<enter>[/] to accept)[/]")
                    .AddChoices(
                        preferences.Language == Language.Spanish ? "Colorido" : "Colorful",
                        preferences.Language == Language.Spanish ? "Diseño compacto" : "Compact design"
                    )
            );


            preferences.Theme = (themes.Contains("Colorful") || themes.Contains("Colorido"))
                ? Theme.Colored
                : Theme.Plain;


            preferences.Mode = (themes.Contains("Compact design") || themes.Contains("Diseño compacto"))
                ? Verbosity.Compact
                : Verbosity.Extended;
        }

        else if (option == "Unit of measurement" || option == "Unidad de medida")
        {
            var unit = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(preferences.Language == Language.Spanish ? "Elige la unidad" : "Choose unit")
                    .AddChoices("Celsius", "Fahrenheit")
            );

            preferences.Unit = unit == "Celsius"
                ? Unit.Celsius
                : Unit.Fahrenheit;
        }
        else if (option == "Language" || option == "Idioma")
        {
            var lang = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(preferences.Language == Language.Spanish ? "Selecciona el idioma" : "Select language")
                    .AddChoices("English", "Spanish")
            );

            preferences.Language = lang == "English"
                ? Language.English
                : Language.Spanish;
        }

        ConfigService.SavePreferences(preferences);

        AnsiConsole.MarkupLine(preferences.Language == Language.Spanish ? "[green]Preferencias actualizadas[/]" : "[green]Preferences updated[/]");
    }

    public static async Task setLocation(string location)
    {
        WeatherService weatherService = new();

        try
        {
            var result = await weatherService.GetCurrentWeatherAsync(location, true);


            if (result?.Hourly == null)
            {
                UserPreferences preferences = ConfigService.LoadPreferences();
                AnsiConsole.MarkupLine(preferences.Language == Language.Spanish ? "[red]No se encontraron datos climáticos.[/]" : "[red]No weather data found.[/]");
                return;
            }
        }
        catch (Exception ex)
        {
            UserPreferences preferences = ConfigService.LoadPreferences();
            AnsiConsole.MarkupLine(preferences.Language == Language.Spanish ? $"[red]Error inesperado:[/] {ex.Message}" : $"[red]Unexpected error:[/] {ex.Message}");
        }
    }
}
