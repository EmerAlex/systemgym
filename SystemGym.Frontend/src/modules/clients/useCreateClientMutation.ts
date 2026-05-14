import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../shared/constants/query-keys'
import { createClient } from './clients-api'
import type { CreateClientRequest } from './clients.types'

export function useCreateClientMutation() {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: CreateClientRequest) => createClient(payload),
    onSuccess: (clientId) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.clients })
      navigate(`/clients/${clientId}`)
    },
  })
}
