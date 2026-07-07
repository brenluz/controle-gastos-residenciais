import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'

// Configuração dos testes (Vitest). Usa jsdom para simular o DOM e um arquivo
// de setup que adiciona os matchers do @testing-library/jest-dom.
export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
  },
})
