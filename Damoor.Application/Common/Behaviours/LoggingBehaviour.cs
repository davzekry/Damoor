using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Damoor.Application.Common.Behaviours
{ 
    public class LoggingBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var name = typeof(TRequest).Name;
            _logger.LogInformation("Handling {Request}: {@Payload}", name, request);

            var sw = Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            _logger.LogInformation("Handled {Request} in {Ms}ms", name, sw.ElapsedMilliseconds);
            return response;
        }
    }
}