using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ferremas.Api.Data;
using Ferremas.Api.Models;
using System.Collections.Generic;

namespace Ferremas.Api.Services
{
    public interface ILoggingService
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogCritical(Exception ex, string message, params object[] args);
        void LogTrace(string message, params object[] args);
        void LogWithContext(string message, LogLevel level, Dictionary<string, object>? context = null);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly AppDbContext _context;

        public LoggingService(ILogger<LoggingService> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args ?? Array.Empty<object>());
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args ?? Array.Empty<object>());
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args ?? Array.Empty<object>());
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args ?? Array.Empty<object>());
        }

        public void LogCritical(Exception ex, string message, params object[] args)
        {
            _logger.LogCritical(ex, message, args ?? Array.Empty<object>());
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args ?? Array.Empty<object>());
        }

        public void LogWithContext(string message, LogLevel level, Dictionary<string, object>? context = null)
        {
            using (_logger.BeginScope(context ?? new Dictionary<string, object>()))
            {
                switch (level)
                {
                    case LogLevel.Trace:
                        _logger.LogTrace(message);
                        break;
                    case LogLevel.Debug:
                        _logger.LogDebug(message);
                        break;
                    case LogLevel.Information:
                        _logger.LogInformation(message);
                        break;
                    case LogLevel.Warning:
                        _logger.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        _logger.LogError(message);
                        break;
                    case LogLevel.Critical:
                        _logger.LogCritical(message);
                        break;
                    default:
                        _logger.LogInformation(message);
                        break;
                }
            }
        }

        public async Task LogError(string message, Exception? exception = null)
        {
            _logger.LogError(exception, message);

            var log = new Log
            {
                Nivel = "Error",
                Mensaje = message,
                Excepcion = exception?.ToString() ?? string.Empty,
                Fecha = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogInfo(string message)
        {
            _logger.LogInformation(message);

            var log = new Log
            {
                Nivel = "Info",
                Mensaje = message,
                Excepcion = string.Empty,
                Fecha = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogWarning(string message)
        {
            _logger.LogWarning(message);

            var log = new Log
            {
                Nivel = "Warning",
                Mensaje = message,
                Excepcion = string.Empty,
                Fecha = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
} 