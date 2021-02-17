namespace Curiosity.Tools.Web.Middleware.IPFilter
{
    public class IPFilter
    {
        // private readonly RequestDelegate _next;
        // private readonly HashSet<IPAddress> _allowedIPs;
        // private readonly IReadOnlyCollection<IPNetwork> _allowedIpNetworks;
        // private readonly ILogger<IPFilter> _logger;
        //
        // public IPFilter(
        //     RequestDelegate next,
        //     IPFilterOptions options,
        //     ILogger<IPFilter> logger)
        // {
        //     if (options == null) throw new ArgumentNullException(nameof(options));
        //     _next = next ?? throw new ArgumentNullException(nameof(next));
        //     _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //
        //     _allowedIPs = new HashSet<IPAddress>();
        //     var allowedIPNetworks = new List<IPNetwork>();
        //     foreach (var allowedIp in options.AllowedIP ?? new string[0])
        //     {
        //         // это может быть конкретный IP
        //         if (IPAddress.TryParse(allowedIp, out var ip))
        //         {
        //             _allowedIPs.Add(ip);
        //         }
        //         else if (IPNetwork.TryParse(allowedIp, out var ipNetwork))
        //         {
        //             // либо сеть
        //             allowedIPNetworks.Add(ipNetwork);
        //         }
        //         else
        //         {
        //             throw new ArgumentException($"В {nameof(options)} указан некорректный IP - \"{allowedIp}\"");
        //         }
        //     }
        //
        //     _allowedIpNetworks = allowedIPNetworks;
        // }
        //
        // public Task Invoke(HttpContext context)
        // {
        //     // определим IP, откуда запрос пришёл,
        //     var ipAddress = context.Connection.RemoteIpAddress;
        //
        //     // по возможности переведем в IPv4
        //     if (ipAddress.IsIPv4MappedToIPv6)
        //         ipAddress = ipAddress.MapToIPv4();
        //
        //     // проверим по "белому" списку
        //     if (_allowedIPs.Contains(ipAddress))
        //     {
        //         // можно пускать
        //         return _next.Invoke(context);
        //     }
        //
        //     // проверим по сетям
        //     if (_allowedIpNetworks.Count > 0)
        //     {
        //         foreach (var ipNetwork in _allowedIpNetworks)
        //         {
        //             if (ipNetwork.Contains(ipAddress))
        //             {
        //                 // можно пускать
        //                 return _next.Invoke(context);
        //             }
        //         }
        //     }
        //
        //     // не прошёл IP контроль
        //     context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
        //     _logger.LogWarning($"Попытка доступа к ресурсам с IP \"{ipAddress}\", который не входит в список разрешённых IP");
        //     return Task.CompletedTask;
        // }
    }
}