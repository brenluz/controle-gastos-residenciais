// Utilitários de formatação para exibição.

/** Formata um número como moeda brasileira (ex.: 1234.5 -> "R$ 1.234,50"). */
export function formatarMoeda(valor: number): string {
  return valor.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  })
}
