import { useBootstrapSession } from '../modules/auth/useBootstrapSession'
import { AppRouter } from './router'

export function AppBootstrap() {
  useBootstrapSession()
  return <AppRouter />
}
