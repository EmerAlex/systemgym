import axios from 'axios'

export function mapHttpError(error: unknown): string {
  if (axios.isAxiosError(error)) {
    return (error.response?.data as { message?: string } | undefined)?.message ?? 'Error de comunicación con la API'
  }

  if (error instanceof Error) {
    return error.message
  }

  return 'Ocurrió un error inesperado'
}
