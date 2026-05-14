const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined

if (!apiBaseUrl) {
  throw new Error('VITE_API_BASE_URL no está configurada')
}

export const env = {
  apiBaseUrl,
  enableDevAuth: import.meta.env.VITE_ENABLE_DEV_AUTH === 'true',
}
