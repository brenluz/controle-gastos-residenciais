namespace ControleGastos.Api.Services;

/// <summary>
/// Resultado de uma operação de serviço que pode falhar por regra de negócio.
/// Evita usar exceções para o fluxo de validação esperado: o serviço devolve
/// o erro como dado e o controller apenas o traduz para a resposta HTTP.
/// </summary>
public class OperationResult<T>
{
    private OperationResult(bool sucesso, T? valor, string? erro)
    {
        Sucesso = sucesso;
        Valor = valor;
        Erro = erro;
    }

    /// <summary>Indica se a operação foi concluída com sucesso.</summary>
    public bool Sucesso { get; }

    /// <summary>Valor produzido pela operação quando bem-sucedida.</summary>
    public T? Valor { get; }

    /// <summary>Mensagem de erro de negócio quando a operação falha.</summary>
    public string? Erro { get; }

    public static OperationResult<T> Ok(T valor) => new(true, valor, null);

    public static OperationResult<T> Falha(string erro) => new(false, default, erro);
}
