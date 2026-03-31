import { useEffect, useState } from 'react'
import type { Product, Review } from '../types'
import { getReviewsByProduct } from '../api/reviews'
import StarRating from './StarRating'
import ReviewCard from './ReviewCard'

interface Props {
  product: Product
  onClose: () => void
  onLeaveReview: (productId: string) => void
}

export default function ProductModal({ product, onClose, onLeaveReview }: Props) {
  const [reviews, setReviews] = useState<Review[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    getReviewsByProduct(product.id)
      .then(setReviews)
      .finally(() => setLoading(false))
  }, [product.id])

  const avgRating = reviews.length
    ? Math.round((reviews.reduce((s, r) => s + r.rating, 0) / reviews.length) * 10) / 10
    : null

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-2xl shadow-xl w-full max-w-lg mx-4 max-h-[90vh] flex flex-col"
        onClick={e => e.stopPropagation()}
      >
        <div className="p-6 border-b border-gray-100 flex items-start justify-between gap-4">
          <div>
            <div className="flex items-center gap-2 mb-1">
              <h2 className="text-xl font-bold text-gray-900">{product.name}</h2>
              <span className="text-xs bg-indigo-50 text-indigo-500 border border-indigo-100 rounded-full px-2 py-0.5 font-medium">
                {product.category}
              </span>
            </div>
            <p className="text-sm text-gray-500">{product.description}</p>
            {avgRating !== null && (
              <div className="mt-2">
                <StarRating rating={avgRating} />
                <span className="text-xs text-gray-400 ml-1">({reviews.length} reseñas)</span>
              </div>
            )}
          </div>
          <button
            type="button"
            onClick={onClose}
            className="text-gray-400 hover:text-gray-700 text-xl leading-none cursor-pointer"
          >
            ✕
          </button>
        </div>

        <div className="p-6 overflow-y-auto flex-1 flex flex-col gap-3">
          {loading && <p className="text-sm text-gray-400">Cargando reseñas…</p>}
          {!loading && reviews.length === 0 && (
            <p className="text-sm text-gray-400">No hay reseñas todavía. ¡Sé el primero!</p>
          )}
          {reviews.map(r => (
            <ReviewCard key={r.id} review={r} />
          ))}
        </div>

        <div className="p-4 border-t border-gray-100 flex justify-end gap-2">
          <button
            type="button"
            onClick={onClose}
            className="text-sm text-gray-500 hover:text-gray-700 px-4 py-2 rounded-lg transition-colors cursor-pointer"
          >
            Cerrar
          </button>
          <button
            type="button"
            onClick={() => { onClose(); onLeaveReview(product.id) }}
            className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors cursor-pointer"
          >
            Leave a Review
          </button>
        </div>
      </div>
    </div>
  )
}
