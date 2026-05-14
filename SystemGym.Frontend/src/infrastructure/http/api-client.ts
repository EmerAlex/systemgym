import axios from 'axios'
import { clearSession as clearSessionAction } from '../../app/store/slices/authSlice'
import { store } from '../../app/store'
import { env } from '../config/env'
import { attachAuthInterceptor } from './auth-interceptor'
import { clearSession as clearStoredSession } from '../../modules/auth/session-storage'

export const apiClient = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

attachAuthInterceptor(apiClient)

// Extrae el mensaje de error del body del API en lugar del texto HTTP genérico
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error?.response?.status === 401) {
      clearStoredSession()
      store.dispatch(clearSessionAction())

      if (typeof window !== 'undefined' && window.location.pathname !== '/login') {
        const expiredMessage = 'Tu sesión expiró. Inicia sesión nuevamente.'
        window.sessionStorage.setItem('systemgym_auth_error', expiredMessage)
        window.location.assign('/login')
      }
    }

    const apiMessage: string | undefined = error?.response?.data?.message
    if (apiMessage) {
      error.message = apiMessage
    }
    return Promise.reject(error)
  },
)
