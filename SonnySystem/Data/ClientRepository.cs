using Dapper;
using SonnySystem.Models;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace SonnySystem.Data
{
    public class ClientRepository
    {
        public List<Client> GetAll()
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                return db.Query<Client>("SELECT * FROM Client").ToList();
            }
        }

        public void Add(Client client)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute("INSERT INTO Client (Nome, Endereco, Numero, Bairro, Cidade, CEP, ComissaoPercentual) VALUES (@Nome, @Endereco, @Numero, @Bairro, @Cidade, @CEP, @ComissaoPercentual)", client);
            }
        }

        public void Update(Client client)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute("UPDATE Client SET Nome = @Nome, Endereco = @Endereco, Numero = @Numero, Bairro = @Bairro, Cidade = @Cidade, CEP = @CEP, ComissaoPercentual = @ComissaoPercentual WHERE Id = @Id", client);
            }
        }
    }
}
