export type UserRole = 'Admin' | 'Standard'

export interface AuthSession {
  token: string
  username: string
  role: UserRole
}
