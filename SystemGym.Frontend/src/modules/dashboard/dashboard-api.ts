import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'

export interface DashboardMetrics {
  totalClients: number
  totalPlans: number
  totalSubscriptions: number
  subscriptionsThisMonth: number
  salesToday: number
  salesAmountToday: number
  salesThisMonth: number
  salesAmountThisMonth: number
}

export async function getDashboardMetrics(): Promise<DashboardMetrics> {
  const { data } = await apiClient.get<ApiResponse<DashboardMetrics>>(
    '/dashboard/metrics',
  )
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar métricas del dashboard')
  return data.data
}
