using System.Net;

namespace StoreSp.Commonds;

public class HttpStatusConfig
{
    public HttpStatusCode status{get; set; }
    public string? message{get; set; }

    public object? data{get; set; }
}
