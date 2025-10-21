using InmobiliariaWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Data;

public class InmuebleRepository
{
    // private readonly ApplicationDbContext _context;

    // public InmuebleRepository(ApplicationDbContext context)
    // {
    //     _context = context;
    // }

    public async Task<List<Inmueble>> GetAllAsync()
    {
        // return await _context.Inmuebles
        //     .Include(i => i.Dueño)
        //     .Include(i => i.Tipo)
        //     .ToListAsync();
        return new List<Inmueble>();
    }

    public async Task<Inmueble?> GetByIdAsync(int? id)
    {
        // return await _context.Inmuebles
        //     .Include(i => i.Dueño)
        //     .Include(i => i.Tipo)
        //     .FirstOrDefaultAsync(m => m.Id == id);
        return null;
    }
    
    public async Task<Inmueble?> FindAsync(int? id)
    {
        // return await _context.Inmuebles.FindAsync(id);
        return null;
    }

    public async Task CreateAsync(Inmueble inmueble)
    {
        // _context.Add(inmueble);
        // await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Inmueble inmueble)
    {
        // _context.Update(inmueble);
        // await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        // var inmueble = await _context.Inmuebles.FindAsync(id);
        // if (inmueble != null)
        // {
        //     _context.Inmuebles.Remove(inmueble);
        //     await _context.SaveChangesAsync();
        // }
    }

    public bool Exists(int id)
    {
        // return _context.Inmuebles.Any(e => e.Id == id);
        return false;
    }
    
    public IQueryable<Propietario> GetPropietarios()
    {
        // return _context.Propietarios;
        return new List<Propietario>().AsQueryable();
    }

    public IQueryable<TipoInmueble> GetTiposInmueble()
    {
        // return _context.TiposInmuebles;
        return new List<TipoInmueble>().AsQueryable();
    }
}
