using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Endpoints de gerenciamento de pessoas: criação, listagem e exclusão.
/// Ao excluir uma pessoa, suas transações são removidas em cascata
/// (comportamento configurado no <see cref="AppDbContext"/>).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly AppDbContext _db;

    public PessoasController(AppDbContext db) => _db = db;

    /// <summary>Lista todas as pessoas cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaResponse>>> Listar()
    {
        var pessoas = await _db.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => PessoaResponse.FromEntity(p))
            .ToListAsync();

        return Ok(pessoas);
    }

    /// <summary>Cadastra uma nova pessoa. O identificador é gerado automaticamente.</summary>
    [HttpPost]
    public async Task<ActionResult<PessoaResponse>> Criar(CriarPessoaRequest request)
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Idade = request.Idade
        };

        // Gravação dentro de uma transação explícita de banco.
        await using var transacaoBd = await _db.Database.BeginTransactionAsync();
        _db.Pessoas.Add(pessoa);
        await _db.SaveChangesAsync();
        await transacaoBd.CommitAsync();

        // 201 Created com o cabeçalho Location apontando para a listagem.
        return CreatedAtAction(nameof(Listar), new { id = pessoa.Id }, PessoaResponse.FromEntity(pessoa));
    }

    /// <summary>
    /// Exclui a pessoa informada. Todas as transações vinculadas são apagadas
    /// automaticamente por cascata.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var pessoa = await _db.Pessoas.FindAsync(id);
        if (pessoa is null)
            return NotFound(new { mensagem = "Pessoa não encontrada." });

        // Exclusão dentro de uma transação explícita: a remoção da pessoa e das
        // suas transações (cascata) é confirmada como uma única unidade atômica.
        await using var transacaoBd = await _db.Database.BeginTransactionAsync();
        _db.Pessoas.Remove(pessoa);
        await _db.SaveChangesAsync();
        await transacaoBd.CommitAsync();

        return NoContent();
    }
}
