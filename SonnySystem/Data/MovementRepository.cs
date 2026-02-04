using Dapper;
using SonnySystem.Models;
using Microsoft.Data.Sqlite;

namespace SonnySystem.Data
{
    public class MovementRepository
    {
        public void Add(Movement movement)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute("INSERT INTO Movement (Data, ClienteId, ProdutoId, QtdReposita, ValorTotal) VALUES (@Data, @ClienteId, @ProdutoId, @QtdReposita, @ValorTotal)", movement);
            }
        }
    }
}
