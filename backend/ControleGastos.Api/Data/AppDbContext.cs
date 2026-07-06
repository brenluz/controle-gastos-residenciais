using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Data;

/// <summary>
/// Contexto do Entity Framework Core responsável pelo mapeamento das entidades
/// para o banco de dados SQLite e pelo acesso aos dados.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Pessoas cadastradas.</summary>
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();

    /// <summary>Transações (receitas/despesas) cadastradas.</summary>
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    /// <summary>
    /// Configuração explícita do modelo (restrições, precisão e relacionamentos).
    /// Deixar isso explícito melhora a legibilidade e evita depender de convenções implícitas.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>(pessoa =>
        {
            pessoa.HasKey(p => p.Id);
            pessoa.Property(p => p.Nome).IsRequired().HasMaxLength(150);
            pessoa.Property(p => p.Idade).IsRequired();

            // Relacionamento 1:N Pessoa -> Transacoes.
            // DeleteBehavior.Cascade garante a regra de negócio: ao excluir uma
            // pessoa, todas as suas transações são apagadas automaticamente.
            pessoa.HasMany(p => p.Transacoes)
                  .WithOne(t => t.Pessoa)
                  .HasForeignKey(t => t.PessoaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transacao>(transacao =>
        {
            transacao.HasKey(t => t.Id);
            transacao.Property(t => t.Descricao).IsRequired().HasMaxLength(250);
            // Precisão adequada para valores monetários.
            transacao.Property(t => t.Valor).HasColumnType("decimal(18,2)");
            transacao.Property(t => t.Tipo).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
