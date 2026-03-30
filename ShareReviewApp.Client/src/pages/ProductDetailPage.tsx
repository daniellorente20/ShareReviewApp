import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import type { Product, Review } from '../types'
import { getProduct } from '../api/products'
import { getReviewsByProduct } from '../api/reviews'
import ReviewCard from '../components/ReviewCard'
import ReviewForm from '../components/ReviewForm'

function averageRating(reviews: Review[]): string {
  if (reviews.length === 0) return '—'
  const avg = reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length
  return avg.toFixed(1)
}

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()

  const [product, setProduct] = useState<Product | null>(null)
  const [reviews, setReviews] = useState<Review[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  async function fetchData() {
    if (!id) return
    setLoading(true)
    setError(null)
    try {
      const [productData, reviewsData] = await Promise.all([
        getProduct(id),
        getReviewsByProduct(id),
      ])
      setProduct(productData)
      setReviews(reviewsData)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar el producto')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchData()
  }, [id])

  async function refetchReviews() {
    if (!id) return
    try {
      const data = await getReviewsByProduct(id)
      setReviews(data)
    } catch {
      // silently ignore refetch errors
    }
  }

  if (loading) {
    return (
      <div className="flex flex-col gap-4 animate-pulse">
        <div className="h-6 bg-gray-200 rounded w-1/3" />
        <div className="h-4 bg-gray-100 rounded w-2/3" />
        <div className="h-4 bg-gray-100 rounded w-1/2" />
      </div>
    )
  }

  if (error || !product) {
    return (
      <div className="flex flex-col gap-4">
        <button onClick={() => navigate('/')} className="text-indigo-600 text-sm hover:underline self-start">
          ← Volver a productos
        </button>
        <p className="text-red-600 bg-red-50 border border-red-200 rounded-lg px-4 py-3 text-sm">
          {error ?? 'Producto no encontrado'}
        </p>
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-8">
      {/* Back */}
      <button onClick={() => navigate('/')} className="text-indigo-600 text-sm hover:underline self-start">
        ← Volver a productos
      </button>

      {/* Product header */}
      <div className="bg-white border border-gray-200 rounded-xl p-6 flex flex-col gap-2 shadow-sm">
        <div className="flex items-start justify-between gap-3">
          <h1 className="text-2xl font-bold text-gray-900">{product.name}</h1>
          <span className="text-xs font-medium bg-indigo-50 text-indigo-600 px-2 py-1 rounded-full shrink-0">
            {product.category}
          </span>
        </div>
        {product.description && (
          <p className="text-sm text-gray-500">{product.description}</p>
        )}
        <div className="flex items-center gap-2 mt-1">
          <span className="text-2xl font-bold text-amber-400">{averageRating(reviews)}</span>
          {reviews.length > 0 && (
            <span className="text-sm text-gray-400">/ 10 · {reviews.length} reseña{reviews.length !== 1 ? 's' : ''}</span>
          )}
          {reviews.length === 0 && (
            <span className="text-sm text-gray-400">Sin reseñas todavía</span>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Reviews list */}
        <div className="flex flex-col gap-4">
          <h2 className="text-base font-semibold text-gray-900">
            Reseñas ({reviews.length})
          </h2>
          {reviews.length === 0 ? (
            <p className="text-sm text-gray-400 py-4">Sé el primero en dejar una reseña.</p>
          ) : (
            <div className="flex flex-col gap-3">
              {reviews.map(review => (
                <ReviewCard key={review.id} review={review} />
              ))}
            </div>
          )}
        </div>

        {/* Review form */}
        <div className="flex flex-col gap-4">
          <h2 className="text-base font-semibold text-gray-900">Dejar una reseña</h2>
          <div className="bg-white border border-gray-200 rounded-xl p-5 shadow-sm">
            <ReviewForm productId={product.id} onSuccess={refetchReviews} />
          </div>
        </div>
      </div>
    </div>
  )
}
