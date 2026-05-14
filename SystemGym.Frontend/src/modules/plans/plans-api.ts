import type { ApiResponse } from '../../shared/models/api.types'
import { apiClient } from '../../infrastructure/http/api-client'
import type { PlansListResponse, PlanResponse, CreatePlanRequest, UpdatePlanRequest } from './plans.types'

export async function createPlan(payload: CreatePlanRequest): Promise<string> {
  const { data } = await apiClient.post<ApiResponse<{ id: string; resource: null }>>('/plans', payload)
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al crear plan')
  return data.data.id
}

export async function getPlans(pageNumber = 1, pageSize = 50): Promise<PlansListResponse> {
  const { data } = await apiClient.get<ApiResponse<PlansListResponse>>('/plans', {
    params: { pageNumber, pageSize },
  })
  if (!data.success || !data.data) throw new Error(data.message ?? 'Error al cargar planes')
  return data.data
}

export async function getPlanById(planId: string): Promise<PlanResponse> {
  const { data } = await apiClient.get<ApiResponse<PlanResponse>>(`/plans/${planId}`)
  if (!data.success || !data.data) throw new Error(data.message ?? 'Plan no encontrado')
  return data.data
}

export async function updatePlan(planId: string, payload: UpdatePlanRequest): Promise<void> {
  const { data } = await apiClient.put<ApiResponse<null>>(`/plans/${planId}`, payload)
  if (!data.success) throw new Error(data.message ?? 'Error al actualizar plan')
}

export async function deletePlan(planId: string): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/plans/${planId}`)
  if (!data.success) throw new Error(data.message ?? 'Error al eliminar plan')
}
