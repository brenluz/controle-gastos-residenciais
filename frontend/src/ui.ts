// Conjuntos de classes Tailwind reutilizados pelas páginas, centralizados aqui
// para manter a consistência visual e evitar repetição das mesmas sequências.
export const ui = {
  // Estrutura da página
  page: 'flex max-w-[760px] flex-col gap-[18px]',
  pageTitle: 'text-[1.9rem]',
  pageSub: 'mt-1.5 text-[0.95rem] text-ink-soft',

  // Cartão
  card: 'rounded-card border border-line bg-surface p-[22px] shadow-card',
  cardLabel: 'mb-3.5 text-[0.72rem] uppercase tracking-[0.12em] text-muted',

  // Formulário
  form: 'flex flex-wrap items-center gap-2.5',
  field:
    'rounded-field border border-line-strong bg-surface px-3 py-2.5 text-[0.95rem] text-ink transition focus:border-brand focus:outline-none focus:ring-[3px] focus:ring-brand-tint',

  // Botões
  btnPrimary:
    'cursor-pointer rounded-field bg-brand px-[18px] py-2.5 text-[0.92rem] font-semibold text-white transition-colors hover:bg-brand-ink disabled:cursor-not-allowed disabled:opacity-60',
  btnGhost:
    'cursor-pointer rounded-field bg-transparent px-2.5 py-1.5 text-[0.85rem] font-semibold text-despesa transition-colors hover:bg-despesa-tint',

  // Selos de tipo
  badge: 'inline-block rounded-full px-2.5 py-0.5 text-[0.78rem] font-semibold',
  badgeReceita: 'bg-receita-tint text-receita',
  badgeDespesa: 'bg-despesa-tint text-despesa',

  // Tabela (registro)
  tableWrap: 'overflow-x-auto',
  table: 'w-full min-w-[440px] border-collapse text-[0.94rem]',
  th: 'border-b border-line px-3 pb-2.5 text-left text-[0.72rem] font-semibold uppercase tracking-[0.08em] text-muted',
  tbody: '[&>tr:last-child>td]:border-b-0',
  tr: 'transition-colors hover:bg-sunken',
  td: 'border-b border-line px-3 py-[13px]',
  tdAcao: 'border-b border-line px-3 py-[13px] w-px whitespace-nowrap text-right',
  totalCell:
    'border-t-[3px] border-double border-line-strong px-3 py-[15px] font-semibold',

  // Estados
  empty: 'py-2 text-muted',
  alertError:
    'mb-0 rounded-field border border-[#ecccc5] bg-despesa-tint px-3.5 py-2.5 text-[0.9rem] text-despesa',
} as const
