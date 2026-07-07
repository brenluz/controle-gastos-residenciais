using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Services;

/// <inheritdoc cref="ITransacaoService"/>
public class TransacaoService : ITransacaoService
{
    /// <summary>Idade mínima (em anos) para ser considerado maior de idade.</summary>
    private const int IdadeMaioridade = 18;

    private readonly AppDbContext _db;

    public TransacaoService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TransacaoResponse>> ListarAsync() =>
        await _db.Transacoes
            .AsNoTracking()
            // Ordem determinística (não há data de lançamento no modelo): por pessoa
            // e, dentro dela, por descrição. A projeção já traz o nome da pessoa.
            .OrderBy(t => t.Pessoa!.Nome)
            .ThenBy(t => t.Descricao)
            .Select(ProjecaoResposta)
            .ToListAsync();

    public async Task<TransacaoResponse?> ObterPorIdAsync(Guid id) =>
        await _db.Transacoes
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(ProjecaoResposta)
            .FirstOrDefaultAsync();

    /// <summary>
    /// Projeção da transação para o DTO de resposta. Como acessa a navegação
    /// <c>Pessoa</c>, é traduzida pelo EF Core para um JOIN no SQL (o nome da
    /// pessoa vem na mesma consulta, sem carregar a entidade inteira).
    /// </summary>
    private static readonly System.Linq.Expressions.Expression<Func<Transacao, TransacaoResponse>> ProjecaoResposta =
        t => new TransacaoResponse
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            Tipo = t.Tipo,
            PessoaId = t.PessoaId,
            PessoaNome = t.Pessoa!.Nome
        };

    public async Task<OperationResult<TransacaoResponse>> CriarAsync(CriarTransacaoRequest request)
    {
        // A verificação das regras (pessoa existe / idade) e a gravação são feitas
        // dentro de uma transação explícita de banco, formando uma única unidade
        // atômica: ou tudo é confirmado, ou nada é gravado (rollback). Se qualquer
        // regra falhar, o "await using" descarta a transação sem commit.
        await using var transacaoBd = await _db.Database.BeginTransactionAsync();

        // Regra: a pessoa precisa existir no cadastro.
        var pessoa = await _db.Pessoas.FindAsync(request.PessoaId);
        if (pessoa is null)
            return OperationResult<TransacaoResponse>.Falha("Pessoa não encontrada para a transação informada.");

        // Regra: menores de idade só podem cadastrar despesas.
        if (pessoa.Idade < IdadeMaioridade && request.Tipo == TipoTransacao.Receita)
            return OperationResult<TransacaoResponse>.Falha("Pessoas menores de 18 anos só podem cadastrar despesas.");

        var transacao = new Transacao
        {
            Id = Guid.NewGuid(),
            Descricao = request.Descricao.Trim(),
            Valor = request.Valor,
            // O tipo é obrigatório e validado pelo DTO; aqui já temos um valor garantido.
            Tipo = request.Tipo!.Value,
            PessoaId = request.PessoaId
        };

        _db.Transacoes.Add(transacao);
        await _db.SaveChangesAsync();
        await transacaoBd.CommitAsync();

        // A navegação Pessoa não foi carregada (entidade montada à mão), então
        // preenchemos o nome a partir da pessoa já consultada acima.
        var resposta = TransacaoResponse.FromEntity(transacao);
        resposta.PessoaNome = pessoa.Nome;
        return OperationResult<TransacaoResponse>.Ok(resposta);
    }
}
