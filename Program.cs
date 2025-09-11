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
)  )  ( )       --'       `- __.'          :(      ))
.-'  (_.'          .')                    `(    )  ))
                  (_  )                     ` __.:'
                                        	
""";

        // Create the layout
        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Clima"),
                new Layout("Right")
                    .SplitRows(
                        new Layout("Precipitacion"),
                        new Layout("Humedad")));

        // Update the left column
        layout["Clima"].Update(
            new Panel(
                Align.Center(
                    new Markup($"{ascii}"),
                    VerticalAlignment.Middle))
                .Expand());

        

        // Render the layout
        AnsiConsole.Write(layout);


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
        Console.ReadKey();
    }

    public static async Task setLocation()
    {
        WeatherService weatherService = new();

        // var location = AnsiConsole.Prompt(
        //         new TextPrompt<string>("Ingrese su localidad! [blue](provincia, localidad, pais)[/]")
        //         );
        string location = "";
        try
        {
            var result = await weatherService.GetCurrentWeatherAsync(location);
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
