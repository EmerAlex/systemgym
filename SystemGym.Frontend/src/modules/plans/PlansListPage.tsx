import AddIcon from '@mui/icons-material/Add'
import { Button } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { EmptyState } from '../../shared/feedback/EmptyState'
import { PageContainer } from '../../shared/layout/PageContainer'
import { PlansTable } from './PlansTable'
import { usePlansQuery } from './usePlansQuery'

export function PlansListPage() {
  const navigate = useNavigate()
  const { data, isLoading, isError, error, refetch } = usePlansQuery()

  if (isLoading) return <FullPageLoader label="Cargando planes..." />
  if (isError) return (
    <ErrorState
      message={error instanceof Error ? error.message : 'Error al cargar planes'}
      onRetry={() => refetch()}
    />
  )

  return (
    <PageContainer
      title="Planes"
      description={`${data?.totalCount ?? 0} planes registrados`}
    >
      <Button
        variant="contained"
        startIcon={<AddIcon />}
        onClick={() => navigate('/plans/new')}
        sx={{ mb: 3, alignSelf: 'flex-start' }}
      >
        Nuevo plan
      </Button>

      {data && data.data.length === 0 ? (
        <EmptyState
          title="Sin planes"
          description="No hay planes registrados en el sistema."
          action={
            <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/plans/new')}>
              Crear primer plan
            </Button>
          }
        />
      ) : (
        <PlansTable rows={data?.data ?? []} />
      )}
    </PageContainer>
  )
}
