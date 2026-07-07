using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Api.Validation;

/// <summary>
/// Valida que um <see cref="Guid"/> foi de fato informado (diferente de
/// <see cref="Guid.Empty"/>). O atributo <c>[Required]</c> não cobre esse caso:
/// como <see cref="Guid"/> é um tipo de valor, ele "sempre tem valor" e um
/// corpo sem o campo chega como <see cref="Guid.Empty"/> passando na validação.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) =>
        // null fica a cargo do [Required]; aqui reprovamos apenas o Guid vazio.
        value is not Guid guid || guid != Guid.Empty;
}
