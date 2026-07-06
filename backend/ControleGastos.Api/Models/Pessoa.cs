namespace ControleGastos.Api.Models;

/// <summary>
/// Representa uma pessoa cadastrada no sistema de controle de gastos.
/// Cada pessoa pode possuir várias transações (receitas/despesas).
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Identificador único da pessoa, gerado automaticamente na criação.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>Nome da pessoa.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Idade da pessoa (em anos). Usada para aplicar a regra do menor de idade.</summary>
    public int Idade { get; set; }

    /// <summary>
    /// Transações vinculadas à pessoa. Ao excluir a pessoa, todas as suas
    /// transações são removidas em cascata (configurado no DbContext).
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
