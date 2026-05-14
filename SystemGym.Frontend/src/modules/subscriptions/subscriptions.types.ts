export interface SaleGenerated {
  saleId: string
  monto: number
  pagado: boolean
  fechaVenta: string
}

export interface SubscriptionResponse {
  subscriptionId: string
  clientId: string
  clientNombreCompleto?: string
  clientTipoDocumento?: string
  clientNumeroDocumento?: string
  planId: string
  planDescripcion: string
  inicioVigencia: string
  finVigencia: string | null
  tieneExpiracion: boolean
  activa: boolean
  ultimoIngreso?: string
  cantidadIngresos: number
  valor: number
  saleGenerated?: SaleGenerated
  createdAt: string
  updatedAt: string
}

export interface SubscriptionsListResponse {
  data: SubscriptionResponse[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  clienteEncontrado: boolean
  clienteNombreCompleto?: string
}

export interface CreateSubscriptionRequest {
  clientId: string
  planId: string
  inicioVigencia: string
  tieneExpiracion: boolean
}

export interface RenewSubscriptionRequest {
  nuevoInicio: string
  tieneExpiracion: boolean
}
