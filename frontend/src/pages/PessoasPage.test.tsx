import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { PessoasPage } from './PessoasPage'
import { pessoasApi } from '../api'

// O componente é testado isoladamente da rede: mockamos o cliente de API.
vi.mock('../api', () => ({
  pessoasApi: {
    listar: vi.fn(),
    criar: vi.fn(),
    excluir: vi.fn(),
  },
}))

const api = vi.mocked(pessoasApi)

describe('PessoasPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    api.listar.mockResolvedValue([])
    api.criar.mockResolvedValue({ id: '1', nome: 'Ana', idade: 20 })
  })

  it('lista as pessoas retornadas pela API', async () => {
    api.listar.mockResolvedValue([{ id: '1', nome: 'Ana', idade: 20 }])

    render(<PessoasPage />)

    expect(await screen.findByText('Ana')).toBeInTheDocument()
  })

  it('envia o nome sem espaços e a idade como número ao criar', async () => {
    render(<PessoasPage />)

    await userEvent.type(screen.getByLabelText('Nome'), '  Bruno  ')
    await userEvent.type(screen.getByLabelText('Idade'), '25')
    await userEvent.click(screen.getByRole('button', { name: /adicionar/i }))

    await waitFor(() => expect(api.criar).toHaveBeenCalledWith({ nome: 'Bruno', idade: 25 }))
  })

  it('exibe a mensagem de erro quando a criação falha', async () => {
    api.criar.mockRejectedValue(new Error('Falha ao criar.'))

    render(<PessoasPage />)

    await userEvent.type(screen.getByLabelText('Nome'), 'Ana')
    await userEvent.type(screen.getByLabelText('Idade'), '20')
    await userEvent.click(screen.getByRole('button', { name: /adicionar/i }))

    expect(await screen.findByText('Falha ao criar.')).toBeInTheDocument()
  })
})
