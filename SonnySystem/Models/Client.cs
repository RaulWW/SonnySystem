namespace SonnySystem.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Nome { get; set; } = default!;
        public string Endereco { get; set; } = default!; // Rua
        public string Numero { get; set; } = default!;
        public string Bairro { get; set; } = default!;
        public string Cidade { get; set; } = default!;
        public string CEP { get; set; } = default!;
        public decimal ComissaoPercentual { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}
