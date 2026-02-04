namespace SonnySystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string CodigoCurto { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public decimal PrecoVenda { get; set; }
        public int EstoqueMestre { get; set; }
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return $"{CodigoCurto} - {Descricao}";
        }
    }
}
