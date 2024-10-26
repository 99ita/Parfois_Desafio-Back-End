using api.Repositories;
using api.Models;
using api.DTOs;

namespace api.Services{
    public class PedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository){
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PedidoDto?> GetPedidoAsync(string id) {
            var existsPedido = await _pedidoRepository.ExistsAsync(id);
            if (!existsPedido) throw new InvalidOperationException($"Pedido with ID {id} not found.");
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null){
                return null;
            }
            return MapToDto(pedido);
        }

        public async Task<PedidoDto> AddPedidoAsync(PedidoDto pedidoDto){
            ValidatePedidoDto(pedidoDto);
            var pedido = MapToModel(pedidoDto);
            await _pedidoRepository.AddAsync(pedido);
            return pedidoDto;
        }

        public async Task UpdatePedidoAsync(PedidoDto pedidoDto) {
            var existsPedido = await _pedidoRepository.ExistsAsync(pedidoDto.PedidoId);
            if (!existsPedido) throw new InvalidOperationException($"Pedido with ID {pedidoDto.PedidoId} not found.");

            ValidatePedidoDto(pedidoDto);

            var existingPedido = await _pedidoRepository.GetByIdAsync(pedidoDto.PedidoId);
            if (existingPedido == null) throw new InvalidOperationException("Pedido not found");

            existingPedido.Itens.Clear();
            foreach (var itemDto in pedidoDto.Itens) {
                existingPedido.Itens.Add(new Item {
                    Descricao = itemDto.Descricao,
                    PrecoUnitario = itemDto.PrecoUnitario,
                    Qtd = itemDto.Qtd,
                    PedidoId = pedidoDto.PedidoId
                });
            }

            await _pedidoRepository.UpdateAsync(existingPedido);
        }

        public async Task DeletePedidoAsync(string id){
            var existsPedido = await _pedidoRepository.ExistsAsync(id);
            if (!existsPedido) throw new InvalidOperationException($"Pedido with ID {id} not found.");
            await _pedidoRepository.DeleteAsync(id);
        }

        private void ValidatePedidoDto(PedidoDto pedidoDto){
            if (pedidoDto == null) throw new ArgumentNullException(nameof(pedidoDto), "Pedido cannot be null");

            if (string.IsNullOrWhiteSpace(pedidoDto.PedidoId)) throw new ArgumentException("Pedido ID cannot be null or empty", nameof(pedidoDto.PedidoId));
            
            if (pedidoDto.Itens == null || !pedidoDto.Itens.Any()) throw new ArgumentException("Pedido must contain at least one item", nameof(pedidoDto.Itens));
            
            foreach (var item in pedidoDto.Itens){
                if (string.IsNullOrWhiteSpace(item.Descricao)) throw new ArgumentException("Item description cannot be null or empty", nameof(item.Descricao));
                if (item.PrecoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(item.PrecoUnitario), "Item price cannot be negative");
                if (item.Qtd < 0) throw new ArgumentOutOfRangeException(nameof(item.Qtd), "Item quantity cannot be negative");
            }
        }

        private Pedido MapToModel(PedidoDto pedidoDto){
            return new Pedido{
                PedidoId = pedidoDto.PedidoId,
                Itens = pedidoDto.Itens.Select(item => new Item{
                    Descricao = item.Descricao,
                    PrecoUnitario = item.PrecoUnitario,
                    Qtd = item.Qtd,
                    PedidoId = pedidoDto.PedidoId
                }).ToList()
            };
        }

        private PedidoDto MapToDto(Pedido pedido){
            return new PedidoDto{
                PedidoId = pedido.PedidoId,
                Itens = pedido.Itens.Select(i => new ItemDto{
                    Descricao = i.Descricao,
                    PrecoUnitario = i.PrecoUnitario,
                    Qtd = i.Qtd
                }).ToList()
            };
        }

    }
}