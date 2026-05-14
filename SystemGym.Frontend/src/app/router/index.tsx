import { RouterProvider } from 'react-router-dom'
import { router } from './route-config'

export function AppRouter() {
  return <RouterProvider router={router} />
}
