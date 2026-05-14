import { Navigate, Outlet } from 'react-router-dom'
import { useAppSelector } from '../store/hooks'
import type { UserRole } from '../../shared/models/auth.types'

interface RoleGuardProps {
  allowedRoles: UserRole[]
}

export function RoleGuard({ allowedRoles }: RoleGuardProps) {
  const role = useAppSelector((state) => state.auth.session?.role)

  if (!role) {
    return <Navigate to="/login" replace />
  }

  if (!allowedRoles.includes(role)) {
    return <Navigate to="/forbidden" replace />
  }

  return <Outlet />
}
