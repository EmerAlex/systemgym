import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getClients, type GetClientsParams } from './clients-api'

export function useClientsQuery(params: GetClientsParams = {}) {
  return useQuery({
    queryKey: [...queryKeys.clients, params],
    queryFn: () => getClients(params),
  })
}
