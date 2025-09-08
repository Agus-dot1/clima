using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Program
{
    public static async Task Main(string[] args)
    {
        AnsiConsole.Clear();

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
                await setLocation();
                break;
        }
    }

    public static async Task setLocation()
    {
        WeatherService weatherService = new();

        var location = AnsiConsole.Prompt(
                new TextPrompt<string>("Ingrese su localidad! [blue](provincia, localidad, pais)[/]")
                );

        try
        {
            var result = await weatherService.GetCurrentWeatherAsync(location);
            if (result == null)
                AnsiConsole.MarkupLine("[red]No se encontraron datos de clima.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error inesperado:[/] {ex.Message}");
        }
    }
}
