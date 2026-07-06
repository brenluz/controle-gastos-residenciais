namespace ControleGastos.Api.DTOs;

/// <summary>
/// Totais consolidados de uma pessoa: soma de receitas, soma de despesas e saldo.
/// </summary>
public class TotalPessoaResponse
{
    public Guid PessoaId { get; set; }
    public string Nome { get; set; } = string.Empty;

    /// <summary>Soma de todas as receitas da pessoa.</summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>Soma de todas as despesas da pessoa.</summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>Saldo da pessoa (receitas menos despesas). Calculado a partir dos totais.</summary>
    public decimal Saldo => TotalReceitas - TotalDespesas;
}
