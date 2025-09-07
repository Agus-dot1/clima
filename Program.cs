using Spectre.Console;
using System.IO;
using clima.Models;
using System.Net.Http;


public static class Program
{
    public static async Task Main(string[] args)
    {
        // string mainDir = AppDomain.CurrentDomain.BaseDirectory;
        // string config = "config/config.json";
        // string configPath = Path.Combine(mainDir, config);
        //
        // if (!File.Exists(configPath)){
        //     try{
        //         FileStream fs = File.Create(configPath);
        //         fs.Close();
        //     }catch(Exception){
        //         AnsiConsole.Markup("[red]Error al crear archivo de configuracion[/]");        
        //     }
        // }else{
        //
        // }
        //
		await Response();
    }


		public static async Task Response (){

				try{
				var client = new HttpClient();
				var response = await client.GetAsync("https://geocoding-api.open-meteo.com/v1/search?name=Paris&count=1");
				var jsonString = await response.Content.ReadAsStringAsync();
				Console.WriteLine("Raw JSON:");
				Console.WriteLine(jsonString);
				}catch(Exception ex){
						AnsiConsole.Markup($"Hubo un error! {ex.Message}");
				}
		
		}

}
 
