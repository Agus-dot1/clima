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
    public Theme Theme { get; set; } = Theme.Colored;
    public Verbosity Mode { get; set; } = Verbosity.Extended;
    public Unit Unit { get; set; } = Unit.Celsius;
}
