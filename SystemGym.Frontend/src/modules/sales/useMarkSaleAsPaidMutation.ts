import { useMutation, useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { markSaleAsPaid } from './sales-api'
import type { MarkSaleAsPaidRequest } from './sales.types'

export function useMarkSaleAsPaidMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ saleId, payload }: { saleId: string; payload: MarkSaleAsPaidRequest }) =>
      markSaleAsPaid(saleId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.sales })
    },
  })
}
