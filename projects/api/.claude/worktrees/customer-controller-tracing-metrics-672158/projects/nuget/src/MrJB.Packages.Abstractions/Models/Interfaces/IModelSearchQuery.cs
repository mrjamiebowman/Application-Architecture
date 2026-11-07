namespace MrJB.Packages.Abstractions.Models.Interfaces;

public interface IModelSearchQuery
{
    public int? PageNo { get; set; }

    public int? PageSize { get; set; }
}
