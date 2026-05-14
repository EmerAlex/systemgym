import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../shared/constants/query-keys'
import { createSale } from './sales-api'
import type { CreateSaleRequest } from './sales.types'

 export function useCreateSaleMutation() {
   const queryClient = useQueryClient()
   const navigate = useNavigate()
 
   return useMutation({
     mutationFn: (payload: CreateSaleRequest) => {
       console.log('🚀 useCreateSaleMutation.mutationFn - Enviando payload:', payload)
       return createSale(payload)
     },
     onSuccess: (data) => {
       console.log('✅ useCreateSaleMutation.onSuccess - Venta creada:', data)
       queryClient.invalidateQueries({ queryKey: queryKeys.sales })
       navigate('/sales')
     },
     onError: (error) => {
       console.error('❌ useCreateSaleMutation.onError:', error)
       if (error instanceof Error) {
         console.error('   Message:', error.message)
         console.error('   Stack:', error.stack)
       }
     },
   })
 }
