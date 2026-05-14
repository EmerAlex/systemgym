import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import { Button, Paper } from '@mui/material'
import { useNavigate, useParams } from 'react-router-dom'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import { EditClientForm } from './EditClientForm'
import { useClientDetailQuery } from './useClientDetailQuery'
import { useUpdateClientMutation } from './useUpdateClientMutation'

export function EditClientPage() {
  const { clientId } = useParams<{ clientId: string }>()
  const navigate = useNavigate()
  const { data, isLoading, isError, error, refetch } = useClientDetailQuery(clientId ?? '')
  const mutation = useUpdateClientMutation(clientId)

  if (isLoading) return <FullPageLoader label="Cargando cliente..." />
  if (isError || !data) return (
    <ErrorState
      message={error instanceof Error ? error.message : 'Cliente no encontrado'}
      onRetry={() => refetch()}
    />
  )

  return (
    <PageContainer
      title="Editar cliente"
      description={`${data.nombreCompleto} (${data.tipoDocumento} ${data.numeroDocumento})`}
    >
      <Button
        startIcon={<ArrowBackIcon />}
        onClick={() => navigate(`/clients/${clientId}`)}
        sx={{ mb: 3 }}
      >
        Volver a cliente
      </Button>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 600 }}>
        <EditClientForm
          initialData={{
            tipoDocumento: data.tipoDocumento,
            numeroDocumento: data.numeroDocumento,
            nombreCompleto: data.nombreCompleto,
            celular: data.celular,
            habilitado: data.habilitado,
          }}
          onSubmit={(updatedData) => mutation.mutate(updatedData)}
          isPending={mutation.isPending}
          error={
            mutation.isError
              ? mutation.error instanceof Error
                ? mutation.error.message
                : 'Error al actualizar cliente'
              : undefined
          }
        />
      </Paper>
    </PageContainer>
  )
}
