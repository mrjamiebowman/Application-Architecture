using MrJB.Packages.Abstractions.Models.Interfaces;

namespace MrJB.Packages.Abstractions.Queries;

public abstract class BaseSearchQuery : IModelSearchQuery
{
    public int? PageNo { get; set; }

    public int? PageSize { get; set; }
}
