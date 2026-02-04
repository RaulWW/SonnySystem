using System;

namespace SonnySystem.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int ClienteId { get; set; }
        public int ProdutoId { get; set; }
        public int QtdReposita { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
