using MrJB.Sample.Data.Entities;
using MrJB.Sample.Domain.Models;

namespace MrJB.Sample.Data.Mappers;

/// <summary>
/// Entity to domain model mapping. Keeps persistence entities from leaking past the data layer.
/// </summary>
public static class CustomerMapper
{
    public static Customer ToModel(this CustomerEntity entity) => new()
    {
        CustomerId = entity.CustomerId,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        PhoneNumber = entity.PhoneNumber,
    };
}
