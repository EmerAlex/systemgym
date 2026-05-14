import type { AxiosInstance } from 'axios'
import { store } from '../../app/store'

export function attachAuthInterceptor(apiClient: AxiosInstance) {
  apiClient.interceptors.request.use((config) => {
    const token = store.getState().auth.session?.token

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  })
}
