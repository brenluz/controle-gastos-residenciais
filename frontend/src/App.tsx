import { useState } from 'react'
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
 * O estado "recolhida" só afeta o layout a partir de telas médias (md); em
 * telas estreitas a barra vira um topo horizontal.
 */
function App() {
  const [aba, setAba] = useState<Aba>('pessoas')
  const [recolhida, setRecolhida] = useState(false)

  return (
    <div
      className={`flex min-h-screen flex-col md:grid ${
        recolhida ? 'md:grid-cols-[62px_1fr]' : 'md:grid-cols-[248px_1fr]'
      }`}
    >
      <aside className="flex flex-wrap items-center gap-x-4 gap-y-2 border-b border-line bg-surface px-5 py-3.5 md:sticky md:top-0 md:h-screen md:flex-col md:flex-nowrap md:items-stretch md:gap-6 md:overflow-hidden md:border-b-0 md:border-r md:px-3.5 md:py-5">
        <div
          className={`flex items-center gap-2.5 md:px-1 ${
            recolhida ? 'md:justify-center' : ''
          }`}
        >
          <button
            className="hidden size-[34px] shrink-0 cursor-pointer place-items-center rounded-field border border-line bg-surface transition-colors hover:border-line-strong hover:bg-sunken md:grid"
            onClick={() => setRecolhida((v) => !v)}
            aria-label={recolhida ? 'Expandir menu' : 'Recolher menu'}
            aria-expanded={!recolhida}
            title={recolhida ? 'Expandir menu' : 'Recolher menu'}
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 16 16"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.6"
              strokeLinecap="round"
              className="text-ink-soft"
              aria-hidden="true"
            >
              <line x1="2.5" y1="4" x2="13.5" y2="4" />
              <line x1="2.5" y1="8" x2="13.5" y2="8" />
              <line x1="2.5" y1="12" x2="13.5" y2="12" />
            </svg>
          </button>
          <span
            className={`whitespace-nowrap font-display text-[1.14rem] font-semibold leading-tight text-brand ${
              recolhida ? 'md:hidden' : ''
            }`}
          >
            Gastos residenciais
          </span>
        </div>

        <nav className="ml-auto flex flex-wrap gap-x-1 md:ml-0 md:flex-col md:flex-nowrap md:gap-0.5">
          {ABAS.map(({ id, rotulo }) => {
            const ativa = aba === id
            return (
              <button
                key={id}
                className={`flex items-center gap-2.5 rounded-field px-3 py-2.5 text-left text-[0.95rem] transition-colors ${
                  ativa
                    ? 'bg-brand-tint font-semibold text-brand-ink'
                    : 'text-ink-soft hover:bg-sunken hover:text-ink'
                } ${recolhida ? 'md:justify-center md:px-0' : ''}`}
                onClick={() => setAba(id)}
                aria-current={ativa ? 'page' : undefined}
                aria-label={rotulo}
                title={rotulo}
              >
                <span
                  className={`size-[7px] shrink-0 rounded-full bg-current ${
                    ativa ? 'opacity-100' : 'opacity-55'
                  }`}
                  aria-hidden="true"
                />
                <span className={recolhida ? 'md:hidden' : ''}>{rotulo}</span>
              </button>
            )
          })}
        </nav>
      </aside>

      <main className="min-w-0 px-5 py-7 md:px-12 md:py-11">
        {aba === 'pessoas' && <PessoasPage />}
        {aba === 'transacoes' && <TransacoesPage />}
        {aba === 'totais' && <TotaisPage />}
      </main>
    </div>
  )
}

export default App
