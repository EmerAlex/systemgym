import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getClientById } from './clients-api'

export function useClientDetailQuery(clientId: string) {
  return useQuery({
    queryKey: [...queryKeys.clients, clientId],
    queryFn: () => getClientById(clientId),
    enabled: !!clientId,
  })
}
