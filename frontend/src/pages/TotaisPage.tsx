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
    return valor < 0 ? 'neg' : 'pos'
  }

  return (
    <section className="page">
      <div className="page__head">
        <h2 className="page__title">Totais</h2>
        <p className="page__sub">
          Receitas, despesas e saldo de cada pessoa — e o resultado geral da casa.
        </p>
      </div>

      {erro && <p className="alert alert--error">{erro}</p>}

      <div className="card">
        {carregando ? (
          <p className="empty">Carregando...</p>
        ) : !totais || totais.pessoas.length === 0 ? (
          <p className="empty">Nenhuma pessoa cadastrada ainda.</p>
        ) : (
          <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Pessoa</th>
                <th className="right">Receitas</th>
                <th className="right">Despesas</th>
                <th className="right">Saldo</th>
              </tr>
            </thead>
            <tbody>
              {totais.pessoas.map((pessoa) => (
                <tr key={pessoa.pessoaId}>
                  <td>{pessoa.nome}</td>
                  <td className="right num">{formatarMoeda(pessoa.totalReceitas)}</td>
                  <td className="right num">{formatarMoeda(pessoa.totalDespesas)}</td>
                  <td className={`right num ${classeSaldo(pessoa.saldo)}`}>
                    {formatarMoeda(pessoa.saldo)}
                  </td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              {/* Total geral de todas as pessoas (receitas, despesas e saldo líquido). */}
              <tr>
                <td className="rotulo-total">Total geral</td>
                <td className="right num">{formatarMoeda(totais.totalReceitas)}</td>
                <td className="right num">{formatarMoeda(totais.totalDespesas)}</td>
                <td className={`right num ${classeSaldo(totais.saldoLiquido)}`}>
                  {formatarMoeda(totais.saldoLiquido)}
                </td>
              </tr>
            </tfoot>
          </table>
          </div>
        )}
      </div>
    </section>
  )
}
