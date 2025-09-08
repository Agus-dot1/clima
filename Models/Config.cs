public class Config {
    public string LocationName { get; set; }
    public Style Style { get; set; }
}

public class Style {
    public string Theme { get; set; } = "default";
    public string Verbosity { get; set; } = "extended";
}

