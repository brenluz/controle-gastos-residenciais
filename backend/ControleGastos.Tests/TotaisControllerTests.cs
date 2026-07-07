using ControleGastos.Api.Controllers;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ControleGastos.Tests;

/// <summary>
/// Testes da consulta de totais: totais por pessoa (receitas, despesas, saldo)
/// e o total geral consolidado.
/// </summary>
public class TotaisControllerTests
{
    [Fact]
    public async Task Consultar_DeveCalcularTotaisPorPessoaEGeral()
    {
        using var factory = new TestDbContextFactory();
        var mariaId = Guid.NewGuid();
        var pedroId = Guid.NewGuid();

        using (var seed = factory.CriarContexto())
        {
            seed.Pessoas.AddRange(
                new Pessoa { Id = mariaId, Nome = "Maria", Idade = 40 },
                new Pessoa { Id = pedroId, Nome = "Pedro", Idade = 16 });

            // Maria: receita 5000, despesa 300,50 => saldo 4699,50
            // Pedro: despesa 20 => saldo -20
            seed.Transacoes.AddRange(
                new Transacao { Id = Guid.NewGuid(), Descricao = "Salário", Valor = 5000m, Tipo = TipoTransacao.Receita, PessoaId = mariaId },
                new Transacao { Id = Guid.NewGuid(), Descricao = "Mercado", Valor = 300.50m, Tipo = TipoTransacao.Despesa, PessoaId = mariaId },
                new Transacao { Id = Guid.NewGuid(), Descricao = "Lanche", Valor = 20m, Tipo = TipoTransacao.Despesa, PessoaId = pedroId });
            await seed.SaveChangesAsync();
        }

        using var contexto = factory.CriarContexto();
        var resultado = await new TotaisController(contexto).Consultar();

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var totais = Assert.IsType<TotaisResponse>(ok.Value);

        var maria = totais.Pessoas.Single(p => p.Nome == "Maria");
        Assert.Equal(5000m, maria.TotalReceitas);
        Assert.Equal(300.50m, maria.TotalDespesas);
        Assert.Equal(4699.50m, maria.Saldo);

        var pedro = totais.Pessoas.Single(p => p.Nome == "Pedro");
        Assert.Equal(0m, pedro.TotalReceitas);
        Assert.Equal(20m, pedro.TotalDespesas);
        Assert.Equal(-20m, pedro.Saldo);

        // Total geral consolidado.
        Assert.Equal(5000m, totais.TotalReceitas);
        Assert.Equal(320.50m, totais.TotalDespesas);
        Assert.Equal(4679.50m, totais.SaldoLiquido);
    }

    [Fact]
    public async Task Consultar_PessoaSemTransacoes_DeveRetornarZeros()
    {
        using var factory = new TestDbContextFactory();
        using (var seed = factory.CriarContexto())
        {
            seed.Pessoas.Add(new Pessoa { Id = Guid.NewGuid(), Nome = "Sofia", Idade = 25 });
            await seed.SaveChangesAsync();
        }

        using var contexto = factory.CriarContexto();
        var resultado = await new TotaisController(contexto).Consultar();

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var totais = Assert.IsType<TotaisResponse>(ok.Value);

        var sofia = Assert.Single(totais.Pessoas);
        Assert.Equal(0m, sofia.TotalReceitas);
        Assert.Equal(0m, sofia.TotalDespesas);
        Assert.Equal(0m, sofia.Saldo);
        Assert.Equal(0m, totais.SaldoLiquido);
    }
}
