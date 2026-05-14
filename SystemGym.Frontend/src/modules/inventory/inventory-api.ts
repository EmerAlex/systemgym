import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'
import type {
  AdjustInventoryRequest,
  AdjustInventoryResult,
  InventoryLogsListResponse,
  ProductsListResponse,
} from './inventory.types'

export interface CreateProductRequest {
  descripcion: string
  valor: number
}

export async function createProduct(payload: CreateProductRequest): Promise<string> {
  const { data } = await apiClient.post<ApiResponse<{ id: string; resource: null }>>('/products', payload)
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al crear producto')
  return data.data.id
}

export async function getProducts(pageNumber = 1, pageSize = 50): Promise<ProductsListResponse> {
  const { data } = await apiClient.get<ApiResponse<ProductsListResponse>>('/products', {
    params: { pageNumber, pageSize },
  })
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar productos')
  return data.data
}

export async function getInventoryLogs(
  pageNumber = 1,
  pageSize = 10,
  productId?: string,
): Promise<InventoryLogsListResponse> {
  const { data } = await apiClient.get<ApiResponse<InventoryLogsListResponse>>('/inventory/logs', {
    params: { pageNumber, pageSize, productId },
  })
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar logs')
  return data.data
}

export async function adjustInventory(
  payload: AdjustInventoryRequest,
): Promise<AdjustInventoryResult> {
  const { data } = await apiClient.post<ApiResponse<AdjustInventoryResult>>(
    '/inventory/adjust',
    payload,
  )
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al ajustar inventario')
  return data.data
}
