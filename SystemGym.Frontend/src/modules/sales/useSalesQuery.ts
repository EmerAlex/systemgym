import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getSales } from './sales-api'
import type { GetSalesParams } from './sales.types'

export function useSalesQuery(params: GetSalesParams = {}) {
  return useQuery({
    queryKey: [...queryKeys.sales, params],
    queryFn: () => getSales(params),
  })
}
