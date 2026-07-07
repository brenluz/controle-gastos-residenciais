import { useEffect, useState } from 'react'
import { pessoasApi } from '../api'
import { ui } from '../ui'
import type { Pessoa } from '../types'

/**
 * Página de cadastro de pessoas: formulário de criação, listagem e exclusão.
 * Ao excluir uma pessoa, o back-end remove também as transações dela (cascata).
 */
export function PessoasPage() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([])
  const [nome, setNome] = useState('')
  const [idade, setIdade] = useState('')
  const [erro, setErro] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)

  // Carrega a lista de pessoas ao montar o componente.
  useEffect(() => {
    carregarPessoas()
  }, [])

  async function carregarPessoas() {
    setCarregando(true)
    setErro(null)
    try {
      setPessoas(await pessoasApi.listar())
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  async function criarPessoa(evento: React.FormEvent) {
    evento.preventDefault()
    setErro(null)
    try {
      await pessoasApi.criar({ nome: nome.trim(), idade: Number(idade) })
      // Limpa o formulário e recarrega a lista.
      setNome('')
      setIdade('')
      await carregarPessoas()
    } catch (e) {
      setErro((e as Error).message)
    }
  }

  async function excluirPessoa(id: string, nomePessoa: string) {
    const confirmado = window.confirm(
      `Excluir "${nomePessoa}"? Todas as transações dessa pessoa também serão apagadas.`,
    )
    if (!confirmado) return

    setErro(null)
    try {
      await pessoasApi.excluir(id)
      await carregarPessoas()
    } catch (e) {
      setErro((e as Error).message)
    }
  }

  return (
    <section className={ui.page}>
      <div>
        <h2 className={ui.pageTitle}>Pessoas</h2>
        <p className={ui.pageSub}>
          Quem faz parte da casa. Ao remover alguém, as transações dessa pessoa
          também são apagadas.
        </p>
      </div>

      <div className={ui.card}>
        <p className={ui.cardLabel}>Nova pessoa</p>
        <form onSubmit={criarPessoa} className={ui.form}>
          <input
            type="text"
            placeholder="Nome"
            aria-label="Nome"
            className={`${ui.field} min-w-0 flex-1`}
            value={nome}
            onChange={(e) => setNome(e.target.value)}
            required
          />
          <input
            type="number"
            placeholder="Idade"
            aria-label="Idade"
            className={`${ui.field} num flex-[0_1_120px]`}
            min={0}
            max={130}
            value={idade}
            onChange={(e) => setIdade(e.target.value)}
            required
          />
          <button type="submit" className={ui.btnPrimary}>
            Adicionar
          </button>
        </form>
      </div>

      {erro && <p className={ui.alertError}>{erro}</p>}

      <div className={ui.card}>
        {carregando ? (
          <p className={ui.empty}>Carregando...</p>
        ) : pessoas.length === 0 ? (
          <p className={ui.empty}>Nenhuma pessoa cadastrada ainda.</p>
        ) : (
          <div className={ui.tableWrap}>
            <table className={ui.table}>
              <thead>
                <tr>
                  <th className={ui.th}>Nome</th>
                  <th className={`${ui.th} text-right`}>Idade</th>
                  <th className={ui.th} aria-label="Ações" />
                </tr>
              </thead>
              <tbody className={ui.tbody}>
                {pessoas.map((pessoa) => (
                  <tr key={pessoa.id} className={ui.tr}>
                    <td className={ui.td}>{pessoa.nome}</td>
                    <td className={`${ui.td} num text-right`}>{pessoa.idade}</td>
                    <td className={ui.tdAcao}>
                      <button
                        type="button"
                        className={ui.btnGhost}
                        onClick={() => excluirPessoa(pessoa.id, pessoa.nome)}
                      >
                        Excluir
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </section>
  )
}
