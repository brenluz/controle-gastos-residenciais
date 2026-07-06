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
    <section>
      <h2>Pessoas</h2>

      <form onSubmit={criarPessoa} className="form-linha">
        <input
          type="text"
          placeholder="Nome"
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          required
        />
        <input
          type="number"
          placeholder="Idade"
          min={0}
          max={130}
          value={idade}
          onChange={(e) => setIdade(e.target.value)}
          required
        />
        <button type="submit">Adicionar</button>
      </form>

      {erro && <p className="erro">{erro}</p>}

      {carregando ? (
        <p>Carregando...</p>
      ) : pessoas.length === 0 ? (
        <p>Nenhuma pessoa cadastrada.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>Idade</th>
              <th aria-label="Ações"></th>
            </tr>
          </thead>
          <tbody>
            {pessoas.map((pessoa) => (
              <tr key={pessoa.id}>
                <td>{pessoa.nome}</td>
                <td>{pessoa.idade}</td>
                <td>
                  <button
                    type="button"
                    className="botao-perigo"
                    onClick={() => excluirPessoa(pessoa.id, pessoa.nome)}
                  >
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  )
}
