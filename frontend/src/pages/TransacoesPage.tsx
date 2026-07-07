import { useEffect, useState } from 'react'
import { pessoasApi, transacoesApi } from '../api'
import { formatarMoeda } from '../format'
import { ui } from '../ui'
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

  return (
    <section className={ui.page}>
      <div>
        <h2 className={ui.pageTitle}>Transações</h2>
        <p className={ui.pageSub}>
          Lançamentos de receitas e despesas. Menores de 18 anos só podem
          registrar despesas.
        </p>
      </div>

      <div className={ui.card}>
        <p className={ui.cardLabel}>Novo lançamento</p>
        {pessoas.length === 0 ? (
          <p className={ui.empty}>Cadastre uma pessoa antes de lançar transações.</p>
        ) : (
          <form onSubmit={criarTransacao} className={ui.form}>
            <input
              type="text"
              placeholder="Descrição"
              aria-label="Descrição"
              className={`${ui.field} min-w-0 flex-1`}
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              required
            />
            <input
              type="number"
              placeholder="Valor"
              aria-label="Valor"
              className={`${ui.field} num flex-[0_1_120px]`}
              min="0.01"
              step="0.01"
              value={valor}
              onChange={(e) => setValor(e.target.value)}
              required
            />
            <select
              aria-label="Tipo"
              className={ui.field}
              value={tipo}
              onChange={(e) => setTipo(e.target.value as TipoTransacao)}
            >
              <option value="Despesa">Despesa</option>
              <option value="Receita">Receita</option>
            </select>
            <select
              aria-label="Pessoa"
              className={ui.field}
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
            <button type="submit" className={ui.btnPrimary}>
              Adicionar
            </button>
          </form>
        )}
      </div>

      {erro && <p className={ui.alertError}>{erro}</p>}

      <div className={ui.card}>
        {carregando ? (
          <p className={ui.empty}>Carregando...</p>
        ) : transacoes.length === 0 ? (
          <p className={ui.empty}>Nenhuma transação cadastrada ainda.</p>
        ) : (
          <div className={ui.tableWrap}>
            <table className={ui.table}>
              <thead>
                <tr>
                  <th className={ui.th}>Descrição</th>
                  <th className={ui.th}>Pessoa</th>
                  <th className={ui.th}>Tipo</th>
                  <th className={`${ui.th} text-right`}>Valor</th>
                </tr>
              </thead>
              <tbody className={ui.tbody}>
                {transacoes.map((transacao) => (
                  <tr key={transacao.id} className={ui.tr}>
                    <td className={ui.td}>{transacao.descricao}</td>
                    <td className={ui.td}>{transacao.pessoaNome}</td>
                    <td className={ui.td}>
                      <span
                        className={`${ui.badge} ${
                          transacao.tipo === 'Receita'
                            ? ui.badgeReceita
                            : ui.badgeDespesa
                        }`}
                      >
                        {transacao.tipo}
                      </span>
                    </td>
                    <td className={`${ui.td} num text-right`}>
                      {formatarMoeda(transacao.valor)}
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
