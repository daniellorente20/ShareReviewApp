import { useState } from 'react'
import type { Product } from '../types'
import { useAuth } from '../context/AuthContext'
import { createReview } from '../api/reviews'
import StarPicker from './StarPicker'

interface Props {
  products: Product[]
  initialProductId?: string
  onClose: () => void
  onSuccess: () => void
}

export default function QuickReviewModal({ products, initialProductId, onClose, onSuccess }: Props) {
  const { currentUser } = useAuth()
  const [productId, setProductId] = useState(initialProductId ?? '')
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!currentUser || !productId) return
    setError(null)
    setLoading(true)
    try {
      await createReview({ userId: currentUser.id, productId, comment, rating })
      onSuccess()
      onClose()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al enviar la reseña')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-2xl shadow-xl w-full max-w-md mx-4"
        onClick={e => e.stopPropagation()}
      >
        <div className="p-4 sm:p-6 border-b border-gray-100 flex items-center justify-between">
          <h2 className="text-lg font-bold text-gray-900">Write a Review</h2>
          <button
            type="button"
            onClick={onClose}
            className="text-gray-400 hover:text-gray-700 text-xl leading-none cursor-pointer"
          >
            ✕
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-4 sm:p-6 flex flex-col gap-4">
          <p className="text-xs text-gray-500">
            Publicando como <span className="font-semibold text-gray-700">{currentUser?.name}</span>
          </p>

          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-gray-700">Producto</label>
            <select
              value={productId}
              onChange={e => setProductId(e.target.value)}
              required
              className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-400 bg-white"
            >
              <option value="">Selecciona un producto…</option>
              {products.map(p => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
          </div>

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

          <div className="flex justify-end gap-2 pt-1">
            <button
              type="button"
              onClick={onClose}
              className="text-sm text-gray-500 hover:text-gray-700 px-4 py-2 rounded-lg transition-colors cursor-pointer"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading || !comment.trim() || !productId}
              className="bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors cursor-pointer disabled:cursor-not-allowed"
            >
              {loading ? 'Enviando…' : 'Publicar reseña'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
