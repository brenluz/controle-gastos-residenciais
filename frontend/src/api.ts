// Cliente HTTP da aplicação. Centraliza as chamadas à API .NET para que os
// componentes não precisem lidar com detalhes de fetch/serialização.
import type {
  NovaPessoa,
  NovaTransacao,
  Pessoa,
  Totais,
  Transacao,
} from './types'

// Todas as rotas passam por "/api", encaminhado ao back-end pelo proxy do Vite.
const BASE_URL = '/api'

/**
 * Wrapper em torno do fetch que:
 *  - envia/recebe JSON;
 *  - em caso de erro, extrai a mensagem de negócio retornada pela API
 *    (ex.: "Pessoas menores de 18 anos só podem cadastrar despesas.").
 */
async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })

  if (!response.ok) {
    throw new Error(await extrairMensagemDeErro(response))
  }

  // 204 No Content (ex.: exclusão) não possui corpo.
  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

/** Tenta obter uma mensagem legível a partir da resposta de erro da API. */
async function extrairMensagemDeErro(response: Response): Promise<string> {
  try {
    const corpo = await response.json()
    // A API usa { mensagem } para erros de negócio e { errors } para validação.
    if (typeof corpo?.mensagem === 'string') return corpo.mensagem
    if (corpo?.errors) {
      const mensagens = Object.values(corpo.errors).flat() as string[]
      if (mensagens.length > 0) return mensagens.join(' ')
    }
  } catch {
    // Corpo não é JSON; usa a mensagem padrão abaixo.
  }
  return `Erro ${response.status} ao comunicar com o servidor.`
}

// ---------------------------------------------------------------------------
// Pessoas
// ---------------------------------------------------------------------------
export const pessoasApi = {
  listar: () => request<Pessoa[]>('/pessoas'),

  criar: (pessoa: NovaPessoa) =>
    request<Pessoa>('/pessoas', {
      method: 'POST',
      body: JSON.stringify(pessoa),
    }),

  excluir: (id: string) =>
    request<void>(`/pessoas/${id}`, { method: 'DELETE' }),
}

// ---------------------------------------------------------------------------
// Transações
// ---------------------------------------------------------------------------
export const transacoesApi = {
  listar: () => request<Transacao[]>('/transacoes'),

  criar: (transacao: NovaTransacao) =>
    request<Transacao>('/transacoes', {
      method: 'POST',
      body: JSON.stringify(transacao),
    }),
}

// ---------------------------------------------------------------------------
// Totais
// ---------------------------------------------------------------------------
export const totaisApi = {
  consultar: () => request<Totais>('/totais'),
}
