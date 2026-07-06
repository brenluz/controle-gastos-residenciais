namespace ControleGastos.Api.DTOs;

/// <summary>
/// Resultado da consulta de totais: os totais por pessoa e o total geral
/// (somando todas as pessoas), conforme exigido pela funcionalidade de consulta.
/// </summary>
public class TotaisResponse
{
    /// <summary>Totais individuais de cada pessoa cadastrada.</summary>
    public IReadOnlyList<TotalPessoaResponse> Pessoas { get; set; } = new List<TotalPessoaResponse>();

    /// <summary>Total geral de receitas (todas as pessoas).</summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>Total geral de despesas (todas as pessoas).</summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>Saldo líquido geral (total de receitas menos total de despesas).</summary>
    public decimal SaldoLiquido => TotalReceitas - TotalDespesas;
}
