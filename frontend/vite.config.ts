import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    // Durante o desenvolvimento, o front chama "/api/..." e o Vite encaminha
    // as requisições para a API .NET, evitando problemas de CORS.
    proxy: {
      '/api': {
        target: 'http://localhost:5285',
        changeOrigin: true,
      },
    },
  },
})
