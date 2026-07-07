import { useEffect, useState } from 'react'
import { totaisApi } from '../api'
import { formatarMoeda } from '../format'
import { ui } from '../ui'
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

  /** Classe de cor para o saldo conforme positivo/negativo. */
  function corSaldo(valor: number): string {
    return valor < 0 ? 'text-despesa' : 'text-receita'
  }

  return (
    <section className={ui.page}>
      <div>
        <h2 className={ui.pageTitle}>Totais</h2>
        <p className={ui.pageSub}>
          Receitas, despesas e saldo de cada pessoa — e o resultado geral da casa.
        </p>
      </div>

      {erro && <p className={ui.alertError}>{erro}</p>}

      <div className={ui.card}>
        {carregando ? (
          <p className={ui.empty}>Carregando...</p>
        ) : !totais || totais.pessoas.length === 0 ? (
          <p className={ui.empty}>Nenhuma pessoa cadastrada ainda.</p>
        ) : (
          <div className={ui.tableWrap}>
            <table className={ui.table}>
              <thead>
                <tr>
                  <th className={ui.th}>Pessoa</th>
                  <th className={`${ui.th} text-right`}>Receitas</th>
                  <th className={`${ui.th} text-right`}>Despesas</th>
                  <th className={`${ui.th} text-right`}>Saldo</th>
                </tr>
              </thead>
              <tbody className={ui.tbody}>
                {totais.pessoas.map((pessoa) => (
                  <tr key={pessoa.pessoaId} className={ui.tr}>
                    <td className={ui.td}>{pessoa.nome}</td>
                    <td className={`${ui.td} num text-right`}>
                      {formatarMoeda(pessoa.totalReceitas)}
                    </td>
                    <td className={`${ui.td} num text-right`}>
                      {formatarMoeda(pessoa.totalDespesas)}
                    </td>
                    <td className={`${ui.td} num text-right ${corSaldo(pessoa.saldo)}`}>
                      {formatarMoeda(pessoa.saldo)}
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                {/* Total geral de todas as pessoas (receitas, despesas e saldo). */}
                <tr>
                  <td className={`${ui.totalCell} font-display`}>Total geral</td>
                  <td className={`${ui.totalCell} num text-right`}>
                    {formatarMoeda(totais.totalReceitas)}
                  </td>
                  <td className={`${ui.totalCell} num text-right`}>
                    {formatarMoeda(totais.totalDespesas)}
                  </td>
                  <td
                    className={`${ui.totalCell} num text-right ${corSaldo(
                      totais.saldoLiquido,
                    )}`}
                  >
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
