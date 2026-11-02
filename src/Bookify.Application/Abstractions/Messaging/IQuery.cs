using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;

public interface IQuery<TResult> : IRequest<Result<TResult>>
{
}