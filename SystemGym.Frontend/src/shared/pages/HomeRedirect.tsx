import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAppSelector } from '../../app/store/hooks'
import { FullPageLoader } from '../feedback/FullPageLoader'

export function HomeRedirect() {
  const navigate = useNavigate()
  const role = useAppSelector((state) => state.auth.session?.role)

  useEffect(() => {
    if (role === 'Admin') {
      navigate('/dashboard', { replace: true })
    } else {
      navigate('/clients', { replace: true })
    }
  }, [role, navigate])

  return <FullPageLoader />
}
