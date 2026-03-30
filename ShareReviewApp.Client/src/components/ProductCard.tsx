import { useNavigate } from 'react-router-dom'
import type { Product } from '../types'

interface Props {
  product: Product
}

export default function ProductCard({ product }: Props) {
  const navigate = useNavigate()

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-5 flex flex-col gap-3 shadow-sm hover:shadow-md transition-shadow">
      <div className="flex items-start justify-between gap-2">
        <h2 className="text-base font-semibold text-gray-900 leading-snug">{product.name}</h2>
        <span className="text-xs font-medium bg-indigo-50 text-indigo-600 px-2 py-0.5 rounded-full whitespace-nowrap shrink-0">
          {product.category}
        </span>
      </div>
      <p className="text-sm text-gray-500 line-clamp-2 flex-1">{product.description || '—'}</p>
      <button
        onClick={() => navigate(`/products/${product.id}`)}
        className="mt-1 w-full bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium py-2 rounded-lg transition-colors cursor-pointer"
      >
        Ver reseñas
      </button>
    </div>
  )
}
