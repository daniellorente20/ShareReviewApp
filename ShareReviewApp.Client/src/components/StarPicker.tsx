import { useState } from 'react'

interface Props {
  value: number
  onChange: (v: number) => void
}

export default function StarPicker({ value, onChange }: Props) {
  const [hovered, setHovered] = useState<number | null>(null)

  return (
    <div className="flex items-center gap-1">
      {Array.from({ length: 10 }, (_, i) => i + 1).map(n => {
        const active = n <= (hovered ?? value)
        return (
          <button
            key={n}
            type="button"
            className={`text-2xl transition-colors cursor-pointer ${active ? 'text-amber-400' : 'text-gray-300'}`}
            onMouseEnter={() => setHovered(n)}
            onMouseLeave={() => setHovered(null)}
            onClick={() => onChange(n)}
            aria-label={`${n} stars`}
          >
            ★
          </button>
        )
      })}
      <span className="ml-2 text-sm text-gray-500">{hovered ?? value} / 10</span>
    </div>
  )
}
