using ControleGastos.Api.Models;

namespace ControleGastos.Api.DTOs;

/// <summary>
/// Representação de uma transação retornada pela API.
/// </summary>
public class TransacaoResponse
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public Guid PessoaId { get; set; }

    /// <summary>Cria a resposta a partir da entidade de domínio.</summary>
    public static TransacaoResponse FromEntity(Transacao transacao) => new()
    {
        Id = transacao.Id,
        Descricao = transacao.Descricao,
        Valor = transacao.Valor,
        Tipo = transacao.Tipo,
        PessoaId = transacao.PessoaId
    };
}
