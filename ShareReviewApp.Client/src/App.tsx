import { Routes, Route, Link } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import LoginPage from './pages/LoginPage'
import ProductsPage from './pages/ProductsPage'
import ProductDetailPage from './pages/ProductDetailPage'

export default function App() {
  const { currentUser, logout } = useAuth()

  if (!currentUser) {
    return <LoginPage />
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b border-gray-200 sticky top-0 z-10">
        <div className="max-w-5xl mx-auto px-4 h-14 flex items-center justify-between">
          <Link to="/" className="text-xl font-bold text-indigo-600 tracking-tight">
            ReviewShare
          </Link>
          <div className="flex items-center gap-3">
            <div className="flex items-center gap-2">
              <div className="w-7 h-7 rounded-full bg-indigo-500 flex items-center justify-center">
                <span className="text-white text-xs font-semibold">
                  {currentUser.name.charAt(0)}
                </span>
              </div>
              <span className="text-sm font-medium text-gray-700">{currentUser.name}</span>
            </div>
            <button
              onClick={logout}
              className="text-xs text-gray-400 hover:text-gray-700 transition-colors cursor-pointer"
            >
              Cerrar sesión
            </button>
          </div>
        </div>
      </header>
      <main className="max-w-5xl mx-auto px-4 py-8">
        <Routes>
          <Route path="/" element={<ProductsPage />} />
          <Route path="/products/:id" element={<ProductDetailPage />} />
        </Routes>
      </main>
    </div>
  )
}
