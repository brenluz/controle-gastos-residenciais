namespace ControleGastos.Api.Models;

/// <summary>
/// Representa uma transação financeira (receita ou despesa) pertencente a uma pessoa.
/// </summary>
public class Transacao
{
    /// <summary>
    /// Identificador único da transação, gerado automaticamente na criação.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>Descrição da transação (ex.: "Salário", "Conta de luz").</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Valor monetário da transação. Sempre positivo; o sinal é dado pelo <see cref="Tipo"/>.</summary>
    public decimal Valor { get; set; }

    /// <summary>Indica se a transação é uma receita ou uma despesa.</summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>Identificador da pessoa dona da transação (chave estrangeira).</summary>
    public Guid PessoaId { get; set; }

    /// <summary>Referência de navegação para a pessoa dona da transação.</summary>
    public Pessoa? Pessoa { get; set; }
}
