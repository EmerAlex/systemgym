import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch } from '../../app/store/hooks'
import { setSession } from '../../app/store/slices/authSlice'
import { loginApi, type LoginRequest } from './auth-api'
import { SESSION_STORAGE_KEY } from './session-storage'

export function useLoginMutation() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (payload: LoginRequest) => loginApi(payload),
    onSuccess: (session) => {
      localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session))
      dispatch(setSession(session))
      const destination = session.role === 'Admin' ? '/dashboard' : '/clients'
      navigate(destination, { replace: true })
    },
  })
}
