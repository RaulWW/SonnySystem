namespace SonnySystem.ViewModels
{
    public class ReplenishmentViewModel
    {
        public int ProductId { get; set; }
        public string DisplayName { get; set; } = default!;
        public decimal UnitPrice { get; set; }
        public int IdealStock { get; set; }
        public int MasterStock { get; set; }
        public int RepositionQty { get; set; }
        public decimal TotalValue => RepositionQty * UnitPrice;
    }
}
