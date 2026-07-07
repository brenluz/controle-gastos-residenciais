using ControleGastos.Api.DTOs;
using ControleGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Endpoints de gerenciamento de transações: criação e listagem.
/// As regras de negócio ficam no <see cref="ITransacaoService"/>.
/// (Edição e exclusão não são requisitos deste cadastro.)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _transacoes;

    public TransacoesController(ITransacaoService transacoes) => _transacoes = transacoes;

    /// <summary>Lista todas as transações cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoResponse>>> Listar() =>
        Ok(await _transacoes.ListarAsync());

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
        var resultado = await _transacoes.CriarAsync(request);
        if (!resultado.Sucesso)
            return BadRequest(new { mensagem = resultado.Erro });

        return CreatedAtAction(nameof(Listar), new { id = resultado.Valor!.Id }, resultado.Valor);
    }
}
