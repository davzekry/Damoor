using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Damoor.API.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(static (ctx, lc) => lc
            .ReadFrom.Configuration(ctx.Configuration)
            .WriteTo.Console()
            .WriteTo.MSSqlServer(
                ctx.Configuration.GetConnectionString("DefaultConnection"),
                new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true
                })
            .Enrich.FromLogContext()
            .Enrich.With());

        return builder;
    }
}