using ControleGastos.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Tests;

/// <summary>
/// Cria um <see cref="AppDbContext"/> apoiado por um banco SQLite em memória.
///
/// Usamos SQLite (e não o provedor InMemory do EF) de propósito: assim os testes
/// exercitam o mesmo comportamento do banco real — incluindo exclusão em cascata
/// e as limitações de agregação sobre decimal. A conexão é mantida aberta enquanto
/// a instância existir, pois o banco em memória some quando a conexão é fechada.
/// </summary>
public sealed class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using var context = CriarContexto();
        context.Database.EnsureCreated();
    }

    /// <summary>Cria um novo contexto compartilhando a mesma conexão/banco.</summary>
    public AppDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new AppDbContext(options);
    }

    public void Dispose() => _connection.Dispose();
}
