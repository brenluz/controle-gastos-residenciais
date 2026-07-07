using ControleGastos.Api.DTOs;

namespace ControleGastos.Api.Services;

/// <summary>
/// Cálculo dos totais consolidados (por pessoa e geral).
/// </summary>
public interface ITotaisService
{
    /// <summary>Retorna os totais por pessoa e o total geral.</summary>
    Task<TotaisResponse> ConsultarAsync();
}
