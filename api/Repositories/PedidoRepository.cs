using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Data;

namespace api.Repositories{

    public class PedidoRepository : IPedidoRepository{
        private readonly ApiDbContext _context;

        public PedidoRepository(ApiDbContext context){
            _context = context;
        }

        public async Task<bool> ExistsAsync(string id) => await _context.Pedidos.AnyAsync(p => p.PedidoId == id);

        public async Task<Pedido?> GetByIdAsync(string id) => await _context.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.PedidoId == id);
        
        public async Task<IEnumerable<Pedido>> GetAllAsync() => await _context.Pedidos.Include(p => p.Itens).ToListAsync();
        
        public async Task AddAsync(Pedido pedido){
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync(); 
        }
        
        public async Task UpdateAsync(Pedido pedido){
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(string id){
            var pedido = await GetByIdAsync(id);
            if (pedido != null) {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync(); 
            }
        }
    }    
}
