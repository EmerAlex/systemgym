import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import AssignmentIcon from '@mui/icons-material/Assignment'
import EditIcon from '@mui/icons-material/Edit'
import { Button, Chip, Divider, Paper, Stack, Typography } from '@mui/material'
import { useNavigate, useParams } from 'react-router-dom'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import { useClientDetailQuery } from './useClientDetailQuery'

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <Stack direction="row" spacing={2} alignItems="baseline">
      <Typography variant="body2" color="text.secondary" sx={{ minWidth: 160 }}>
        {label}
      </Typography>
      <Typography variant="body1">{value}</Typography>
    </Stack>
  )
}

export function ClientDetailPage() {
  const { clientId } = useParams<{ clientId: string }>()
  const navigate = useNavigate()
  const { data, isLoading, isError, error, refetch } = useClientDetailQuery(clientId ?? '')

  if (isLoading) return <FullPageLoader label="Cargando cliente..." />
  if (isError || !data) return (
    <ErrorState
      message={error instanceof Error ? error.message : 'Cliente no encontrado'}
      onRetry={() => refetch()}
    />
  )

  return (
    <PageContainer
      title={data.nombreCompleto}
      description={`${data.tipoDocumento} ${data.numeroDocumento}`}
    >
      <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/clients')}>
          Volver a clientes
        </Button>
        <Button
          variant="outlined"
          startIcon={<EditIcon />}
          onClick={() => navigate(`/clients/${clientId}/edit`)}
        >
          Editar
        </Button>
        <Button
          variant="outlined"
          startIcon={<AssignmentIcon />}
          onClick={() => navigate(`/clients/${clientId}/subscriptions`)}
        >
          Ver suscripciones
        </Button>
      </Stack>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 600 }}>
        <Stack spacing={2}>
          <Stack direction="row" alignItems="center" spacing={2}>
            <Typography variant="h6">Información del cliente</Typography>
            <Chip
              label={data.habilitado ? 'Activo' : 'Inactivo'}
              color={data.habilitado ? 'success' : 'default'}
              size="small"
              variant="outlined"
            />
          </Stack>

          <Divider />

          <InfoRow label="Tipo de documento" value={data.tipoDocumento} />
          <InfoRow label="Número de documento" value={data.numeroDocumento} />
          <InfoRow label="Nombre completo" value={data.nombreCompleto} />
          <InfoRow label="Celular" value={data.celular ?? '—'} />
          <InfoRow
            label="Registrado"
            value={new Date(data.createdAt).toLocaleDateString('es-CO', {
              year: 'numeric', month: 'long', day: 'numeric',
            })}
          />
          <InfoRow
            label="Actualizado"
            value={new Date(data.updatedAt).toLocaleDateString('es-CO', {
              year: 'numeric', month: 'long', day: 'numeric',
            })}
          />
        </Stack>
      </Paper>
    </PageContainer>
  )
}
