import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../shared/constants/query-keys'
import { createSubscription, renewSubscription, cancelSubscription, registerIngreso } from './subscriptions-api'
import type { CreateSubscriptionRequest, RenewSubscriptionRequest } from './subscriptions.types'

export function useCreateSubscriptionMutation(clientId: string) {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: CreateSubscriptionRequest) => createSubscription(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.subscriptions(clientId) })
      navigate(`/clients/${clientId}/subscriptions`)
    },
  })
}

export function useRenewSubscriptionMutation(clientId: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ subscriptionId, payload }: { subscriptionId: string; payload: RenewSubscriptionRequest }) =>
      renewSubscription(subscriptionId, payload),
    onSuccess: () => {
      if (clientId) {
        // Si hay clientId, invalida queries específicas de ese cliente
        queryClient.invalidateQueries({ 
          queryKey: ['subscriptions', clientId],
          refetchType: 'all' 
        })
      } else {
        // Si no hay clientId, estamos en la página global de búsqueda
        // Invalida todas las queries de subscriptions globales
        queryClient.invalidateQueries({ 
          queryKey: ['subscriptions', 'all'],
          refetchType: 'all' 
        })
      }
      // También invalida caché de todas las suscripciones
      queryClient.invalidateQueries({
        queryKey: ['subscriptions'],
        refetchType: 'all'
      })
    },
    onError: (error) => {
      console.error('Error renovando suscripción:', error)
    }
  })
}

export function useCancelSubscriptionMutation(clientId: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (subscriptionId: string) => cancelSubscription(subscriptionId),
    onSuccess: () => {
      if (clientId) {
        queryClient.invalidateQueries({ queryKey: ['subscriptions', clientId] })
      } else {
        queryClient.invalidateQueries({ queryKey: ['subscriptions', 'all'] })
      }
      queryClient.invalidateQueries({ queryKey: ['subscriptions'] })
    },
  })
}

export function useRegisterIngresoMutation(clientId?: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (subscriptionId: string) => registerIngreso(subscriptionId),
    onSuccess: () => {
      if (clientId) {
        queryClient.invalidateQueries({ queryKey: queryKeys.subscriptions(clientId) })
      }
      queryClient.invalidateQueries({ queryKey: ['subscriptions', 'all'] })
    },
  })
}
