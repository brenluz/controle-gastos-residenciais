using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Services;

/// <inheritdoc cref="IPessoaService"/>
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _db;

    public PessoaService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PessoaResponse>> ListarAsync() =>
        await _db.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => PessoaResponse.FromEntity(p))
            .ToListAsync();

    public async Task<PessoaResponse?> ObterPorIdAsync(Guid id)
    {
        var pessoa = await _db.Pessoas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return pessoa is null ? null : PessoaResponse.FromEntity(pessoa);
    }

    public async Task<PessoaResponse> CriarAsync(CriarPessoaRequest request)
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Idade = request.Idade
        };

        // Gravação dentro de uma transação explícita de banco.
        await using var transacaoBd = await _db.Database.BeginTransactionAsync();
        _db.Pessoas.Add(pessoa);
        await _db.SaveChangesAsync();
        await transacaoBd.CommitAsync();

        return PessoaResponse.FromEntity(pessoa);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var pessoa = await _db.Pessoas.FindAsync(id);
        if (pessoa is null)
            return false;

        // Exclusão dentro de uma transação explícita: a remoção da pessoa e das
        // suas transações (cascata) é confirmada como uma única unidade atômica.
        await using var transacaoBd = await _db.Database.BeginTransactionAsync();
        _db.Pessoas.Remove(pessoa);
        await _db.SaveChangesAsync();
        await transacaoBd.CommitAsync();

        return true;
    }
}
