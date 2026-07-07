import { useEffect, useState } from 'react'
import { pessoasApi, transacoesApi } from '../api'
import { formatarMoeda } from '../format'
import type { Pessoa, TipoTransacao, Transacao } from '../types'

/**
 * Página de cadastro de transações: formulário de criação e listagem.
 * O back-end aplica as regras de negócio (pessoa precisa existir; menores de
 * idade só podem cadastrar despesas) e a mensagem de erro é exibida ao usuário.
 */
export function TransacoesPage() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([])
  const [pessoas, setPessoas] = useState<Pessoa[]>([])
  const [descricao, setDescricao] = useState('')
  const [valor, setValor] = useState('')
  const [tipo, setTipo] = useState<TipoTransacao>('Despesa')
  const [pessoaId, setPessoaId] = useState('')
  const [erro, setErro] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)

  // Carrega transações e pessoas (necessárias para o seletor) ao montar.
  useEffect(() => {
    carregar()
  }, [])

  async function carregar() {
    setCarregando(true)
    setErro(null)
    try {
      const [listaTransacoes, listaPessoas] = await Promise.all([
        transacoesApi.listar(),
        pessoasApi.listar(),
      ])
      setTransacoes(listaTransacoes)
      setPessoas(listaPessoas)
    } catch (e) {
      setErro((e as Error).message)
    } finally {
      setCarregando(false)
    }
  }

  async function criarTransacao(evento: React.FormEvent) {
    evento.preventDefault()
    setErro(null)
    try {
      await transacoesApi.criar({
        descricao: descricao.trim(),
        valor: Number(valor),
        tipo,
        pessoaId,
      })
      // Limpa o formulário e recarrega a listagem.
      setDescricao('')
      setValor('')
      setTipo('Despesa')
      setPessoaId('')
      await carregar()
    } catch (e) {
      // Ex.: "Pessoas menores de 18 anos só podem cadastrar despesas."
      setErro((e as Error).message)
    }
  }

  /** Nome da pessoa dona da transação, para exibição na tabela. */
  function nomeDaPessoa(id: string): string {
    return pessoas.find((p) => p.id === id)?.nome ?? '—'
  }

  return (
    <section className="page">
      <div className="page__head">
        <h2 className="page__title">Transações</h2>
        <p className="page__sub">
          Lançamentos de receitas e despesas. Menores de 18 anos só podem
          registrar despesas.
        </p>
      </div>

      <div className="card">
        <p className="card__label">Novo lançamento</p>
        {pessoas.length === 0 ? (
          <p className="empty">Cadastre uma pessoa antes de lançar transações.</p>
        ) : (
          <form onSubmit={criarTransacao} className="form">
            <input
              type="text"
              placeholder="Descrição"
              aria-label="Descrição"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              required
            />
            <input
              type="number"
              className="num"
              placeholder="Valor"
              aria-label="Valor"
              min="0.01"
              step="0.01"
              value={valor}
              onChange={(e) => setValor(e.target.value)}
              required
            />
            <select
              aria-label="Tipo"
              value={tipo}
              onChange={(e) => setTipo(e.target.value as TipoTransacao)}
            >
              <option value="Despesa">Despesa</option>
              <option value="Receita">Receita</option>
            </select>
            <select
              aria-label="Pessoa"
              value={pessoaId}
              onChange={(e) => setPessoaId(e.target.value)}
              required
            >
              <option value="">Selecione a pessoa</option>
              {pessoas.map((pessoa) => (
                <option key={pessoa.id} value={pessoa.id}>
                  {pessoa.nome}
                </option>
              ))}
            </select>
            <button type="submit" className="btn btn--primary">
              Adicionar
            </button>
          </form>
        )}
      </div>

      {erro && <p className="alert alert--error">{erro}</p>}

      <div className="card">
        {carregando ? (
          <p className="empty">Carregando...</p>
        ) : transacoes.length === 0 ? (
          <p className="empty">Nenhuma transação cadastrada ainda.</p>
        ) : (
          <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Pessoa</th>
                <th>Tipo</th>
                <th className="right">Valor</th>
              </tr>
            </thead>
            <tbody>
              {transacoes.map((transacao) => (
                <tr key={transacao.id}>
                  <td>{transacao.descricao}</td>
                  <td>{nomeDaPessoa(transacao.pessoaId)}</td>
                  <td>
                    <span
                      className={
                        transacao.tipo === 'Receita'
                          ? 'badge badge--receita'
                          : 'badge badge--despesa'
                      }
                    >
                      {transacao.tipo}
                    </span>
                  </td>
                  <td className="right num">{formatarMoeda(transacao.valor)}</td>
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
