import { Box, Button, Stack, TextField, Typography } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { EmptyState } from '../../shared/feedback/EmptyState'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import { SalesTable } from './SalesTable'
import { useMarkSaleAsPaidMutation } from './useMarkSaleAsPaidMutation'
import { useSalesQuery } from './useSalesQuery'

export function SalesListPage() {
  const navigate = useNavigate()

  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(10)
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')

  const { data, isLoading, isError, error, refetch } = useSalesQuery({
    pageNumber: page + 1,
    pageSize,
    fromDate: fromDate || undefined,
    toDate: toDate || undefined,
  })

  const markPaidMutation = useMarkSaleAsPaidMutation()

  const handleMarkPaid = (saleId: string) => {
    markPaidMutation.mutate({ saleId, payload: {} })
  }

  if (isLoading) return <FullPageLoader label="Cargando ventas..." />
  if (isError) return (
    <ErrorState
      message={error instanceof Error ? error.message : 'Error al cargar ventas'}
      onRetry={() => refetch()}
    />
  )

  return (
    <PageContainer
      title="Historial de ventas"
      description={`${data?.totalCount ?? 0} ventas`}
    >
      <Button
        variant="contained"
        startIcon={<AddIcon />}
        onClick={() => navigate('/sales/new')}
        sx={{ mb: 3, alignSelf: 'flex-start' }}
      >
        Nueva venta
      </Button>

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="center" sx={{ mb: 3 }}>
        <TextField
          label="Desde"
          type="date"
          size="small"
          InputLabelProps={{ shrink: true }}
          value={fromDate}
          onChange={(e) => { setFromDate(e.target.value); setPage(0) }}
        />
        <TextField
          label="Hasta"
          type="date"
          size="small"
          InputLabelProps={{ shrink: true }}
          value={toDate}
          onChange={(e) => { setToDate(e.target.value); setPage(0) }}
        />
        {data && data.total > 0 && (
          <Box sx={{ ml: 'auto' }}>
            <Typography variant="subtitle1" fontWeight={700} color="primary.main">
              Total: {new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 }).format(data.total)}
            </Typography>
          </Box>
        )}
      </Stack>

      {data && data.data.length === 0 ? (
        <EmptyState title="Sin ventas" description="Las ventas se generan automáticamente al crear o renovar suscripciones." />
      ) : (
        <SalesTable
          rows={data?.data ?? []}
          totalCount={data?.totalCount ?? 0}
          page={page}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={(s) => { setPageSize(s); setPage(0) }}
          onMarkPaid={handleMarkPaid}
          markingId={markPaidMutation.isPending ? markPaidMutation.variables?.saleId : undefined}
        />
      )}
    </PageContainer>
  )
}
