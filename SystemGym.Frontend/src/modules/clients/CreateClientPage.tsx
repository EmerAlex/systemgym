import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import { Button, Paper } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { PageContainer } from '../../shared/layout/PageContainer'
import { ClientForm } from './ClientForm'
import { useCreateClientMutation } from './useCreateClientMutation'

export function CreateClientPage() {
  const navigate = useNavigate()
  const mutation = useCreateClientMutation()

  return (
    <PageContainer title="Nuevo cliente" description="Registra un cliente en el sistema">
      <Button
        startIcon={<ArrowBackIcon />}
        onClick={() => navigate('/clients')}
        sx={{ mb: 3 }}
      >
        Volver a clientes
      </Button>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 600 }}>
        <ClientForm
          onSubmit={(data) => mutation.mutate(data)}
          isPending={mutation.isPending}
          error={
            mutation.isError
              ? mutation.error instanceof Error
                ? mutation.error.message
                : 'Error al crear cliente'
              : undefined
          }
        />
      </Paper>
    </PageContainer>
  )
}
