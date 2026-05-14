import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'
import type { CreateUserRequest, UpdateUserRequest, UserResponse, UsersListResponse } from './users.types'

export interface GetUsersParams {
  pageNumber?: number
  pageSize?: number
  searchTerm?: string
}

export async function getUsers(params: GetUsersParams = {}): Promise<UsersListResponse> {
  const { data } = await apiClient.get<ApiResponse<UsersListResponse>>('/auth/users', {
    params: {
      pageNumber: params.pageNumber ?? 1,
      pageSize: params.pageSize ?? 10,
      searchTerm: params.searchTerm ?? undefined,
    },
  })

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Error al cargar usuarios')
  }

  return data.data
}

export async function getUserById(userId: string): Promise<UserResponse> {
  const { data } = await apiClient.get<ApiResponse<UserResponse>>(`/auth/users/${userId}`)

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Usuario no encontrado')
  }

  return data.data
}

export async function createUser(payload: CreateUserRequest): Promise<string> {
  const { data } = await apiClient.post<ApiResponse<{ id: string; resource: null }>>(
    '/auth/register',
    payload,
  )

  if (!data.success || !data.data) {
    throw new Error(data.message ?? 'Error al crear usuario')
  }

  return data.data.id
}

export async function updateUser(userId: string, payload: UpdateUserRequest): Promise<void> {
  const { data } = await apiClient.put<ApiResponse<null>>(
    `/auth/users/${userId}`,
    payload,
  )

  if (!data.success) {
    throw new Error(data.message ?? 'Error al actualizar usuario')
  }
}

export async function deleteUser(userId: string): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/auth/users/${userId}`)

  if (!data.success) {
    throw new Error(data.message ?? 'Error al eliminar usuario')
  }
}
