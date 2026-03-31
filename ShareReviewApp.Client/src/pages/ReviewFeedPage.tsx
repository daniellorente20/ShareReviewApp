import { useEffect, useState, useMemo } from 'react'
import type { Review, Product } from '../types'
import { getAllReviews } from '../api/reviews'
import { getProducts } from '../api/products'
import ReviewCard from '../components/ReviewCard'
import QuickReviewModal from '../components/QuickReviewModal'
import ProductModal from '../components/ProductModal'

type SortBy = 'date-desc' | 'rating-desc' | 'rating-asc'

export default function ReviewFeedPage() {
  const [reviews, setReviews] = useState<Review[]>([])
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [category, setCategory] = useState('All')
  const [sortBy, setSortBy] = useState<SortBy>('date-desc')
  const [showDropdown, setShowDropdown] = useState(false)
  const [quickReviewOpen, setQuickReviewOpen] = useState(false)
  const [quickReviewProductId, setQuickReviewProductId] = useState<string | undefined>()
  const [productModal, setProductModal] = useState<Product | null>(null)

  function load() {
    Promise.all([getAllReviews(), getProducts()])
      .then(([r, p]) => { setReviews(r); setProducts(p) })
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const categories = useMemo(() => {
    const cats = Array.from(new Set(products.map(p => p.category))).sort()
    return ['All', ...cats]
  }, [products])

  const filteredProducts = useMemo(() =>
    search.trim()
      ? products.filter(p => p.name?.toLowerCase().includes(search.toLowerCase()))
      : [],
    [products, search]
  )

  const filteredReviews = useMemo(() => {
    let result = reviews
    if (category !== 'All') {
      result = result.filter(r => r.productCategory === category)
    }
    if (search.trim()) {
      const q = search.toLowerCase()
      result = result.filter(r =>
        r.productName?.toLowerCase().includes(q) ||
        r.comment?.toLowerCase().includes(q)
      )
    }
    return [...result].sort((a, b) => {
      if (sortBy === 'rating-desc') return b.rating - a.rating
      if (sortBy === 'rating-asc') return a.rating - b.rating
      return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    })
  }, [reviews, category, search, sortBy])

  function openProductModal(productId: string) {
    const p = products.find(x => x.id === productId)
    if (p) setProductModal(p)
  }

  function openQuickReview(productId?: string) {
    setQuickReviewProductId(productId)
    setQuickReviewOpen(true)
  }

  return (
    <div className="flex flex-col gap-6">
      {/* Top bar */}
      <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-3">
        <button
          type="button"
          onClick={() => openQuickReview()}
          className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors cursor-pointer whitespace-nowrap"
        >
          + Write a Review
        </button>

        {/* Search */}
        <div className="relative flex-1">
          <input
            type="text"
            value={search}
            onChange={e => { setSearch(e.target.value); setShowDropdown(true) }}
            onFocus={() => setShowDropdown(true)}
            onBlur={() => setTimeout(() => setShowDropdown(false), 150)}
            placeholder="Search software..."
            className="w-full border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-400"
          />
          {showDropdown && filteredProducts.length > 0 && (
            <ul className="absolute top-full left-0 right-0 mt-1 bg-white border border-gray-200 rounded-lg shadow-lg z-20 max-h-48 overflow-y-auto">
              {filteredProducts.map(p => (
                <li key={p.id}>
                  <button
                    type="button"
                    onMouseDown={() => {
                      setProductModal(p)
                      setSearch(p.name)
                      setShowDropdown(false)
                    }}
                    className="w-full text-left px-4 py-2 text-sm hover:bg-indigo-50 flex items-center justify-between cursor-pointer"
                  >
                    <span className="font-medium text-gray-800">{p.name}</span>
                    <span className="text-xs text-indigo-500">{p.category}</span>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Sort */}
        <select
          value={sortBy}
          onChange={e => setSortBy(e.target.value as SortBy)}
          className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-400 bg-white cursor-pointer"
        >
          <option value="date-desc">Más recientes</option>
          <option value="rating-desc">Mayor rating</option>
          <option value="rating-asc">Menor rating</option>
        </select>
      </div>

      {/* Category chips */}
      <div className="flex items-center gap-2 flex-wrap">
        {categories.map(cat => (
          <button
            key={cat}
            type="button"
            onClick={() => setCategory(cat)}
            className={`text-sm px-3 py-1 rounded-full border transition-colors cursor-pointer ${
              category === cat
                ? 'bg-indigo-600 text-white border-indigo-600'
                : 'bg-white text-gray-600 border-gray-300 hover:border-indigo-400 hover:text-indigo-600'
            }`}
          >
            {cat}
          </button>
        ))}
      </div>

      {/* Feed */}
      {loading && <p className="text-sm text-gray-400">Cargando reseñas…</p>}
      {!loading && filteredReviews.length === 0 && (
        <p className="text-sm text-gray-400">No hay reseñas para mostrar.</p>
      )}
      <div className="flex flex-col gap-3">
        {filteredReviews.map(r => (
          <ReviewCard key={r.id} review={r} onProductClick={openProductModal} />
        ))}
      </div>

      {/* Modals */}
      {quickReviewOpen && (
        <QuickReviewModal
          products={products}
          initialProductId={quickReviewProductId}
          onClose={() => setQuickReviewOpen(false)}
          onSuccess={load}
        />
      )}
      {productModal && (
        <ProductModal
          product={productModal}
          onClose={() => setProductModal(null)}
          onLeaveReview={(pid) => openQuickReview(pid)}
        />
      )}
    </div>
  )
}
