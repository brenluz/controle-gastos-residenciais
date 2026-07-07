// Adiciona matchers como toBeInTheDocument/toHaveTextContent ao expect do Vitest
// e limpa o DOM renderizado entre os testes.
import '@testing-library/jest-dom/vitest'
import { afterEach } from 'vitest'
import { cleanup } from '@testing-library/react'

afterEach(() => {
  cleanup()
})
