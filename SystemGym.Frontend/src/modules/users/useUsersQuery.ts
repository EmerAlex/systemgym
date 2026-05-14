import { useQuery } from '@tanstack/react-query'
import { getUsers } from './users-api'
import type { GetUsersParams } from './users-api'

const queryKeys = {
  all: ['users'] as const,
  lists: () => [...queryKeys.all, 'list'] as const,
  list: (filters: GetUsersParams) => [...queryKeys.lists(), { filters }] as const,
  details: () => [...queryKeys.all, 'detail'] as const,
  detail: (id: string) => [...queryKeys.details(), id] as const,
}

export function useUsersQuery(params: GetUsersParams = {}) {
  return useQuery({
    queryKey: queryKeys.list(params),
    queryFn: () => getUsers(params),
  })
}
