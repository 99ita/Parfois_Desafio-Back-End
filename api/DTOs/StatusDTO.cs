namespace api.DTOs{
    public class StatusRequestDto{
        public required string Status { get; set; }
        public required int ItensAprovados { get; set; }
        public required decimal ValorAprovado { get; set; }
        public required string Pedido { get; set; }
    }

    public class StatusResponseDto{
        public required string Pedido { get; set; }
        public required List<string> Status { get; set; }
    }
}