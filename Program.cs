using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using Spectre.Console.Rendering;

public static class Program
{
    public static async Task Main(string[] args)
    {
        AnsiConsole.Clear();
        string location = "Buenos Aires";

        foreach(var arg in args){
            if(arg == "--c" || arg == "-c"){
                
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
        }

            }
        }

   



        
        
    }

    public static async Task setLocation(string location)
    {
        WeatherService weatherService = new();


        // var location = AnsiConsole.Prompt(
        //         new TextPrompt<string>("Ingrese su localidad! [blue](provincia, localidad, pais)[/]")
        //         );
        try
        {
            var result = await weatherService.GetCurrentWeatherAsync(location);


            if (result?.Hourly == null)
            {
                AnsiConsole.MarkupLine("[red]No se encontraron datos de clima.[/]");
                return;
            }
                
          



            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.Title("Clima de hoy");

            table.AddColumn(new TableColumn("Hora").Centered());
            table.AddColumn(new TableColumn("Temperatura (°C)").Centered());
            table.AddColumn(new TableColumn("Clima").Centered());

            int[] posiciones = { 8, 10, 12, 14, 16, 18, 20, 22, 23 };

            string ColorearTemp(double temp)
            {
                if (temp < 18) return $"[blue]{temp:0.#}°C[/]";
                if (temp < 25) return $"[yellow]{temp:0.#}°C[/]";
                return $"[red]{temp:0.#}°C[/]";
            }

            string IconoClima(int hora)
            {
                if (hora >= 6 && hora <= 18)
                {
                    if (hora < 10) return "☀";
                    if (hora < 16) return " ⛅";
                    return "☁";
                }
                else
                {
                    return "  🌛";
                }
            }

            foreach (var pos in posiciones)
            {
                table.AddRow(
                    $"[bold]{pos:00}:00[/]",
                    ColorearTemp(result.Hourly.TemperatureC[pos + 2]),
                    IconoClima(pos)
                );
            }

            string nubesAscii = @"
      .--.                 .---.
   .-(    )..--.        .-(     ).
  (___.__)__)___).     (___.__)___)
";

            AnsiConsole.WriteLine(nubesAscii);
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Temperaturas de la semana:");


            var grid = new Grid();
            // Cantidad de columnas = cantidad de días
            for (int i = 0; i < result.Daily.temperature_2m_mean.Length; i++)
            {
                grid.AddColumn(new GridColumn().Centered());
            }

            int aux = 0;
            List<IRenderable> columnas = new();

            foreach (var _ in result.Daily.temperature_2m_mean)
            {
                double mean = result.Daily.temperature_2m_mean[aux];
                double min = result.Daily.temperature_2m_min[aux];
                double max = result.Daily.temperature_2m_max[aux];

                string icono = mean switch
                {
                    < 15 => "☁",
                    < 22 => "⛅",
                    < 28 => "☀",
                    _ => "🔥"
                };

                var dayTable = new Table();
                dayTable.Border = TableBorder.Rounded;
                dayTable.AddColumn(new TableColumn($"[bold]{icono} Día {aux + 1}[/]").Centered().Width(25));
                dayTable.AddRow($"[yellow]Promedio: {mean:0.#}°C[/]");
                dayTable.AddRow($"[blue]Min: {min:0.#}°C[/]\n[red]Max: {max:0.#}°C[/]");

                columnas.Add(dayTable);
                aux++;
            }

            grid.AddRow(columnas.ToArray());
            AnsiConsole.Write(grid);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error inesperado:[/] {ex.Message}");
        }
    }
}
