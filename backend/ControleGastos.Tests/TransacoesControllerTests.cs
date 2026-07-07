using ControleGastos.Api.Controllers;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using ControleGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ControleGastos.Tests;

/// <summary>
/// Testes das regras de negócio do cadastro de transações:
/// a pessoa precisa existir e menores de idade só podem cadastrar despesas.
/// </summary>
public class TransacoesControllerTests
{
    /// <summary>Cadastra uma pessoa e devolve seu Id, para uso nos testes.</summary>
    private static async Task<Guid> SemearPessoaAsync(TestDbContextFactory factory, int idade)
    {
        var id = Guid.NewGuid();
        using var seed = factory.CriarContexto();
        seed.Pessoas.Add(new Pessoa { Id = id, Nome = "Pessoa", Idade = idade });
        await seed.SaveChangesAsync();
        return id;
    }

    [Fact]
    public async Task Criar_ParaAdulto_Receita_DeveSerCriada()
    {
        using var factory = new TestDbContextFactory();
        var adultoId = await SemearPessoaAsync(factory, idade: 30);

        using var contexto = factory.CriarContexto();
        var resultado = await new TransacoesController(new TransacaoService(contexto)).Criar(new CriarTransacaoRequest
        {
            Descricao = "Salário",
            Valor = 5000m,
            Tipo = TipoTransacao.Receita,
            PessoaId = adultoId
        });

        var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        var transacao = Assert.IsType<TransacaoResponse>(created.Value);
        Assert.Equal(TipoTransacao.Receita, transacao.Tipo);
        Assert.NotEqual(Guid.Empty, transacao.Id);
    }

    [Fact]
    public async Task Criar_ParaMenor_Receita_DeveRetornarBadRequest()
    {
        using var factory = new TestDbContextFactory();
        var menorId = await SemearPessoaAsync(factory, idade: 16);

        using var contexto = factory.CriarContexto();
        var resultado = await new TransacoesController(new TransacaoService(contexto)).Criar(new CriarTransacaoRequest
        {
            Descricao = "Mesada",
            Valor = 100m,
            Tipo = TipoTransacao.Receita,
            PessoaId = menorId
        });

        // Regra: menor de idade não pode cadastrar receita.
        Assert.IsType<BadRequestObjectResult>(resultado.Result);

        using var verificacao = factory.CriarContexto();
        Assert.Equal(0, await verificacao.Transacoes.CountAsync());
    }

    [Fact]
    public async Task Criar_ParaMenor_Despesa_DeveSerCriada()
    {
        using var factory = new TestDbContextFactory();
        var menorId = await SemearPessoaAsync(factory, idade: 16);

        using var contexto = factory.CriarContexto();
        var resultado = await new TransacoesController(new TransacaoService(contexto)).Criar(new CriarTransacaoRequest
        {
            Descricao = "Lanche",
            Valor = 20m,
            Tipo = TipoTransacao.Despesa,
            PessoaId = menorId
        });

        // Despesa é permitida para menores.
        Assert.IsType<CreatedAtActionResult>(resultado.Result);
    }

    [Fact]
    public async Task Criar_ParaPessoaCom17Anos_Receita_DeveRetornarBadRequest()
    {
        // Fronteira da regra (< 18): 17 anos ainda é menor.
        using var factory = new TestDbContextFactory();
        var id = await SemearPessoaAsync(factory, idade: 17);

        using var contexto = factory.CriarContexto();
        var resultado = await new TransacoesController(new TransacaoService(contexto)).Criar(new CriarTransacaoRequest
        {
            Descricao = "Mesada",
            Valor = 100m,
            Tipo = TipoTransacao.Receita,
            PessoaId = id
        });

        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task Criar_ParaPessoaCom18Anos_Receita_DeveSerCriada()
    {
        // Fronteira da regra (< 18): 18 anos já é maior de idade.
        using var factory = new TestDbContextFactory();
        var id = await SemearPessoaAsync(factory, idade: 18);

        using var contexto = factory.CriarContexto();
        var resultado = await new TransacoesController(new TransacaoService(contexto)).Criar(new CriarTransacaoRequest
        {
            Descricao = "Salário",
            Valor = 3000m,
            Tipo = TipoTransacao.Receita,
            PessoaId = id
        });

        Assert.IsType<CreatedAtActionResult>(resultado.Result);
    }

    [Fact]
    public async Task Criar_PessoaInexistente_DeveRetornarBadRequest()
    {
        using var factory = new TestDbContextFactory();
        using var contexto = factory.CriarContexto();

        var resultado = await new TransacoesController(new TransacaoService(contexto)).Criar(new CriarTransacaoRequest
        {
            Descricao = "Qualquer",
            Valor = 10m,
            Tipo = TipoTransacao.Despesa,
            PessoaId = Guid.NewGuid() // não existe
        });

        Assert.IsType<BadRequestObjectResult>(resultado.Result);
    }
}
