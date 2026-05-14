import { useEffect } from 'react'
import { useAppDispatch } from '../../app/store/hooks'
import { setBootstrapCompleted, setSession } from '../../app/store/slices/authSlice'
import { loadSession } from './session-storage'

export function useBootstrapSession() {
  const dispatch = useAppDispatch()

  useEffect(() => {
    dispatch(setBootstrapCompleted(false))
    const session = loadSession()
    if (session) {
      dispatch(setSession(session))
    }
    dispatch(setBootstrapCompleted(true))
  }, [dispatch])
}
