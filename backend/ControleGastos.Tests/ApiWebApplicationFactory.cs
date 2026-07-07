using ControleGastos.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ControleGastos.Tests;

/// <summary>
/// Sobe a API em um servidor de teste (TestServer), trocando o SQLite em
/// arquivo por um SQLite em memória isolado. Assim os testes de integração
/// exercitam o pipeline HTTP completo — roteamento, model binding, validação
/// dos DTOs e serialização — sem depender de um banco externo.
/// </summary>
public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // A conexão fica aberta durante toda a vida da fábrica: o banco em
        // memória do SQLite existe enquanto houver uma conexão aberta.
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Remove o DbContext configurado com o SQLite em arquivo (Program.cs)
            // e o substitui pelo mesmo AppDbContext apoiado na conexão em memória.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}
