using Dapper;
using SonnySystem.Models;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace SonnySystem.Data
{
    public class ProductRepository
    {
        public List<Product> GetAll()
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                return db.Query<Product>("SELECT * FROM Product").ToList();
            }
        }

        public List<Product> GetActive()
        {
             using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                return db.Query<Product>("SELECT * FROM Product WHERE IsActive = 1").ToList();
            }
        }

        public void Add(Product product)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute("INSERT INTO Product (CodigoCurto, Descricao, PrecoVenda, EstoqueMestre, IsActive) VALUES (@CodigoCurto, @Descricao, @PrecoVenda, @EstoqueMestre, @IsActive)", product);
            }
        }

        public void Update(Product product)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute("UPDATE Product SET CodigoCurto = @CodigoCurto, Descricao = @Descricao, PrecoVenda = @PrecoVenda, EstoqueMestre = @EstoqueMestre, IsActive = @IsActive WHERE Id = @Id", product);
            }
        }

        public void DeductStock(int productId, int quantity)
        {
            using (var db = new SqliteConnection(DatabaseService.ConnectionString))
            {
                db.Execute("UPDATE Product SET EstoqueMestre = EstoqueMestre - @Quantity WHERE Id = @Id", new { Quantity = quantity, Id = productId });
            }
        }
    }
}
