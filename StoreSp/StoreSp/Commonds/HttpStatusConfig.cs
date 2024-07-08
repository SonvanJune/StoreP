using System.Net;
using Google.Rpc;

namespace StoreSp.Commons;

public class HttpStatusConfig
{
    public HttpStatusCode status{get; set; }
    public string? message{get; set; }

    public object? data{get; set; }
}
