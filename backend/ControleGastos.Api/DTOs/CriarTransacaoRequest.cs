using System.ComponentModel.DataAnnotations;
using ControleGastos.Api.Models;

namespace ControleGastos.Api.DTOs;

/// <summary>
/// Dados necessários para cadastrar uma nova transação.
/// O identificador é gerado automaticamente pelo servidor.
/// </summary>
public class CriarTransacaoRequest
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(250, ErrorMessage = "A descrição deve ter no máximo 250 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    // Nullable + [Required] força o cliente a informar explicitamente o tipo
    // (evita assumir "Despesa" por ser o valor padrão do enum).
    [Required(ErrorMessage = "O tipo (Despesa/Receita) é obrigatório.")]
    public TipoTransacao? Tipo { get; set; }

    [Required(ErrorMessage = "A pessoa é obrigatória.")]
    public Guid PessoaId { get; set; }
}
