import { useEffect, useState } from 'react'
import { totaisApi } from '../api'
import { formatarMoeda } from '../format'
import type { Totais } from '../types'

/**
 * Página de consulta de totais: exibe, para cada pessoa, o total de receitas,
 * despesas e o saldo; ao final, o total geral consolidado de todas as pessoas.
 */
export function TotaisPage() {
  const [totais, setTotais] = useState<Totais | null>(null)
  const [erro, setErro] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)

  useEffect(() => {
    carregar()
  }, [])

  async function carregar() {
    setCarregando(true)
    setErro(null)
    try {
      setTotais(await totaisApi.consultar())
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  /** Classe para colorir o saldo conforme positivo/negativo. */
  function classeSaldo(valor: number): string {
    return valor < 0 ? 'saldo-negativo' : 'saldo-positivo'
  }

  return (
    <section>
      <h2>Totais</h2>

      {erro && <p className="erro">{erro}</p>}

      {carregando ? (
        <p>Carregando...</p>
      ) : !totais || totais.pessoas.length === 0 ? (
        <p>Nenhuma pessoa cadastrada.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Pessoa</th>
              <th>Receitas</th>
              <th>Despesas</th>
              <th>Saldo</th>
            </tr>
          </thead>
          <tbody>
            {totais.pessoas.map((pessoa) => (
              <tr key={pessoa.pessoaId}>
                <td>{pessoa.nome}</td>
                <td>{formatarMoeda(pessoa.totalReceitas)}</td>
                <td>{formatarMoeda(pessoa.totalDespesas)}</td>
                <td className={classeSaldo(pessoa.saldo)}>{formatarMoeda(pessoa.saldo)}</td>
              </tr>
            ))}
          </tbody>
          <tfoot>
            {/* Total geral de todas as pessoas (receitas, despesas e saldo líquido). */}
            <tr className="linha-total">
              <td>Total geral</td>
              <td>{formatarMoeda(totais.totalReceitas)}</td>
              <td>{formatarMoeda(totais.totalDespesas)}</td>
              <td className={classeSaldo(totais.saldoLiquido)}>
                {formatarMoeda(totais.saldoLiquido)}
              </td>
            </tr>
          </tfoot>
        </table>
      )}
    </section>
  )
}
