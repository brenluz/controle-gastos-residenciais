using ControleGastos.Api.DTOs;

namespace ControleGastos.Api.Services;

/// <summary>
/// Regras e acesso a dados do cadastro de pessoas. Mantém os controllers finos
/// (apenas HTTP) e concentra a lógica de negócio em um ponto testável.
/// </summary>
public interface IPessoaService
{
    /// <summary>Lista as pessoas cadastradas, ordenadas por nome.</summary>
    Task<IReadOnlyList<PessoaResponse>> ListarAsync();

    /// <summary>Obtém uma pessoa pelo identificador, ou <c>null</c> se não existir.</summary>
    Task<PessoaResponse?> ObterPorIdAsync(Guid id);

    /// <summary>Cria uma nova pessoa e devolve a representação persistida.</summary>
    Task<PessoaResponse> CriarAsync(CriarPessoaRequest request);

    /// <summary>Exclui a pessoa (e suas transações em cascata). Retorna <c>false</c> se não existir.</summary>
    Task<bool> ExcluirAsync(Guid id);
}
