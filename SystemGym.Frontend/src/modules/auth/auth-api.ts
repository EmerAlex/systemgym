import type { ApiResponse } from '../../shared/models/api.types'
import type { AuthSession } from '../../shared/models/auth.types'
import { apiClient } from '../../infrastructure/http/api-client'

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponseData {
  success: boolean
  token: string
  expiresIn: number
  role: string
}

export async function loginApi(payload: LoginRequest): Promise<AuthSession> {
  const { data } = await apiClient.post<ApiResponse<LoginResponseData>>(
    '/auth/login',
    payload,
  )

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Error al iniciar sesión')
  }

  return {
    token: data.data.token,
    username: payload.username,
    role: data.data.role as AuthSession['role'],
  }
}
