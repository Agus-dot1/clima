```text
  ██████╗██╗     ██╗███╗   ███╗ █████╗ 
 ██╔════╝██║     ██║████╗ ████║██╔══██╗
 ██║     ██║     ██║██╔████╔██║███████║
 ██║     ██║     ██║██║╚██╔╝██║██╔══██║
 ╚██████╗███████╗██║██║ ╚═╝ ██║██║  ██║
  ╚═════╝╚══════╝╚═╝╚═╝     ╚═╝╚═╝  ╚═╝

           Terminal Weather
```

# Clima

A simple and clean command-line weather tool built with .NET and Spectre.Console. It provides a visual way to check the weather directly from your terminal using data from the Open-Meteo API.

## Current status

The project is fully functional and supports a few core features:
* Daily weather breakdown with temperature and conditions.
* Weekly forecast with average, minimum, and maximum temperatures.
* Persistent user preferences saved locally in a JSON file.
* Customizable options including temperature units (Celsius/Fahrenheit) and visual themes.
* Bilingual support for both English and Spanish cuz i can.

## How it works

The app uses the Open-Meteo Geocoding API to find your city and the main Forecast API to retrieve weather data. It remembers your location so you don't have to enter it every time you run the tool.

## Future plans

I am looking to expand the tool with more data and better flexibility:
* Preferences that actually work and are persistent.
* Scoop installation.
* More visual themes and specialized layouts for smaller terminals.
