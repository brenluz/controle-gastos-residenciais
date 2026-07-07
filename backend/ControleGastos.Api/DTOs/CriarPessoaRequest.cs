using System.ComponentModel.DataAnnotations;
using ControleGastos.Api.Validation;

namespace ControleGastos.Api.DTOs;

/// <summary>
/// Dados necessários para cadastrar uma nova pessoa.
/// O identificador não é informado pelo cliente — é gerado automaticamente pelo servidor.
/// As anotações de validação são verificadas automaticamente pelo [ApiController].
/// </summary>
public class CriarPessoaRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [NotWhitespace(ErrorMessage = "O nome não pode ser vazio.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 130, ErrorMessage = "A idade deve estar entre 0 e 130 anos.")]
    public int Idade { get; set; }
}
