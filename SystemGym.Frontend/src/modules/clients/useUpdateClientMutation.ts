import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../shared/constants/query-keys'
import { updateClient } from './clients-api'
import type { UpdateClientRequest } from './clients.types'

export function useUpdateClientMutation(clientId?: string) {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: UpdateClientRequest | { clientId: string; payload: UpdateClientRequest }) => {
      // Soportar ambos formatos para compatibilidad
      if ('clientId' in payload) {
        return updateClient(payload.clientId, payload.payload)
      }
      return updateClient(clientId!, payload)
    },
    onSuccess: (_, variables) => {
      const id = 'clientId' in variables ? variables.clientId : clientId
      
      // Invalidar queries de clientes
      queryClient.invalidateQueries({ queryKey: queryKeys.clients })
      
      // Si estamos en página de edición, redirigir a detalle
      if (clientId) {
        navigate(`/clients/${id}`)
      }
    },
  })
}
