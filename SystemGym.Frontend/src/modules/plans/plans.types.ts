export interface PlanResponse {
  planId: string
  descripcion: string
  tipoPeriodo: string
  cantidadIntervalosPeriodo: number
  valor: number
  habilitado: boolean
  createdAt: string
  updatedAt: string
}

export interface PlansListResponse {
  data: PlanResponse[]
  totalCount: number
}

export interface CreatePlanRequest {
  descripcion: string
  tipoPeriodo: string
  cantidadIntervalosPeriodo: number
  valor: number
}

export interface UpdatePlanRequest {
  descripcion: string
  tipoPeriodo: string
  cantidadIntervalosPeriodo: number
  valor: number
}
