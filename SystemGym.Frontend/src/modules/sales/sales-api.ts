import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'
import type { CreateSaleRequest, GetSalesParams, MarkSaleAsPaidRequest, SalesListResponse } from './sales.types'

export async function createSale(payload: CreateSaleRequest): Promise<string> {
  console.log('📤 sales-api.createSale - Enviando al backend:', {
    clientId: payload.clientId,
    productId: payload.productId,
    fechaVenta: payload.fechaVenta,
    pagado: payload.pagado,
  })
  try {
    const { data } = await apiClient.post<ApiResponse<{ id: string; resource: null }>>('/sales', payload)
    console.log('📥 sales-api.createSale - Respuesta del backend:', data)
    if (!data.success || !data.data) {
      const errorMsg = data.message ?? 'Error al crear venta'
      console.error('❌ sales-api.createSale - Error de negocio:', errorMsg)
      throw new Error(errorMsg)
    }
    return data.data.id
  } catch (error) {
    console.error('❌ sales-api.createSale - Error de red:', error)
    throw error
  }
}

export async function getSales(params: GetSalesParams = {}): Promise<SalesListResponse> {
  const { data } = await apiClient.get<ApiResponse<SalesListResponse>>('/sales', {
    params: {
      pageNumber: params.pageNumber ?? 1,
      pageSize: params.pageSize ?? 10,
      clientId: params.clientId,
      fromDate: params.fromDate,
      toDate: params.toDate,
    },
  })
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar ventas')
  return data.data
}

export async function markSaleAsPaid(
  saleId: string,
  payload: MarkSaleAsPaidRequest,
): Promise<void> {
  const { data } = await apiClient.put<ApiResponse<unknown>>(
    `/sales/${saleId}/mark-paid`,
    payload,
  )
  if (!data.success) throw new Error(data.message ?? 'Error al marcar como pagada')
}
