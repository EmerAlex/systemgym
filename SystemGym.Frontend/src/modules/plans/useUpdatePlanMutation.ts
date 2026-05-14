import { useMutation, useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { updatePlan } from './plans-api'
import type { UpdatePlanRequest } from './plans.types'

export function useUpdatePlanMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ planId, payload }: { planId: string; payload: UpdatePlanRequest }) =>
      updatePlan(planId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.plans })
    },
  })
}
