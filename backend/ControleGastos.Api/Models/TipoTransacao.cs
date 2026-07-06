namespace ControleGastos.Api.Models;

/// <summary>
/// Tipo de uma transação financeira.
/// Uma transação é sempre uma entrada (receita) ou uma saída (despesa) de dinheiro.
/// </summary>
public enum TipoTransacao
{
    /// <summary>Saída de dinheiro (gasto).</summary>
    Despesa = 0,

    /// <summary>Entrada de dinheiro (ganho).</summary>
    Receita = 1
}
