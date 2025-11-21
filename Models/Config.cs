public enum Theme {
    Plain,
    Colored
}

public enum Verbosity {
    Extended,
    Compact
}

public enum Unit {
    Celsius,
    Fahrenheit
}  

public class UserPreferences {
    public string Name { get; set; }
    public string City { get; set; }
    public Theme Tema { get; set; } = Theme.Colored;
    public Verbosity Modo { get; set; } = Verbosity.Extended;
    public Unit Unidad { get; set; } = Unit.Celsius;
}
