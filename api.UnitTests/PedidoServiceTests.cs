using Xunit;
using Moq;
using api.Services;
using api.Repositories;
using api.Models;
using api.DTOs;

namespace api.UnitTests{
    public class PedidoServiceTests{
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly PedidoService _pedidoService;

        public PedidoServiceTests()
        {
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _pedidoService = new PedidoService(_pedidoRepositoryMock.Object);
        }

        [Fact]
        public async Task AddPedidoAsync_ShouldAddPedido_WhenValidDtoIsProvided(){
            var pedidoDto = new PedidoDto{
                PedidoId = "1",
                Itens = [
                    new ItemDto { Descricao = "Item 1", PrecoUnitario = 10, Qtd = 2 },
                    new ItemDto { Descricao = "Item 2", PrecoUnitario = 5, Qtd = 1 }
                ]
            };

            var result = await _pedidoService.AddPedidoAsync(pedidoDto);

            
            Assert.NotNull(result);
            Assert.Equal(2, result.Itens.Count);
            _pedidoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Pedido>()), Times.Once);
        }

        [Fact]
        public async Task GetPedidoAsync_ShouldThrowNotFoundException_WhenPedidoDoesNotExist() {
            var pedidoId = "999";
            
            _pedidoRepositoryMock.Setup(r => r.ExistsAsync(pedidoId)).ReturnsAsync(false); 

            await Assert.ThrowsAsync<InvalidOperationException>(() => _pedidoService.GetPedidoAsync(pedidoId));
        }

        [Fact]
        public async Task GetPedidoAsync_ShouldReturnPedido_WhenPedidoExists() {
            var pedidoId = "1";
            var pedido = new Pedido {
                PedidoId = pedidoId,
                Itens = [
                    new Item { Descricao = "Item 1", PrecoUnitario = 10, Qtd = 2, PedidoId = pedidoId },
                    new Item { Descricao = "Item 2", PrecoUnitario = 5, Qtd = 1, PedidoId = pedidoId }
                ]
            };

            _pedidoRepositoryMock.Setup(r => r.ExistsAsync(pedidoId)).ReturnsAsync(true);
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync(pedido);

            var result = await _pedidoService.GetPedidoAsync(pedidoId);

            Assert.NotNull(result);
            Assert.Equal(pedidoId, result.PedidoId);
            Assert.Equal(2, result.Itens.Count);
        }

        [Fact]
        public async Task UpdatePedidoAsync_ShouldUpdatePedido_WhenValidDtoIsProvided(){
            var pedidoId = "1";
            var existingPedido = new Pedido{
                PedidoId = pedidoId,
                Itens = [
                    new Item { Descricao = "Item 1", PrecoUnitario = 10, Qtd = 2, PedidoId = pedidoId },
                    new Item { Descricao = "Item 2", PrecoUnitario = 5, Qtd = 1, PedidoId = pedidoId }
                ]
            };

            _pedidoRepositoryMock.Setup(r => r.ExistsAsync(pedidoId)).ReturnsAsync(true);
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync(existingPedido);

            var updatedPedidoDto = new PedidoDto{
                PedidoId = pedidoId,
                Itens = [
                    new ItemDto { Descricao = "Item 1", PrecoUnitario = 10, Qtd = 3 },
                    new ItemDto { Descricao = "Item 3", PrecoUnitario = 8, Qtd = 1 }
                ]
            };

            await _pedidoService.UpdatePedidoAsync(updatedPedidoDto);

            
            _pedidoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Pedido>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePedidoAsync_ShouldThrowNotFoundException_WhenPedidoDoesNotExist(){
            var pedidoId = "999";
            var updatedPedidoDto = new PedidoDto{
                PedidoId = pedidoId,
                Itens = []
            };

            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(pedidoId)).ReturnsAsync((Pedido?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _pedidoService.UpdatePedidoAsync(updatedPedidoDto));
        }
    }
}