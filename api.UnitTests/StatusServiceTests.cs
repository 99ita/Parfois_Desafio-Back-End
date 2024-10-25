using Xunit;
using Moq;
using System.Threading.Tasks;
using api.Services;
using api.Repositories;
using api.DTOs;
using api.Models;
using System.Collections.Generic;

namespace api.UnitTests{
    public class StatusServiceTests{
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly StatusService _statusService;

        public StatusServiceTests(){
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _statusService = new StatusService(_pedidoRepositoryMock.Object);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnInvalidCode_WhenPedidoDoesNotExist(){
            var statusRequest = GenDefaultStatusRequest("1", "REPROVADO", 0, 0);
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync((Pedido?)null);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("CODIGO_PEDIDO_INVALIDO", result.Status);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnReprovado_WhenStatusIsReprovado(){
            var statusRequest = GenDefaultStatusRequest("1", "REPROVADO", 0, 0);
            var pedido = GenDefaultPedido();

            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync(pedido);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("REPROVADO", result.Status);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnAprovado_WhenStatusIsAprovadoAndValuesMatch(){
            var statusRequest = GenDefaultStatusRequest("1", "APROVADO", 2, 20);

            var pedidoId = "1";
            var pedido = new Pedido{
                PedidoId = pedidoId,
                Itens = [
                    new Item { Descricao = "Item 1", PrecoUnitario = 10, Qtd = 2, PedidoId = pedidoId}
                ]
            };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync(pedido);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("APROVADO", result.Status);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnAprovadoValorAMenor_WhenAprovadoAndValorIsLess(){
            var statusRequest = GenDefaultStatusRequest("1", "APROVADO", 2, 15);

            var pedido = GenDefaultPedido();
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync(pedido);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("APROVADO_VALOR_A_MENOR", result.Status);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnAprovadoValorAMaior_WhenValorAprovadoIsMore(){
            var statusRequest = GenDefaultStatusRequest("1", "APROVADO", 2, 25);

            var pedido = GenDefaultPedido();
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync(pedido);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("APROVADO_VALOR_A_MAIOR", result.Status);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnAprovadoQTDAMenor_WhenItensAprovadosAreLess(){
            var statusRequest = GenDefaultStatusRequest("1", "APROVADO", 1, 20);

            var pedido = GenDefaultPedido();
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync(pedido);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("APROVADO_QTD_A_MENOR", result.Status);
        }

        [Fact]
        public async Task AnswerStatusAsync_ShouldReturnAprovadoQTDAMaior_WhenItensAprovadosAreMore(){
            var statusRequest = GenDefaultStatusRequest("1", "APROVADO", 3, 20);

            var pedido = GenDefaultPedido();
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(statusRequest.Pedido)).ReturnsAsync(pedido);

            var result = await _statusService.AnswerStatusAsync(statusRequest);

            Assert.NotNull(result);
            Assert.Equal(statusRequest.Pedido, result.Pedido);
            Assert.Contains("APROVADO_QTD_A_MAIOR", result.Status);
        }

        private static StatusRequestDto GenDefaultStatusRequest(string pedidoId, string status, int itensAprovados, int valorAprovado){
            return new StatusRequestDto{
                Pedido = pedidoId,
                Status = status,
                ItensAprovados = itensAprovados,
                ValorAprovado = valorAprovado
            };
        }

        private static Pedido GenDefaultPedido(){
            var pedidoId = "1";
            return new Pedido{
                PedidoId = pedidoId,
                Itens = [
                    new Item { Descricao = "Item 1", PrecoUnitario = 10, Qtd = 2, PedidoId = pedidoId }
                ]
            };
        }
    }
}
