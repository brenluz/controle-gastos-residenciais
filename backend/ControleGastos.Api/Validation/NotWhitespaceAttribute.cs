using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Api.Validation;

/// <summary>
/// Valida que uma string não é composta apenas por espaços em branco.
/// Complementa o <c>[Required]</c>, que considera <c>"   "</c> como preenchido.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) =>
        // null fica a cargo do [Required]; aqui reprovamos apenas texto em branco.
        value is not string texto || !string.IsNullOrWhiteSpace(texto);
}
