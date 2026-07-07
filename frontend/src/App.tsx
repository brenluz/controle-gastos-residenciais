import { useState } from 'react'
import './App.css'
import { PessoasPage } from './pages/PessoasPage'
import { TransacoesPage } from './pages/TransacoesPage'
import { TotaisPage } from './pages/TotaisPage'

// Abas disponíveis na navegação. Um router seria overkill para 3 telas simples,
// então usamos estado local para alternar entre elas.
type Aba = 'pessoas' | 'transacoes' | 'totais'

const ABAS: { id: Aba; rotulo: string }[] = [
  { id: 'pessoas', rotulo: 'Pessoas' },
  { id: 'transacoes', rotulo: 'Transações' },
  { id: 'totais', rotulo: 'Totais' },
]

/**
 * Componente raiz: barra lateral de navegação (retrátil) + página ativa.
 */
function App() {
  const [aba, setAba] = useState<Aba>('pessoas')
  const [recolhida, setRecolhida] = useState(false)

  return (
    <div className={`app ${recolhida ? 'app--recolhida' : ''}`}>
      <aside className="sidebar">
        <div className="sidebar__top">
          <button
            className="sidebar__toggle"
            onClick={() => setRecolhida((v) => !v)}
            aria-label={recolhida ? 'Expandir menu' : 'Recolher menu'}
            aria-expanded={!recolhida}
            title={recolhida ? 'Expandir menu' : 'Recolher menu'}
          >
            <span className="sidebar__toggle-icon" aria-hidden="true" />
          </button>
          <span className="brand">Gastos residenciais</span>
        </div>

        <nav className="nav">
          {ABAS.map(({ id, rotulo }) => (
            <button
              key={id}
              className={`nav__item ${aba === id ? 'nav__item--active' : ''}`}
              onClick={() => setAba(id)}
              aria-current={aba === id ? 'page' : undefined}
              aria-label={rotulo}
              title={rotulo}
            >
              <span className="nav__dot" aria-hidden="true" />
              <span className="nav__label">{rotulo}</span>
            </button>
          ))}
        </nav>
      </aside>

      <main className="content">
        {aba === 'pessoas' && <PessoasPage />}
        {aba === 'transacoes' && <TransacoesPage />}
        {aba === 'totais' && <TotaisPage />}
      </main>
    </div>
  )
}

export default App
