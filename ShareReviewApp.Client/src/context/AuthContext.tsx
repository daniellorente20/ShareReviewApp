import { createContext, useContext, useState } from 'react'
import type { ReactNode } from 'react'

interface User {
  id: string
  name: string
}

interface AuthContextValue {
  currentUser: User | null
  login: (user: User) => void
  logout: () => void
}

const SESSION_KEY = 'reviewshare_user'

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [currentUser, setCurrentUser] = useState<User | null>(() => {
    const stored = sessionStorage.getItem(SESSION_KEY)
    return stored ? (JSON.parse(stored) as User) : null
  })

  function login(user: User) {
    sessionStorage.setItem(SESSION_KEY, JSON.stringify(user))
    setCurrentUser(user)
  }

  function logout() {
    sessionStorage.removeItem(SESSION_KEY)
    setCurrentUser(null)
  }

  return (
    <AuthContext.Provider value={{ currentUser, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}
