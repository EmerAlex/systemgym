import { lazy, Suspense } from 'react'
import { createBrowserRouter } from 'react-router-dom'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { ProtectedRoute } from './ProtectedRoute'
import { RoleGuard } from './RoleGuard'
import { AppShell } from '../../shared/layout/AppShell'
import { ForbiddenPage } from '../../shared/pages/ForbiddenPage'
import { LoginPage } from '../../shared/pages/LoginPage'
import { HomeRedirect } from '../../shared/pages/HomeRedirect'

// Lazy-loaded pages — each becomes a separate chunk
const DashboardPage       = lazy(() => import('../../shared/pages/DashboardPage').then((m) => ({ default: m.DashboardPage })))
const UsersListPage       = lazy(() => import('../../modules/users/UsersListPage').then((m) => ({ default: m.UsersListPage })))
const CreateUserPage      = lazy(() => import('../../modules/users/CreateUserPage').then((m) => ({ default: m.CreateUserPage })))
const ClientsListPage     = lazy(() => import('../../modules/clients/ClientsListPage').then((m) => ({ default: m.ClientsListPage })))
const CreateClientPage    = lazy(() => import('../../modules/clients/CreateClientPage').then((m) => ({ default: m.CreateClientPage })))
const ClientDetailPage    = lazy(() => import('../../modules/clients/ClientDetailPage').then((m) => ({ default: m.ClientDetailPage })))
const EditClientPage      = lazy(() => import('../../modules/clients/EditClientPage').then((m) => ({ default: m.EditClientPage })))
const PlansListPage       = lazy(() => import('../../modules/plans/PlansListPage').then((m) => ({ default: m.PlansListPage })))
const CreatePlanPage      = lazy(() => import('../../modules/plans/CreatePlanPage').then((m) => ({ default: m.CreatePlanPage })))
const CreateProductPage   = lazy(() => import('../../modules/inventory/CreateProductPage').then((m) => ({ default: m.CreateProductPage })))
const ClientSubscriptionsPage  = lazy(() => import('../../modules/subscriptions/ClientSubscriptionsPage').then((m) => ({ default: m.ClientSubscriptionsPage })))
const CreateSubscriptionPage   = lazy(() => import('../../modules/subscriptions/CreateSubscriptionPage').then((m) => ({ default: m.CreateSubscriptionPage })))
const SubscriptionsListPage    = lazy(() => import('../../modules/subscriptions/SubscriptionsListPage').then((m) => ({ default: m.SubscriptionsListPage })))
const SalesListPage       = lazy(() => import('../../modules/sales/SalesListPage').then((m) => ({ default: m.SalesListPage })))
const CreateSalePage      = lazy(() => import('../../modules/sales/CreateSalePage').then((m) => ({ default: m.CreateSalePage })))
const InventoryPage       = lazy(() => import('../../modules/inventory/InventoryPage').then((m) => ({ default: m.InventoryPage })))

function S({ children }: { children: React.ReactNode }) {
  return <Suspense fallback={<FullPageLoader />}>{children}</Suspense>
}

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/forbidden',
    element: <ForbiddenPage />,
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppShell />,
        children: [
          // Ruta raíz - redirige según rol (Admin -> Dashboard, otros -> Clients)
          {
            path: '/',
            element: <HomeRedirect />,
          },
          // Dashboard (solo Admin)
          {
            element: <RoleGuard allowedRoles={['Admin']} />,
            children: [
              {
                path: '/dashboard',
                element: <S><DashboardPage /></S>,
              },
              // Admin section
              {
                path: '/admin/users',
                element: <S><UsersListPage /></S>,
              },
              {
                path: '/admin/users/new',
                element: <S><CreateUserPage /></S>,
              },
              {
                path: '/inventory',
                element: <S><InventoryPage /></S>,
              },
              {
                path: '/inventory/products/new',
                element: <S><CreateProductPage /></S>,
              },
            ],
          },
          // Clientes
          {
            path: '/clients',
            element: <S><ClientsListPage /></S>,
          },
          {
            path: '/clients/new',
            element: <S><CreateClientPage /></S>,
          },
          {
            path: '/clients/:clientId',
            element: <S><ClientDetailPage /></S>,
          },
          {
            path: '/clients/:clientId/edit',
            element: <S><EditClientPage /></S>,
          },
          // Planes
          {
            path: '/plans',
            element: <S><PlansListPage /></S>,
          },
          {
            path: '/plans/new',
            element: <S><CreatePlanPage /></S>,
          },
          // Suscripciones (globales — ambos roles)
          {
            path: '/subscriptions',
            element: <S><SubscriptionsListPage /></S>,
          },
          // Suscripciones por cliente
          {
            path: '/clients/:clientId/subscriptions',
            element: <S><ClientSubscriptionsPage /></S>,
          },
          {
            path: '/clients/:clientId/subscriptions/new',
            element: <S><CreateSubscriptionPage /></S>,
          },
          // Ventas
          {
            path: '/sales',
            element: <S><SalesListPage /></S>,
          },
          {
            path: '/sales/new',
            element: <S><CreateSalePage /></S>,
          },
        ],
      },
    ],
  },
])
