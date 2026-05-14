import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'
import type {
  SubscriptionsListResponse,
  CreateSubscriptionRequest,
  RenewSubscriptionRequest,
} from './subscriptions.types'

export async function getClientSubscriptions(
  clientId: string,
  pageNumber = 1,
  pageSize = 10,
): Promise<SubscriptionsListResponse> {
  const { data } = await apiClient.get<ApiResponse<SubscriptionsListResponse>>(
    `/subscriptions/client/${clientId}`,
    { params: { pageNumber, pageSize } },
  )
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar suscripciones')
  return data.data
}

export async function createSubscription(
  payload: CreateSubscriptionRequest,
): Promise<{ subscriptionId: string; saleId?: string }> {
  const { data } = await apiClient.post<ApiResponse<{ subscriptionId: string; saleId?: string }>>(
    '/subscriptions',
    payload,
  )
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al crear suscripción')
  return data.data
}

export async function renewSubscription(
  subscriptionId: string,
  payload: RenewSubscriptionRequest,
): Promise<void> {
  const { data } = await apiClient.put<ApiResponse<unknown>>(
    `/subscriptions/${subscriptionId}/renew`,
    payload,
  )
  if (!data.success) throw new Error(data.message ?? 'Error al renovar suscripción')
}

export async function cancelSubscription(subscriptionId: string): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<unknown>>(
    `/subscriptions/${subscriptionId}`,
  )
  if (!data.success) throw new Error(data.message ?? 'Error al cancelar suscripción')
}

export async function getAllSubscriptions(
  pageNumber = 1,
  pageSize = 10,
  tipoDocumento?: string,
  numeroDocumento?: string,
): Promise<SubscriptionsListResponse> {
  const { data } = await apiClient.get<ApiResponse<SubscriptionsListResponse>>(
    '/subscriptions',
    { params: { pageNumber, pageSize, tipoDocumento, numeroDocumento } },
  )
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar suscripciones')
  return data.data
}

export async function registerIngreso(subscriptionId: string): Promise<string> {
  const { data } = await apiClient.post<ApiResponse<{ subscriptionId: string }>>(
    `/subscriptions/${subscriptionId}/ingreso`,
  )
  if (!data.success) throw new Error(data.message ?? 'Error al registrar ingreso')
  return subscriptionId
}

export async function exportSubscriptionsCsv(
  tipoDocumento?: string,
  numeroDocumento?: string,
  nombreCliente?: string,
): Promise<void> {
  const response = await apiClient.get('/subscriptions/export-csv', {
    params: { tipoDocumento, numeroDocumento, nombreCliente },
    responseType: 'blob',
  })
  const url = URL.createObjectURL(new Blob([response.data], { type: 'text/csv;charset=utf-8;' }))
  const link = document.createElement('a')
  link.href = url
  link.download = `suscripciones-${new Date().toISOString().split('T')[0]}.csv`
  link.click()
  URL.revokeObjectURL(url)
}
