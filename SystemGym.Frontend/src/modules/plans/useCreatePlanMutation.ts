import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../shared/constants/query-keys'
import { createPlan } from './plans-api'
import type { CreatePlanRequest } from './plans.types'

export function useCreatePlanMutation() {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: CreatePlanRequest) => createPlan(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.plans })
      navigate('/plans')
    },
  })
}
