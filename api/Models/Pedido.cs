namespace api.Models{

    public class Pedido{
        public required string PedidoId { get; set; }
        public required ICollection<Item> Itens { get; set; } = [];
    }
}