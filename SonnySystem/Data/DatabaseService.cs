using System.IO;
using Microsoft.Data.Sqlite;
using Dapper;

namespace SonnySystem.Data
{
    public static class DatabaseService
    {
        private static string DbFile = "sonnysystem.db";
        public static string ConnectionString = $"Data Source={DbFile};";

        public static void Initialize()
        {
            if (!File.Exists(DbFile))
            {
                // Microsoft.Data.Sqlite automatically creates the file on first connection open if it doesn't exist,
                // but we keep the logic consistent or ensure the file is clean.
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                // 1. Create Tables
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Product (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CodigoCurto TEXT NOT NULL,
                        Descricao TEXT NOT NULL,
                        PrecoVenda DECIMAL NOT NULL,
                        EstoqueMestre INTEGER NOT NULL,
                        IsActive INTEGER DEFAULT 1
                    );

                    CREATE TABLE IF NOT EXISTS Client (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nome TEXT NOT NULL,
                        Endereco TEXT,
                        Numero TEXT,
                        Bairro TEXT,
                        Cidade TEXT,
                        CEP TEXT,
                        ComissaoPercentual DECIMAL NOT NULL
                    );
                    
                    /* New Consignment Tables */
                     CREATE TABLE IF NOT EXISTS ClientProduct (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClienteId INTEGER NOT NULL,
                        ProdutoId INTEGER NOT NULL,
                        QuantidadeNoCliente INTEGER DEFAULT 0,
                        FOREIGN KEY(ClienteId) REFERENCES Client(Id),
                        FOREIGN KEY(ProdutoId) REFERENCES Product(Id),
                        UNIQUE(ClienteId, ProdutoId)
                    );

                    CREATE TABLE IF NOT EXISTS MovementHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Data TEXT NOT NULL,
                        ClienteId INTEGER NOT NULL,
                        ProdutoId INTEGER NOT NULL,
                        Quantidade INTEGER NOT NULL,
                        Tipo TEXT NOT NULL,
                        Valor DECIMAL NOT NULL,
                        FOREIGN KEY(ClienteId) REFERENCES Client(Id),
                        FOREIGN KEY(ProdutoId) REFERENCES Product(Id)
                    );
                ");

                // 2. Migrations (Safe Execute)
                SafeExecute(connection, "ALTER TABLE Client ADD COLUMN Numero TEXT;");
                SafeExecute(connection, "ALTER TABLE Client ADD COLUMN Bairro TEXT;");
                SafeExecute(connection, "ALTER TABLE Client ADD COLUMN CEP TEXT;");
                // SafeExecute(connection, "ALTER TABLE Product ADD COLUMN EstoqueMestre INTEGER DEFAULT 0;");
            }
        }
        
        private static void SafeExecute(SqliteConnection db, string sql)
        {
            try
            {
                db.Execute(sql);
            }
            catch
            {
                // Ignore if column exists
            }
        }
    }
}
