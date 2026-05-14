import { useMutation, useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { deletePlan } from './plans-api'

export function useDeletePlanMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (planId: string) => deletePlan(planId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.plans })
    },
  })
}
