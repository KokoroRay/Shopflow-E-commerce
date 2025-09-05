using System.ComponentModel.DataAnnotations;

namespace ShopFlow.API.Configurations;

public class LoggingOptions
{
    public const string SectionName = "Logging";

    public SerilogOptions Serilog { get; set; } = new();
}

public class SerilogOptions
{
    public bool EnableConsole { get; set; } = true;
    public bool EnableFile { get; set; } = true;
    public bool EnableSeq { get; set; } = false;
    
    public string FilePathTemplate { get; set; } = "logs/shopflow-.txt";
    public string SeqServerUrl { get; set; } = "http://localhost:5341";
    public string MinimumLevel { get; set; } = "Information";
    
    public bool EnableRequestLogging { get; set; } = true;
    public bool EnrichFromRequest { get; set; } = true;
}
