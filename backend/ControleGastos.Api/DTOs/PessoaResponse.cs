using ControleGastos.Api.Models;

namespace ControleGastos.Api.DTOs;

/// <summary>
/// Representação de uma pessoa retornada pela API.
/// Usamos um DTO (em vez da entidade) para não expor a coleção de navegação
/// e manter o contrato da API estável e enxuto.
/// </summary>
public class PessoaResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }

    /// <summary>Cria a resposta a partir da entidade de domínio.</summary>
    public static PessoaResponse FromEntity(Pessoa pessoa) => new()
    {
        Id = pessoa.Id,
        Nome = pessoa.Nome,
        Idade = pessoa.Idade
    };
}
