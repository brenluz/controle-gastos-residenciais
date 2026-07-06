// Tipos compartilhados que espelham os contratos (DTOs) expostos pela API .NET.

/** Tipo de uma transação, alinhado ao enum do back-end. */
export type TipoTransacao = 'Receita' | 'Despesa'

/** Pessoa cadastrada. */
export interface Pessoa {
  id: string
  nome: string
  idade: number
}

/** Dados para cadastrar uma nova pessoa (sem o id, gerado pelo servidor). */
export interface NovaPessoa {
  nome: string
  idade: number
}

/** Transação (receita/despesa) vinculada a uma pessoa. */
export interface Transacao {
  id: string
  descricao: string
  valor: number
  tipo: TipoTransacao
  pessoaId: string
}

/** Dados para cadastrar uma nova transação. */
export interface NovaTransacao {
  descricao: string
  valor: number
  tipo: TipoTransacao
  pessoaId: string
}

/** Totais consolidados de uma pessoa. */
export interface TotalPessoa {
  pessoaId: string
  nome: string
  totalReceitas: number
  totalDespesas: number
  saldo: number
}

/** Resultado da consulta de totais: por pessoa e o total geral. */
export interface Totais {
  pessoas: TotalPessoa[]
  totalReceitas: number
  totalDespesas: number
  saldoLiquido: number
}
