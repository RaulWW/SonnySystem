using Dapper;
using SonnySystem.Models;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using SonnySystem.ViewModels;

namespace SonnySystem.Data
{
    public class ClientProductRepository
    {
        // Get all products for a client, effectively doing a LEFT JOIN to see what they have
        // Actually, the requirement is: "Show ALL products. If link doesn't exist, Qty=0".
        // This is best done via a specific DTO or a query that returns the combined view.
        // For the repository, we can just expose CRUD for the link.
        
        public ClientProduct Get(int clienteId, int produtoId)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                return db.QueryFirstOrDefault<ClientProduct>(
                    "SELECT * FROM ClientProduct WHERE ClienteId = @ClienteId AND ProdutoId = @ProdutoId", 
                    new { ClienteId = clienteId, ProdutoId = produtoId })!;
            }
        }

        public void Save(ClientProduct cp)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                // Upsert logic
                var existing = Get(cp.ClienteId, cp.ProdutoId);
                if (existing == null)
                {
                    db.Execute("INSERT INTO ClientProduct (ClienteId, ProdutoId, QuantidadeNoCliente) VALUES (@ClienteId, @ProdutoId, @QuantidadeNoCliente)", cp);
                }
                else
                {
                     db.Execute("UPDATE ClientProduct SET QuantidadeNoCliente = @QuantidadeNoCliente WHERE ClienteId = @ClienteId AND ProdutoId = @ProdutoId", cp);
                }
            }
        }
        
        // Helper to get the "Grid" view
        public IEnumerable<ConsignmentItem> GetConsignmentItems(int clientId)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                // Return All Active Products Left Joined with ClientProduct
                var sql = @"
                    SELECT 
                        p.Id as ProductId,
                        p.CodigoCurto,
                        p.Descricao,
                        p.PrecoVenda,
                        IFNULL(cp.QuantidadeNoCliente, 0) as QtdAtualLa
                    FROM Product p
                    LEFT JOIN ClientProduct cp ON p.Id = cp.ProdutoId AND cp.ClienteId = @ClientId
                    WHERE p.IsActive = 1
                ";
                return db.Query<ConsignmentItem>(sql, new { ClientId = clientId });
            }
        }
    }
}
