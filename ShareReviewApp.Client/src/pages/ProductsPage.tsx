import { useState, useEffect } from 'react'
import type { Product } from '../types'
import { getProducts } from '../api/products'
import ProductCard from '../components/ProductCard'
import NewProductForm from '../components/NewProductForm'

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [showForm, setShowForm] = useState(false)

  async function fetchProducts() {
    setLoading(true)
    setError(null)
    try {
      const data = await getProducts()
      setProducts(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar los productos')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchProducts()
  }, [])

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Productos</h1>
        <button
          onClick={() => setShowForm(v => !v)}
          className="bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors cursor-pointer"
        >
          {showForm ? 'Cancelar' : '+ Nuevo producto'}
        </button>
      </div>

      {showForm && (
        <NewProductForm
          onSuccess={() => { setShowForm(false); fetchProducts() }}
          onCancel={() => setShowForm(false)}
        />
      )}

      {loading && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {[1, 2, 3].map(i => (
            <div key={i} className="bg-white border border-gray-200 rounded-xl p-5 h-36 animate-pulse">
              <div className="h-4 bg-gray-200 rounded w-2/3 mb-3" />
              <div className="h-3 bg-gray-100 rounded w-full mb-2" />
              <div className="h-3 bg-gray-100 rounded w-4/5" />
            </div>
          ))}
        </div>
      )}

      {error && (
        <p className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-lg px-4 py-3">
          {error}
        </p>
      )}

      {!loading && !error && products.length === 0 && (
        <p className="text-gray-400 text-sm text-center py-12">
          No hay productos todavía. ¡Crea el primero!
        </p>
      )}

      {!loading && products.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {products.map(product => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}
    </div>
  )
}
