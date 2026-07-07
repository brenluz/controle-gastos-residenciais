import { useEffect, useState } from 'react'
import { pessoasApi } from '../api'
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
    <section className="page">
      <div className="page__head">
        <h2 className="page__title">Pessoas</h2>
        <p className="page__sub">
          Quem faz parte da casa. Ao remover alguém, as transações dessa pessoa
          também são apagadas.
        </p>
      </div>

      <div className="card">
        <p className="card__label">Nova pessoa</p>
        <form onSubmit={criarPessoa} className="form">
          <input
            type="text"
            placeholder="Nome"
            aria-label="Nome"
            value={nome}
            onChange={(e) => setNome(e.target.value)}
            required
          />
          <input
            type="number"
            className="num"
            placeholder="Idade"
            aria-label="Idade"
            min={0}
            max={130}
            value={idade}
            onChange={(e) => setIdade(e.target.value)}
            required
          />
          <button type="submit" className="btn btn--primary">
            Adicionar
          </button>
        </form>
      </div>

      {erro && <p className="alert alert--error">{erro}</p>}

      <div className="card">
        {carregando ? (
          <p className="empty">Carregando...</p>
        ) : pessoas.length === 0 ? (
          <p className="empty">Nenhuma pessoa cadastrada ainda.</p>
        ) : (
          <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Nome</th>
                <th className="right">Idade</th>
                <th className="col-acao" aria-label="Ações"></th>
              </tr>
            </thead>
            <tbody>
              {pessoas.map((pessoa) => (
                <tr key={pessoa.id}>
                  <td>{pessoa.nome}</td>
                  <td className="right num">{pessoa.idade}</td>
                  <td className="col-acao">
                    <button
                      type="button"
                      className="btn btn--ghost"
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
