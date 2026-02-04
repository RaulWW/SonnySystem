using System;

namespace SonnySystem.Models
{
    public class MovementHistory
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int ClienteId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string Tipo { get; set; } = default!; // "Reposicao", "Devolucao", "Venda"
        public decimal Valor { get; set; }
    }
}
