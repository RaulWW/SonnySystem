namespace SonnySystem.ViewModels
{
    public class ConsignmentItem
    {
        public int ProductId { get; set; }
        public string CodigoCurto { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public decimal PrecoVenda { get; set; }
        public int QtdAtualLa { get; set; }
        
        public string DisplayName => $"{CodigoCurto} - {Descricao}";
    }
}
