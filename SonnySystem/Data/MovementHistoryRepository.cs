using Dapper;
using SonnySystem.Models;
using Microsoft.Data.Sqlite;

namespace SonnySystem.Data
{
    public class MovementHistoryRepository
    {
        public void Add(MovementHistory history)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute(@"
                    INSERT INTO MovementHistory (Data, ClienteId, ProdutoId, Quantidade, Tipo, Valor) 
                    VALUES (@Data, @ClienteId, @ProdutoId, @Quantidade, @Tipo, @Valor)", 
                    history);
            }
        }
    }
}
