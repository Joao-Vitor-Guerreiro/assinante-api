using AssinanteAPI.Application.Interfaces;
using AssinanteAPI.Domain.Entities;
using AssinanteAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssinanteAPI.Infrastructure.Repositories;

public class AssinanteRepository : IAssinanteRepository
{
    private readonly ApplicationDbContext _context;

    public AssinanteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assinante?> ObterPorIdAsync(int id)
    {
        return await _context.Assinantes.FindAsync(id);
    }

    public async Task<Assinante?> ObterPorEmailAsync(string email)
    {
        return await _context.Assinantes
            .FirstOrDefaultAsync(a => a.Email == email.ToLower());
    }

    public async Task<List<Assinante>> ObterTodosAtivosAsync()
    {
        return await _context.Assinantes
            .Where(a => a.Status == Domain.Enums.StatusAssinatura.Ativo)
            .ToListAsync();
    }

    public async Task<(List<Assinante> assinantes, int totalCount)> ObterTodosAtivosPaginadosAsync(int pageNumber, int pageSize)
    {
        var query = _context.Assinantes
            .Where(a => a.Status == Domain.Enums.StatusAssinatura.Ativo);

        var totalCount = await query.CountAsync();

        var assinantes = await query
            .OrderBy(a => a.NomeCompleto)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (assinantes, totalCount);
    }

    public async Task<List<Assinante>> ObterTodosAsync()
    {
        return await _context.Assinantes.ToListAsync();
    }

    public async Task AdicionarAsync(Assinante assinante)
    {
        await _context.Assinantes.AddAsync(assinante);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Assinante assinante)
    {
        _context.Assinantes.Update(assinante);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Assinante assinante)
    {
        _context.Assinantes.Remove(assinante);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteEmailAsync(string email, int? id = null)
    {
        var query = _context.Assinantes
            .Where(a => a.Email == email.ToLower());

        if (id.HasValue)
        {
            query = query.Where(a => a.Id != id.Value);
        }

        return await query.AnyAsync();
    }
}
