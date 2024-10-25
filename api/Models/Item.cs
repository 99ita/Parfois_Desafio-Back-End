namespace api.Models{
    public class Item{
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Qtd { get; set; }
        public required string PedidoId { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}