import './App.css'
import { PessoasPage } from './pages/PessoasPage'

/**
 * Componente raiz da aplicação. Por enquanto exibe o cabeçalho e a página de
 * Pessoas; as páginas de Transações e Totais serão adicionadas na navegação.
 */
function App() {
  return (
    <div className="container">
      <header>
        <h1>Controle de Gastos Residenciais</h1>
      </header>
      <main>
        <PessoasPage />
      </main>
    </div>
  )
}

export default App
