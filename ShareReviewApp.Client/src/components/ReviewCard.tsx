import type { Review } from '../types'
import { SEEDED_USERS } from '../types'
import StarRating from './StarRating'

interface Props {
  review: Review
  onProductClick?: (productId: string) => void
}

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

export default function ReviewCard({ review, onProductClick }: Props) {
  const user = SEEDED_USERS.find(u => u.id === review.userId)
  const userName = user?.name ?? 'Usuario desconocido'

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4 flex flex-col gap-2">
      <div className="flex items-center justify-between gap-2 flex-wrap">
        <div className="flex items-center gap-2">
          {review.productName && (
            <button
              type="button"
              onClick={() => onProductClick?.(review.productId)}
              className="text-sm font-semibold text-indigo-700 hover:underline cursor-pointer"
            >
              {review.productName}
            </button>
          )}
          {review.productCategory && (
            <span className="text-xs bg-indigo-50 text-indigo-500 border border-indigo-100 rounded-full px-2 py-0.5 font-medium">
              {review.productCategory}
            </span>
          )}
        </div>
        <span className="text-xs text-gray-400">{formatDate(review.createdAt)}</span>
      </div>
      <div className="flex items-center gap-2">
        <StarRating rating={review.rating} />
        <span className="text-sm text-gray-500">{userName}</span>
      </div>
      <p className="text-sm text-gray-600">{review.comment}</p>
    </div>
  )
}
