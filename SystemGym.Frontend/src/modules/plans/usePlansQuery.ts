import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../shared/constants/query-keys'
import { getPlans } from './plans-api'

export function usePlansQuery() {
  return useQuery({
    queryKey: queryKeys.plans,
    queryFn: () => getPlans(),
    staleTime: 5 * 60 * 1000, // planes cambian poco
  })
}
