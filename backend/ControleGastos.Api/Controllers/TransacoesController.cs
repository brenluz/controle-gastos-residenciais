using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Endpoints de gerenciamento de transações: criação e listagem.
/// (Edição e exclusão não são requisitos deste cadastro.)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    /// <summary>Idade mínima (em anos) para ser considerado maior de idade.</summary>
    private const int IdadeMaioridade = 18;

    private readonly AppDbContext _db;

    public TransacoesController(AppDbContext db) => _db = db;

    /// <summary>Lista todas as transações cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoResponse>>> Listar()
    {
        var transacoes = await _db.Transacoes
            .AsNoTracking()
            .Select(t => TransacaoResponse.FromEntity(t))
            .ToListAsync();

        return Ok(transacoes);
    }

    /// <summary>
    /// Cadastra uma nova transação, aplicando as regras de negócio:
    /// <list type="bullet">
    ///   <item>A pessoa informada precisa existir no cadastro.</item>
    ///   <item>Pessoas menores de 18 anos só podem cadastrar despesas.</item>
    /// </list>
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransacaoResponse>> Criar(CriarTransacaoRequest request)
    {
        // Regra: a pessoa precisa existir no cadastro.
        var pessoa = await _db.Pessoas.FindAsync(request.PessoaId);
        if (pessoa is null)
            return BadRequest(new { mensagem = "Pessoa não encontrada para a transação informada." });

        // Regra: menores de idade só podem cadastrar despesas.
        if (pessoa.Idade < IdadeMaioridade && request.Tipo == TipoTransacao.Receita)
            return BadRequest(new { mensagem = "Pessoas menores de 18 anos só podem cadastrar despesas." });

        var transacao = new Transacao
        {
            Id = Guid.NewGuid(),
            Descricao = request.Descricao,
            Valor = request.Valor,
            // O tipo é obrigatório e validado pelo DTO; aqui já temos um valor garantido.
            Tipo = request.Tipo!.Value,
            PessoaId = request.PessoaId
        };

        _db.Transacoes.Add(transacao);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Listar), new { id = transacao.Id }, TransacaoResponse.FromEntity(transacao));
    }
}
