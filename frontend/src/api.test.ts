import { describe, it, expect, vi, afterEach } from 'vitest'
import { pessoasApi } from './api'

/** Monta uma Response JSON para simular o retorno do fetch. */
function respostaJson(status: number, body: unknown): Response {
  return new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  })
}

describe('cliente de API', () => {
  afterEach(() => vi.unstubAllGlobals())

  it('retorna os dados quando a resposta é 200', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue(respostaJson(200, [{ id: '1', nome: 'Ana', idade: 20 }])),
    )

    const pessoas = await pessoasApi.listar()

    expect(pessoas).toHaveLength(1)
    expect(pessoas[0].nome).toBe('Ana')
  })

  it('lança o detail do ProblemDetails em erro de negócio', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue(
        respostaJson(400, { title: 'Requisição inválida.', detail: 'Mensagem de negócio.' }),
      ),
    )

    await expect(pessoasApi.criar({ nome: 'Ana', idade: 20 })).rejects.toThrow('Mensagem de negócio.')
  })

  it('junta as mensagens de validação vindas em errors', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue(respostaJson(400, { errors: { Nome: ['O nome é obrigatório.'] } })),
    )

    await expect(pessoasApi.criar({ nome: '', idade: 20 })).rejects.toThrow('O nome é obrigatório.')
  })
})
