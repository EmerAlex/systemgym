import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getClientSubscriptions } from './subscriptions-api'

export function useClientSubscriptionsQuery(clientId: string, page = 0, pageSize = 10) {
  return useQuery({
    queryKey: [...queryKeys.subscriptions(clientId), page, pageSize],
    queryFn: () => getClientSubscriptions(clientId, page + 1, pageSize),
    enabled: !!clientId,
  })
}
