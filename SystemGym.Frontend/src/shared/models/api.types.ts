export interface ApiResponse<TData> {
  success: boolean
  message: string
  data: TData
  errors?: Record<string, string[]>
}
