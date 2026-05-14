export interface ProductResponse {
  productId: string
  descripcion: string
  valor: number
  habilitado: boolean
  cantidadActual: number
  createdAt: string
  updatedAt: string
}

export interface ProductsListResponse {
  data: ProductResponse[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface InventoryLogResponse {
  logId: string
  productId: string
  productoDescripcion: string
  cantidadAnterior: number
  cantidadNueva: number
  diferencia: number
  operacion: string
  motivoAuditoria: string
  createdAt: string
}

export interface InventoryLogsListResponse {
  data: InventoryLogResponse[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface AdjustInventoryRequest {
  productId: string
  cambio: number
  motivoAuditoria: string
}

export interface AdjustInventoryResult {
  productId: string
  logId: string
  cantidadAnterior: number
  cantidadNueva: number
  diferencia: number
  operacion: string
}
