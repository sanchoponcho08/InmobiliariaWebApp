using InmobiliariaWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Data;

public class InmuebleRepository
{
    private readonly ApplicationDbContext _context;

    public InmuebleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Inmueble>> GetAllAsync()
    {
        return await _context.Inmuebles
            .Include(i => i.Dueño)
            .Include(i => i.Tipo)
            .ToListAsync();
    }

    public async Task<Inmueble?> GetByIdAsync(int? id)
    {
        return await _context.Inmuebles
            .Include(i => i.Dueño)
            .Include(i => i.Tipo)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    
    public async Task<Inmueble?> FindAsync(int? id)
    {
        return await _context.Inmuebles.FindAsync(id);
    }

    public async Task CreateAsync(Inmueble inmueble)
    {
        _context.Add(inmueble);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Inmueble inmueble)
    {
        _context.Update(inmueble);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var inmueble = await _context.Inmuebles.FindAsync(id);
        if (inmueble != null)
        {
            _context.Inmuebles.Remove(inmueble);
            await _context.SaveChangesAsync();
        }
    }

    public bool Exists(int id)
    {
        return _context.Inmuebles.Any(e => e.Id == id);
    }
    
    public IQueryable<Propietario> GetPropietarios()
    {
        return _context.Propietarios;
    }

    public IQueryable<TipoInmueble> GetTiposInmueble()
    {
        return _context.TiposInmuebles;
    }
}
