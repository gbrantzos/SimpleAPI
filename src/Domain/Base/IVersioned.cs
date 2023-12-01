namespace SimpleAPI.Domain.Base;

/// <summary>
/// <b>Versioned entities</b>
/// <para>
/// Add <see cref="RowVersion"/> property to support optimistic concurrency.
/// Not all entities should be versioned, mainly aggregates
/// </para>
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>
/// Changes on detail entities should increase version on parent
/// </item>
/// <item>
/// Version should be send on UI as part of the model
/// </item>
/// <item>
/// Consider using specific view model <code>VersionedViewModel</code> and/or entity <code>VersionEntity</code> 
/// </item>
/// </list>
/// </remarks>
public interface IVersioned
{
    public int RowVersion { get; set; }
}
