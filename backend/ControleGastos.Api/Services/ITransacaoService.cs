using ControleGastos.Api.DTOs;

namespace ControleGastos.Api.Services;

/// <summary>
/// Regras e acesso a dados do cadastro de transações. Concentra as regras de
/// negócio (pessoa precisa existir; menores de idade só cadastram despesas).
/// </summary>
public interface ITransacaoService
{
    /// <summary>Lista as transações cadastradas.</summary>
    Task<IReadOnlyList<TransacaoResponse>> ListarAsync();

    /// <summary>Obtém uma transação pelo identificador, ou <c>null</c> se não existir.</summary>
    Task<TransacaoResponse?> ObterPorIdAsync(Guid id);

    /// <summary>
    /// Cria uma transação aplicando as regras de negócio. Em caso de violação,
    /// devolve <see cref="OperationResult{T}.Falha"/> com a mensagem apropriada.
    /// </summary>
    Task<OperationResult<TransacaoResponse>> CriarAsync(CriarTransacaoRequest request);
}
