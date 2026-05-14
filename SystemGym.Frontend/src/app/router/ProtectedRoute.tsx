import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAppSelector } from '../store/hooks'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'

export function ProtectedRoute() {
  const location = useLocation()
  const { session, bootstrapCompleted } = useAppSelector((state) => state.auth)

  if (!bootstrapCompleted) {
    return <FullPageLoader label="Inicializando sesión" />
  }

  if (!session) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  return <Outlet />
}
