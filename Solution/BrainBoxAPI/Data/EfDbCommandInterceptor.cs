using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace BrainBoxAPI.Data
{
    public class EfDbCommandInterceptor : DbCommandInterceptor
    {
        private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "EFCoreLog.txt");

        public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
        {
            LogCommand(command.CommandText);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
        {
            LogCommand(command.CommandText);

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private void LogCommand(string commandText)
        {
            try
            {
                File.AppendAllText(LogFilePath, $"{DateTime.UtcNow} - {commandText}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging command: {ex.Message}");
            }
        }
    }
}
