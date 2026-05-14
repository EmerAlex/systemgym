export interface ClientResponse {
  clientId: string
  tipoDocumento: string
  numeroDocumento: string
  nombreCompleto: string
  celular?: string
  habilitado: boolean
  createdAt: string
  updatedAt: string
}

export interface ClientsListResponse {
  data: ClientResponse[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface CreateClientRequest {
  tipoDocumento: string
  numeroDocumento: string
  nombreCompleto: string
  celular?: string
}

export interface UpdateClientRequest {
  tipoDocumento: string
  numeroDocumento: string
  nombreCompleto: string
  celular?: string
  habilitado: boolean
}
