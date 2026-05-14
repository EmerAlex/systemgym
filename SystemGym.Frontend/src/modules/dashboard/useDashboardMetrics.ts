import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getDashboardMetrics } from './dashboard-api'

export interface DashboardMetrics {
  totalClients: number
  totalPlans: number
  totalSubscriptions: number
  subscriptionsThisMonth: number
  salesToday: number
  salesAmountToday: number
  salesThisMonth: number
  salesAmountThisMonth: number
  isLoading: boolean
  isError: boolean
}

export function useDashboardMetrics(): DashboardMetrics {
  const { data, isLoading, isError } = useQuery({
    queryKey: queryKeys.dashboard,
    queryFn: getDashboardMetrics,
    staleTime: 60 * 1000, // 1 minuto
    refetchInterval: 5 * 60 * 1000, // Recargar cada 5 minutos
  })

  return {
    totalClients: data?.totalClients ?? 0,
    totalPlans: data?.totalPlans ?? 0,
    totalSubscriptions: data?.totalSubscriptions ?? 0,
    subscriptionsThisMonth: data?.subscriptionsThisMonth ?? 0,
    salesToday: data?.salesToday ?? 0,
    salesAmountToday: data?.salesAmountToday ?? 0,
    salesThisMonth: data?.salesThisMonth ?? 0,
    salesAmountThisMonth: data?.salesAmountThisMonth ?? 0,
    isLoading,
    isError,
  }
}
