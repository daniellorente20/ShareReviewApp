import type { Review } from '../types'
import { SEEDED_USERS } from '../types'
import StarRating from './StarRating'

interface Props {
  review: Review
}

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString('es-ES', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

export default function ReviewCard({ review }: Props) {
  const user = SEEDED_USERS.find(u => u.id === review.userId)
  const userName = user?.name ?? 'Usuario desconocido'

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4 flex flex-col gap-2">
      <div className="flex items-center justify-between gap-2">
        <span className="text-sm font-medium text-gray-800">{userName}</span>
        <span className="text-xs text-gray-400">{formatDate(review.createdAt)}</span>
      </div>
      <StarRating rating={review.rating} />
      <p className="text-sm text-gray-600">{review.comment}</p>
    </div>
  )
}
