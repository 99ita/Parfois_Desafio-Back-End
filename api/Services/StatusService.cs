using api.Repositories;
using api.DTOs;

namespace api.Services{

    public class StatusService {
        private readonly IPedidoRepository _pedidoRepository;

        public StatusService(IPedidoRepository pedidoRepository) {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<StatusResponseDto> AnswerStatusAsync(StatusRequestDto statusRequest) {
            var pedido = await _pedidoRepository.GetByIdAsync(statusRequest.Pedido);
            
            if (pedido == null) {
                return new StatusResponseDto {
                    Pedido = statusRequest.Pedido,
                    Status = new List<string> { "CODIGO_PEDIDO_INVALIDO" }
                };
            }

            var statusList = new List<string>();

            if (statusRequest.Status == "REPROVADO") {
                statusList.Add("REPROVADO");
                return new StatusResponseDto {
                    Pedido = statusRequest.Pedido,
                    Status = statusList
                };
            }

            var totalQuantity = pedido.Itens.Sum(i => i.Qtd);
            var totalValue = pedido.Itens.Sum(i => i.PrecoUnitario * i.Qtd);

            if (statusRequest.Status == "APROVADO"){
                if (statusRequest.ItensAprovados == totalQuantity && statusRequest.ValorAprovado == totalValue) {
                    statusList.Add("APROVADO");
                    return new StatusResponseDto {
                    Pedido = statusRequest.Pedido,
                        Status = statusList
                    };
                }

                if (statusRequest.ValorAprovado < totalValue){
                    statusList.Add("APROVADO_VALOR_A_MENOR");
                } else if (statusRequest.ValorAprovado > totalValue){
                    statusList.Add("APROVADO_VALOR_A_MAIOR");
                }
                if (statusRequest.ItensAprovados < totalQuantity) {
                    statusList.Add("APROVADO_QTD_A_MENOR");
                } else if (statusRequest.ItensAprovados > totalQuantity){
                    statusList.Add("APROVADO_QTD_A_MAIOR");
                }
            }

            return new StatusResponseDto {
                Pedido = statusRequest.Pedido,
                Status = statusList
            };
        }
    }
}