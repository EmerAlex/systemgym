import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { adjustInventory, getInventoryLogs, getProducts } from './inventory-api'
import type { AdjustInventoryRequest } from './inventory.types'

export function useProductsQuery() {
  return useQuery({
    queryKey: ['products'],
    queryFn: () => getProducts(),
  })
}

export function useInventoryLogsQuery(page = 0, pageSize = 10, productId?: string) {
  return useQuery({
    queryKey: [...queryKeys.inventoryLogs, page, pageSize, productId],
    queryFn: () => getInventoryLogs(page + 1, pageSize, productId),
  })
}

export function useAdjustInventoryMutation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: AdjustInventoryRequest) => adjustInventory(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
      queryClient.invalidateQueries({ queryKey: queryKeys.inventoryLogs })
    },
  })
}
