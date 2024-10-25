namespace api.DTOs{

    public class PedidoDto
    {
        public required string PedidoId { get; set; }
        public required List<ItemDto> Itens { get; set; } = [];
    }

    public class ItemDto
    {
        public required string Descricao { get; set; }
        public required decimal PrecoUnitario { get; set; }
        public required int Qtd { get; set; }
    }
}