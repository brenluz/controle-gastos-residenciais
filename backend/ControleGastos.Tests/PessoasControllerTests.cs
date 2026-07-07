using ControleGastos.Api.Controllers;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ControleGastos.Tests;

/// <summary>
/// Testes das regras do cadastro de pessoas, incluindo a exclusão em cascata
/// das transações vinculadas.
/// </summary>
public class PessoasControllerTests
{
    [Fact]
    public async Task Criar_DeveGerarIdEPersistirPessoa()
    {
        using var factory = new TestDbContextFactory();
        using var contexto = factory.CriarContexto();
        var controller = new PessoasController(contexto);

        var resultado = await controller.Criar(new CriarPessoaRequest { Nome = "João", Idade = 30 });

        // Deve retornar 201 Created com a pessoa criada e um Id gerado.
        var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        var pessoa = Assert.IsType<PessoaResponse>(created.Value);
        Assert.NotEqual(Guid.Empty, pessoa.Id);
        Assert.Equal("João", pessoa.Nome);

        // E deve ter sido de fato persistida no banco.
        using var verificacao = factory.CriarContexto();
        Assert.Equal(1, await verificacao.Pessoas.CountAsync());
    }

    [Fact]
    public async Task Listar_DeveRetornarPessoasOrdenadasPorNome()
    {
        using var factory = new TestDbContextFactory();
        using (var seed = factory.CriarContexto())
        {
            seed.Pessoas.AddRange(
                new Pessoa { Id = Guid.NewGuid(), Nome = "Carlos", Idade = 40 },
                new Pessoa { Id = Guid.NewGuid(), Nome = "Ana", Idade = 20 });
            await seed.SaveChangesAsync();
        }

        using var contexto = factory.CriarContexto();
        var resultado = await new PessoasController(contexto).Listar();

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var pessoas = Assert.IsAssignableFrom<IEnumerable<PessoaResponse>>(ok.Value).ToList();
        Assert.Equal(new[] { "Ana", "Carlos" }, pessoas.Select(p => p.Nome));
    }

    [Fact]
    public async Task Excluir_DeveRemoverTransacoesEmCascata()
    {
        using var factory = new TestDbContextFactory();
        var pessoaId = Guid.NewGuid();
        using (var seed = factory.CriarContexto())
        {
            seed.Pessoas.Add(new Pessoa { Id = pessoaId, Nome = "Maria", Idade = 35 });
            seed.Transacoes.Add(new Transacao
            {
                Id = Guid.NewGuid(),
                Descricao = "Salário",
                Valor = 1000m,
                Tipo = TipoTransacao.Receita,
                PessoaId = pessoaId
            });
            await seed.SaveChangesAsync();
        }

        using (var contexto = factory.CriarContexto())
        {
            var resultado = await new PessoasController(contexto).Excluir(pessoaId);
            Assert.IsType<NoContentResult>(resultado);
        }

        // A pessoa e suas transações devem ter sido removidas.
        using var verificacao = factory.CriarContexto();
        Assert.Equal(0, await verificacao.Pessoas.CountAsync());
        Assert.Equal(0, await verificacao.Transacoes.CountAsync());
    }

    [Fact]
    public async Task Excluir_PessoaInexistente_DeveRetornarNotFound()
    {
        using var factory = new TestDbContextFactory();
        using var contexto = factory.CriarContexto();

        var resultado = await new PessoasController(contexto).Excluir(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(resultado);
    }
}
