import AddIcon from '@mui/icons-material/Add'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import { Button, Stack } from '@mui/material'
import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useAppSelector } from '../../app/store/hooks'
import { EmptyState } from '../../shared/feedback/EmptyState'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import {
  useCancelSubscriptionMutation,
  useRenewSubscriptionMutation,
  useRegisterIngresoMutation,
} from './subscriptions-mutations'
import { SubscriptionsTable } from './SubscriptionsTable'
import { useClientSubscriptionsQuery } from './useClientSubscriptionsQuery'

export function ClientSubscriptionsPage() {
  const { clientId } = useParams<{ clientId: string }>()
  const navigate = useNavigate()
  const role = useAppSelector((s) => s.auth.session?.role)
  const isAdmin = role === 'Admin'

  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(10)

  const { data, isLoading, isError, error, refetch } = useClientSubscriptionsQuery(
    clientId ?? '',
    page,
    pageSize,
  )

  const renewMutation = useRenewSubscriptionMutation(clientId ?? '')
  const cancelMutation = useCancelSubscriptionMutation(clientId ?? '')
  const ingresoMutation = useRegisterIngresoMutation(clientId ?? '')

  // Refetch cuando la renovación completa exitosamente
  useEffect(() => {
    if (renewMutation.isSuccess) {
      refetch()
    }
  }, [renewMutation.isSuccess, refetch])

  // Refetch cuando la cancelación completa exitosamente
  useEffect(() => {
    if (cancelMutation.isSuccess) {
      refetch()
    }
  }, [cancelMutation.isSuccess, refetch])

  // Refetch cuando se registra ingreso exitosamente
  useEffect(() => {
    if (ingresoMutation.isSuccess) {
      refetch()
    }
  }, [ingresoMutation.isSuccess, refetch])

  const handleRenew = (subscriptionId: string, tieneExpiracion: boolean) => {
    const today = new Date().toISOString().split('T')[0] // Solo fecha: YYYY-MM-DD, sin componente de hora
    renewMutation.mutate({ subscriptionId, payload: { nuevoInicio: today, tieneExpiracion } })
  }

  const handleCancel = (subscriptionId: string) => {
    if (window.confirm('¿Cancelar esta suscripción?')) {
      cancelMutation.mutate(subscriptionId)
    }
  }

  const handleRegisterIngreso = (subscriptionId: string) => {
    ingresoMutation.mutate(subscriptionId)
  }

  if (isLoading) return <FullPageLoader label="Cargando suscripciones..." />
  if (isError) return (
    <ErrorState
      message={error instanceof Error ? error.message : 'Error al cargar suscripciones'}
      onRetry={() => refetch()}
    />
  )

  return (
    <PageContainer
      title="Suscripciones"
      description={`${data?.totalCount ?? 0} suscripciones del cliente`}
    >
      <Stack direction="row" spacing={2} sx={{ mb: 3 }} alignItems="center">
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/clients/${clientId}`)}>
          Volver al cliente
        </Button>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate(`/clients/${clientId}/subscriptions/new`)}
        >
          Nueva suscripción
        </Button>
      </Stack>

      {data && data.data.length === 0 ? (
        <EmptyState
          title="Sin suscripciones"
          description="Este cliente no tiene suscripciones registradas."
        />
      ) : (
        <SubscriptionsTable
          rows={data?.data ?? []}
          totalCount={data?.totalCount ?? 0}
          page={page}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={(s) => { setPageSize(s); setPage(0) }}
          onRenew={handleRenew}
          onCancel={handleCancel}
          onRegisterIngreso={handleRegisterIngreso}
          isAdmin={isAdmin}
          renewingId={renewMutation.isPending ? renewMutation.variables?.subscriptionId : undefined}
          cancellingId={cancelMutation.isPending ? cancelMutation.variables : undefined}
          registerIngresoId={ingresoMutation.isPending ? ingresoMutation.variables : undefined}
        />
      )}
    </PageContainer>
  )
}
