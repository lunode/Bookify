using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Arpartments;

public record SearchApartmentsQuery(DateOnly StartDate, DateOnly EndDate) : IQuery<IReadOnlyList<ApartmentResponse>>;