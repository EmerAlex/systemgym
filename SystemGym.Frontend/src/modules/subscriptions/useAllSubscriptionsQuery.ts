import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getAllSubscriptions } from './subscriptions-api'

export function useAllSubscriptionsQuery(
  page: number,
  pageSize: number,
  tipoDocumento?: string,
  numeroDocumento?: string,
  enabled = true,
) {
  return useQuery({
    queryKey: queryKeys.allSubscriptions(page, tipoDocumento, numeroDocumento),
    queryFn: () => getAllSubscriptions(page + 1, pageSize, tipoDocumento, numeroDocumento),
    enabled,
    placeholderData: (prev) => prev,
  })
}
