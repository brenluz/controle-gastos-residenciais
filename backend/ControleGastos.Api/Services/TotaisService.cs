using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Services;

/// <inheritdoc cref="ITotaisService"/>
public class TotaisService : ITotaisService
{
    private readonly AppDbContext _db;

    public TotaisService(AppDbContext db) => _db = db;

    public async Task<TotaisResponse> ConsultarAsync()
    {
        // Carrega cada pessoa com o tipo/valor de suas transações. Projetamos apenas
        // as colunas necessárias para não trazer dados desnecessários do banco.
        // Observação: o SQLite não suporta Sum sobre decimal em SQL, então a soma é
        // feita em memória (LINQ to Objects), o que também preserva a precisão monetária.
        var pessoas = await _db.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new
            {
                p.Id,
                p.Nome,
                Transacoes = p.Transacoes.Select(t => new { t.Tipo, t.Valor })
            })
            .ToListAsync();

        // Totais por pessoa. Pessoas sem transações aparecem com totais zerados.
        var totaisPorPessoa = pessoas
            .Select(p => new TotalPessoaResponse
            {
                PessoaId = p.Id,
                Nome = p.Nome,
                TotalReceitas = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor),
                TotalDespesas = p.Transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor)
            })
            .ToList();

        // Total geral: soma dos totais individuais já calculados.
        return new TotaisResponse
        {
            Pessoas = totaisPorPessoa,
            TotalReceitas = totaisPorPessoa.Sum(p => p.TotalReceitas),
            TotalDespesas = totaisPorPessoa.Sum(p => p.TotalDespesas)
        };
    }
}
