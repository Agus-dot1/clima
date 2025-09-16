using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;


public static class Program
{
    public static async Task Main(string[] args)
    {
        AnsiConsole.Clear();


        string ascii = """
              .
               					
              |					
     .               /				
      \       I     				
                  /
        \  ,g88R_
          d888(`  ).                   _
 -  --==  888(     ).=--           .+(`  )`.
)         Y8P(       '`.          :(   .    )
        .+(`(      .   )     .--  `.  (    ) )
       ((    (..__.:'-'   .=(   )   ` _`  ) )
`.     `(       ) )       (   .  )     (   )  ._
  )      ` __.:'   )     (   (   ))     `-'.:(`  )
)
    )  ( )       --'       `- __.'          :(      ))
.-'  (_.'          .')                    `(    )  ))
                  (_  )                     ` __.:'
                                        	
""";
        await setLocation();




        // var menu = AnsiConsole.Prompt(
        //         new SelectionPrompt<string>().
        //         Title("Seleccione su opcion!")
        //         .AddChoices(new[]{
        //             "Ingresar Ubicación",
        //             "Preferencias",
        //             "Salir"
        //             })
        //         );
        // switch (menu)
        // {
        //     case "Ingresar Ubicación":
        //         await setLocation();

        //         break;
        // }
    }

    public static async Task setLocation()
    {
        WeatherService weatherService = new();


        // var location = AnsiConsole.Prompt(
        //         new TextPrompt<string>("Ingrese su localidad! [blue](provincia, localidad, pais)[/]")
        //         );
        string location = "Buenos aires";
        try
        {
            var country = await weatherService.GetCoordinatesAsync(location);
            var result = await weatherService.GetCurrentWeatherAsync(location);


            if (result?.Hourly == null)
            {
                AnsiConsole.MarkupLine("[red]No se encontraron datos de clima.[/]");
                return;
            }

            // Create a table
            var table = new Table();

            // Add some columns
            table.AddColumn($"Clima para hoy [blue]{country.Country}[/]");
            table.AddColumn(new TableColumn("Sensación térmica").Centered());

            // foreach (var degree in result.Daily.temperature_2m_min[0])
            //     table.AddRow($"[blue]{degree}[/]");

            // foreach (var degree in result.Daily.temperature_2m_mean[2])
            //     table.AddRow($"{degree}");

            // foreach (var degree in result.Daily.temperature_2m_max[3])
            //     table.AddRow($"[red]{degree}[/]");
            table.AddRow($"{result.Daily.temperature_2m_mean[2]}");
            table.AddRow(new Panel($"[blue]{result.Daily.temperature_2m_min[0]}[/]")
                    , new Panel($"[red]{result.Daily.temperature_2m_max[3]}[/]"));



            // Add some rows
            table.AddRow(new Markup("[blue]Corgi[/]"), new Panel("Waldo"));
            // Render the table to the console
            AnsiConsole.Write(table);

        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error inesperado:[/] {ex.Message}");
        }
    }
}
