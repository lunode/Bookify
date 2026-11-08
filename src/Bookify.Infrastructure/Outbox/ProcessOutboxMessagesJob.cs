using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(
    ISqlConnectionFactory sqlConnectionFactory,
    IPublisher publisher,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxMessagesJob> logger
) : IJob
{
    private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All
    };
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Beginning to process outbox message.");
        using var connection = sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        var outboxMessages = await GetOutboxMessage(connection, transaction);
        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                var domianEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, jsonSerializerSettings)!;
                await publisher.Publish(domianEvent, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while processing outbox message {MessageId}", outboxMessage.Id);
                exception = e;
            }
            await UpdateOutBoxMessage(connection, transaction, outboxMessage, exception);
        }
        transaction.Commit();
        logger.LogInformation("Completed processing outbox message");
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessage(IDbConnection connection, IDbTransaction transaction)
    {
        string sql = $"""
                      SELECT id, content
                      FROM outbox_messages
                      WHERE processed_on_utc IS NULL
                      ORDER BY occurred_on_utc
                      LIMIT {outboxOptions.Value.BatchSize}
                      FOR UPDATE
                      """;
        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);
        return outboxMessages.ToList();
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);

    private async Task UpdateOutBoxMessage(IDbConnection connection, IDbTransaction transaction, OutboxMessageResponse outboxMessage, Exception? exception)
    {
        const string sql = @"
            UPDATE outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = @Error
            WHERE id = @Id";
        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = outboxMessage.Id,
                ProcessedOnUtc = dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction);
    }
}
