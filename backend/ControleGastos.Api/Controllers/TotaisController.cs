using ControleGastos.Api.DTOs;
using ControleGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Consulta de totais: lista todas as pessoas com seus totais de receitas,
/// despesas e saldo, além do total geral consolidado de todas elas.
/// O cálculo fica no <see cref="ITotaisService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TotaisController : ControllerBase
{
    private readonly ITotaisService _totais;

    public TotaisController(ITotaisService totais) => _totais = totais;

    /// <summary>Retorna os totais por pessoa e o total geral.</summary>
    [HttpGet]
    public async Task<ActionResult<TotaisResponse>> Consultar() =>
        Ok(await _totais.ConsultarAsync());
}
