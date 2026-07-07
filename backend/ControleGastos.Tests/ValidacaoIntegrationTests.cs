using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastos.Tests;

/// <summary>
/// Testes de integração que sobem a API e exercitam a validação de entrada
/// (anotações dos DTOs) através do pipeline HTTP real — algo que os testes de
/// unidade, que chamam os controllers direto, não cobrem. Também validam o
/// caminho feliz de criação, incluindo o Location do 201 e o nome da pessoa.
/// </summary>
public class ValidacaoIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    // A API serializa o enum Tipo como texto ("Receita"/"Despesa"); o cliente
    // precisa do mesmo conversor para desserializar a resposta.
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public ValidacaoIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        // O host (TestServer) é reaproveitado via IClassFixture, mas o construtor
        // roda a cada teste: zeramos os dados aqui para que um teste não enxergue
        // o que outro criou (isolamento equivalente ao dos testes de unidade).
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Transacoes.RemoveRange(db.Transacoes.ToList());
        db.Pessoas.RemoveRange(db.Pessoas.ToList());
        db.SaveChanges();
    }

    [Fact]
    public async Task PostPessoa_SemNome_DeveRetornar400()
    {
        var resposta = await _client.PostAsJsonAsync("/api/pessoas", new { idade = 30 });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPessoa_NomeEmBranco_DeveRetornar400()
    {
        var resposta = await _client.PostAsJsonAsync("/api/pessoas", new { nome = "   ", idade = 30 });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPessoa_IdadeForaDoIntervalo_DeveRetornar400()
    {
        var resposta = await _client.PostAsJsonAsync("/api/pessoas", new { nome = "Ana", idade = 200 });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostTransacao_SemPessoa_DeveRetornar400()
    {
        // PessoaId ausente chega como Guid.Empty e deve ser reprovado na validação.
        var resposta = await _client.PostAsJsonAsync(
            "/api/transacoes",
            new { descricao = "Compra", valor = 10m, tipo = "Despesa" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostTransacao_SemTipo_DeveRetornar400()
    {
        var resposta = await _client.PostAsJsonAsync(
            "/api/transacoes",
            new { descricao = "Compra", valor = 10m, pessoaId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostTransacao_ValorZero_DeveRetornar400()
    {
        var resposta = await _client.PostAsJsonAsync(
            "/api/transacoes",
            new { descricao = "Compra", valor = 0m, tipo = "Despesa", pessoaId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task FluxoCompleto_CriaPessoaETransacao_ComLocationENomeDaPessoa()
    {
        // Cria a pessoa: 201 com Location apontando para o recurso criado.
        var criacaoPessoa = await _client.PostAsJsonAsync("/api/pessoas", new { nome = "Maria", idade = 30 });
        Assert.Equal(HttpStatusCode.Created, criacaoPessoa.StatusCode);
        Assert.NotNull(criacaoPessoa.Headers.Location);

        var pessoa = await criacaoPessoa.Content.ReadFromJsonAsync<PessoaResponse>();
        Assert.NotNull(pessoa);

        // O Location do 201 precisa resolver para o GET do recurso.
        var pessoaObtida = await _client.GetFromJsonAsync<PessoaResponse>(criacaoPessoa.Headers.Location);
        Assert.Equal(pessoa!.Id, pessoaObtida!.Id);

        // Cria a transação e confere que a resposta já traz o nome da pessoa.
        var criacaoTransacao = await _client.PostAsJsonAsync(
            "/api/transacoes",
            new { descricao = "Salário", valor = 5000m, tipo = "Receita", pessoaId = pessoa.Id });
        Assert.Equal(HttpStatusCode.Created, criacaoTransacao.StatusCode);

        var transacao = await criacaoTransacao.Content.ReadFromJsonAsync<TransacaoResponse>(JsonOpts);
        Assert.Equal("Maria", transacao!.PessoaNome);
    }
}
