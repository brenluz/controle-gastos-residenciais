using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Controllers;

/// <summary>
/// Consulta de totais: lista todas as pessoas com seus totais de receitas,
/// despesas e saldo, além do total geral consolidado de todas elas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TotaisController : ControllerBase
{
    private readonly AppDbContext _db;

    public TotaisController(AppDbContext db) => _db = db;

    /// <summary>Retorna os totais por pessoa e o total geral.</summary>
    [HttpGet]
    public async Task<ActionResult<TotaisResponse>> Consultar()
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
        var resposta = new TotaisResponse
        {
            Pessoas = totaisPorPessoa,
            TotalReceitas = totaisPorPessoa.Sum(p => p.TotalReceitas),
            TotalDespesas = totaisPorPessoa.Sum(p => p.TotalDespesas)
        };

        return Ok(resposta);
    }
}
