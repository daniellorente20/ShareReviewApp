import { useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { createReview } from '../api/reviews'
import StarPicker from './StarPicker'

interface Props {
  productId: string
  onSuccess: () => void
}

export default function ReviewForm({ productId, onSuccess }: Props) {
  const { currentUser } = useAuth()
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  if (!currentUser) return null

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      await createReview({ userId: currentUser!.id, productId, comment, rating })
      setComment('')
      setRating(5)
      onSuccess()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al enviar la reseña')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <p className="text-xs text-gray-500">
        Publicando como <span className="font-semibold text-gray-700">{currentUser.name}</span>
      </p>

      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-gray-700">Puntuación</label>
        <StarPicker value={rating} onChange={setRating} />
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-gray-700">Comentario</label>
        <textarea
          value={comment}
          onChange={e => setComment(e.target.value)}
          required
          rows={3}
          placeholder="Escribe tu opinión sobre el producto..."
          className="border border-gray-300 rounded-lg px-3 py-2 text-sm resize-none focus:outline-none focus:ring-2 focus:ring-indigo-400"
        />
      </div>

      {error && (
        <p className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-lg px-3 py-2">
          {error}
        </p>
      )}

      <button
        type="submit"
        disabled={loading || !comment.trim()}
        className="bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50 text-white text-sm font-medium py-2 rounded-lg transition-colors cursor-pointer disabled:cursor-not-allowed"
      >
        {loading ? 'Enviando…' : 'Publicar reseña'}
      </button>
    </form>
  )
}
