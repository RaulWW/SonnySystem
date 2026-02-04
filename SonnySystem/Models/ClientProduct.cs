namespace SonnySystem.Models
{
    public class ClientProduct
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ProdutoId { get; set; }
        public int QuantidadeNoCliente { get; set; }
    }
}
