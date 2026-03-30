import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { SEEDED_USERS } from '../types'

const AVATAR_COLORS = [
  'bg-indigo-500',
  'bg-emerald-500',
]

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()

  function handleLogin(user: typeof SEEDED_USERS[number]) {
    login(user)
    navigate('/')
  }

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="w-full max-w-sm flex flex-col items-center gap-8">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-indigo-600 tracking-tight">ReviewShare</h1>
          <p className="text-sm text-gray-500 mt-2">Elige tu cuenta para continuar</p>
        </div>

        <div className="w-full flex flex-col gap-3">
          {SEEDED_USERS.map((user, i) => (
            <button
              key={user.id}
              onClick={() => handleLogin(user)}
              className="w-full bg-white border border-gray-200 rounded-xl px-5 py-4 flex items-center gap-4 shadow-sm hover:shadow-md hover:border-indigo-300 transition-all cursor-pointer text-left"
            >
              <div className={`${AVATAR_COLORS[i]} w-11 h-11 rounded-full flex items-center justify-center shrink-0`}>
                <span className="text-white text-lg font-semibold">
                  {user.name.charAt(0)}
                </span>
              </div>
              <div>
                <p className="text-sm font-semibold text-gray-900">{user.name}</p>
                <p className="text-xs text-gray-400">Cuenta de demostración</p>
              </div>
              <span className="ml-auto text-gray-300 text-lg">→</span>
            </button>
          ))}
        </div>
      </div>
    </div>
  )
}
