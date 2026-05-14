export interface SaleResponse {
  saleId: string
  clientId: string
  clienteNombre: string
  productId: string
  productoDescripcion: string
  subscriptionId?: string
  userId: string
  userName: string
  fechaVenta: string
  monto: number
  pagado: boolean
  medioPago?: string
  referencia?: string
  createdAt: string
  updatedAt: string
}

export interface SalesListResponse {
  data: SaleResponse[]
  total: number
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface MarkSaleAsPaidRequest {
  medioPago?: string
  referencia?: string
}

export interface CreateSaleRequest {
  clientId: string
  productId: string
  fechaVenta: string
  pagado: boolean
  medioPago?: string
  referencia?: string
}

export interface GetSalesParams {
  pageNumber?: number
  pageSize?: number
  clientId?: string
  fromDate?: string
  toDate?: string
}
