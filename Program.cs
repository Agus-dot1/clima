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

        foreach (var arg in args)
        {
            if (arg == "--c" || arg == "-c")
            {

                var menu = AnsiConsole.Prompt(
                        new SelectionPrompt<string>().
                        Title("Seleccione su opcion!")
                        .AddChoices(new[]{
                    "Ingresar Ubicación",
                    "Preferencias",
                    "Salir"
                            })
                        );
                switch (menu)
                {
                    case "Ingresar Ubicación":
                        location = AnsiConsole.Prompt(new TextPrompt<string>("Ingresa tu provincia o localidad! [blue](Buenos Aires, Caballito, Avellaneda)[/]:"));
                        await setLocation(location);
                        break;
                    case "Preferencias":
                        setPreferences(preferences);
                        break;
                    case "Salir":
                        return;
                }
            }
        }

        if (preferences.City == null)
        {
            location = AnsiConsole.Prompt(new TextPrompt<string>("Ingresa tu provincia o localidad! [blue](Buenos Aires, Caballito, Avellaneda)[/]:"));
            await setLocation(location);
            return;
        }

        await weatherService.ShowWeather();
    }


    public static void setPreferences(UserPreferences preferences)
    {
        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Selecciona tus preferencias")
            .AddChoices("Tema", "Unidad de medida")
        );

        switch (option)
        {
            case "Tema":
                var temas = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("Elegí el estilo visual")
                        .NotRequired()
                        .InstructionsText(
            "[grey](Press [blue]<space>[/] to toggle a option, " +
            "[green]<enter>[/] to accept)[/]")
                        .AddChoices("Colorido", "Diseño compacto")
                );


                preferences.Tema = temas.Contains("Colorido")
                    ? Theme.Colored
                    : Theme.Plain;


                preferences.Modo = temas.Contains("Diseño compacto")
                    ? Verbosity.Compact
                    : Verbosity.Extended;

                break;

            case "Unidad de medida":
                var unidad = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Elegí la unidad")
                        .AddChoices("Celsius", "Fahrenheit")
                );

                preferences.Unidad = unidad == "Celsius"
                    ? Unit.Celsius
                    : Unit.Fahrenheit;

                break;
        }

        ConfigService.SavePreferences(preferences);

        AnsiConsole.MarkupLine("[green]Preferencias actualizadas[/]");
    }

    public static async Task setLocation(string location)
    {
        WeatherService weatherService = new();

        try
        {
            var result = await weatherService.GetCurrentWeatherAsync(location, true);


            if (result?.Hourly == null)
            {
                AnsiConsole.MarkupLine("[red]No se encontraron datos de clima.[/]");
                return;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error inesperado:[/] {ex.Message}");
        }
    }
}
