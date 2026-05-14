import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'
import type { ClientResponse, ClientsListResponse, CreateClientRequest, UpdateClientRequest } from './clients.types'

export interface GetClientsParams {
  pageNumber?: number
  pageSize?: number
  searchTerm?: string
}

export async function getClients(params: GetClientsParams = {}): Promise<ClientsListResponse> {
  const { data } = await apiClient.get<ApiResponse<ClientsListResponse>>('/clients', {
    params: {
      pageNumber: params.pageNumber ?? 1,
      pageSize: params.pageSize ?? 10,
      searchTerm: params.searchTerm ?? undefined,
    },
  })

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Error al cargar clientes')
  }

  return data.data
}

export async function getClientById(clientId: string): Promise<ClientResponse> {
  const { data } = await apiClient.get<ApiResponse<ClientResponse>>(`/clients/${clientId}`)

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Cliente no encontrado')
  }

  return data.data
}

export async function createClient(payload: CreateClientRequest): Promise<string> {
  const { data } = await apiClient.post<ApiResponse<{ id: string; resource: null }>>(
    '/clients',
    payload,
  )

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Error al crear cliente')
  }

  return data.data.id
}

export async function updateClient(clientId: string, payload: UpdateClientRequest): Promise<void> {
  const { data } = await apiClient.put<ApiResponse<null>>(`/clients/${clientId}`, payload)
  if (!data.success) throw new Error(data.message ?? 'Error al actualizar cliente')
}
