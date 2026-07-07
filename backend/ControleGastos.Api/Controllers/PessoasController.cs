using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Endpoints de gerenciamento de pessoas: criação, listagem e exclusão.
/// A lógica de negócio e o acesso a dados ficam no <see cref="IPessoaService"/>;
/// o controller apenas traduz o resultado para HTTP. Ao excluir uma pessoa, suas
/// transações são removidas em cascata (comportamento configurado no <see cref="AppDbContext"/>).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoas;

    public PessoasController(IPessoaService pessoas) => _pessoas = pessoas;

    /// <summary>Lista todas as pessoas cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaResponse>>> Listar() =>
        Ok(await _pessoas.ListarAsync());

    /// <summary>Cadastra uma nova pessoa. O identificador é gerado automaticamente.</summary>
    [HttpPost]
    public async Task<ActionResult<PessoaResponse>> Criar(CriarPessoaRequest request)
    {
        var pessoa = await _pessoas.CriarAsync(request);

        // 201 Created com o cabeçalho Location apontando para a listagem.
        return CreatedAtAction(nameof(Listar), new { id = pessoa.Id }, pessoa);
    }

    /// <summary>
    /// Exclui a pessoa informada. Todas as transações vinculadas são apagadas
    /// automaticamente por cascata.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var removida = await _pessoas.ExcluirAsync(id);
        if (!removida)
            return NotFound(new { mensagem = "Pessoa não encontrada." });

        return NoContent();
    }
}
