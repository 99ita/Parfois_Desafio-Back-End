using api.Models;

namespace api.Repositories{
    public interface IPedidoRepository
    {
        Task<bool> ExistsAsync(string id);
        Task<Pedido?> GetByIdAsync(string id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task AddAsync(Pedido pedido);
        Task UpdateAsync(Pedido pedido);
        Task DeleteAsync(string id);
    }
}