import { useState } from 'react'
import './App.css'
import { PessoasPage } from './pages/PessoasPage'
import { TransacoesPage } from './pages/TransacoesPage'
import { TotaisPage } from './pages/TotaisPage'

// Abas disponíveis na navegação. Um router seria overkill para 3 telas simples,
// então usamos estado local para alternar entre elas.
type Aba = 'pessoas' | 'transacoes' | 'totais'

/**
 * Componente raiz: cabeçalho, navegação por abas e a página ativa.
 */
function App() {
  const [aba, setAba] = useState<Aba>('pessoas')

  return (
    <div className="container">
      <header>
        <h1>Controle de Gastos Residenciais</h1>
        <nav className="abas">
          <button
            className={aba === 'pessoas' ? 'aba-ativa' : ''}
            onClick={() => setAba('pessoas')}
          >
            Pessoas
          </button>
          <button
            className={aba === 'transacoes' ? 'aba-ativa' : ''}
            onClick={() => setAba('transacoes')}
          >
            Transações
          </button>
          <button
            className={aba === 'totais' ? 'aba-ativa' : ''}
            onClick={() => setAba('totais')}
          >
            Totais
          </button>
        </nav>
      </header>

      <main>
        {aba === 'pessoas' && <PessoasPage />}
        {aba === 'transacoes' && <TransacoesPage />}
        {aba === 'totais' && <TotaisPage />}
      </main>
    </div>
  )
}

export default App
