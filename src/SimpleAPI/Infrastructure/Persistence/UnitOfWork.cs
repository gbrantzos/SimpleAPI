using SimpleAPI.Domain;
using SimpleAPI.Domain.Core;

namespace SimpleAPI.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SimpleAPIContext _context;

    public UnitOfWork(SimpleAPIContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
