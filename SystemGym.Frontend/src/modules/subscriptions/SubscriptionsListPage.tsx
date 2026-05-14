import { useState, useEffect } from 'react'
import {
  Box,
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextField,
  Typography,
} from '@mui/material'
import SearchIcon from '@mui/icons-material/Search'
import FileDownloadIcon from '@mui/icons-material/FileDownload'
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
import { useAllSubscriptionsQuery } from './useAllSubscriptionsQuery'
import { exportSubscriptionsCsv } from './subscriptions-api'

const TIPOS_DOCUMENTO = ['CC', 'TI', 'CE', 'PAS']

export function SubscriptionsListPage() {
  const role = useAppSelector((s) => s.auth.session?.role)
  const isAdmin = role === 'Admin'

  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(10)
  const [isExporting, setIsExporting] = useState(false)

  // Valores del formulario (lo que el usuario escribe)
  const [tipoDocForm, setTipoDocForm] = useState('CC')
  const [numDocForm, setNumDocForm] = useState('')

  // Parámetros de búsqueda confirmados (se aplican al presionar Buscar)
  const [searchTipoDoc, setSearchTipoDoc] = useState<string | undefined>(undefined)
  const [searchNumDoc, setSearchNumDoc] = useState<string | undefined>(undefined)

  // Admin: carga inmediata sin búsqueda obligatoria; estándar: solo tras buscar
  const queryEnabled = isAdmin ? true : !!searchNumDoc

  const { data, isLoading, isError, error, refetch } = useAllSubscriptionsQuery(
    page,
    pageSize,
    searchTipoDoc,
    searchNumDoc,
    queryEnabled,
  )

  const renewMutation = useRenewSubscriptionMutation('')
  const cancelMutation = useCancelSubscriptionMutation('')
  const ingresoMutation = useRegisterIngresoMutation()

  useEffect(() => {
    if (renewMutation.isSuccess) refetch()
  }, [renewMutation.isSuccess, refetch])

  useEffect(() => {
    if (cancelMutation.isSuccess) refetch()
  }, [cancelMutation.isSuccess, refetch])

  useEffect(() => {
    if (ingresoMutation.isSuccess) refetch()
  }, [ingresoMutation.isSuccess, refetch])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    // Admin puede buscar sin documento; estándar requiere documento
    if (!isAdmin && !numDocForm.trim()) return
    setPage(0)
    setSearchTipoDoc(numDocForm.trim() ? tipoDocForm : undefined)
    setSearchNumDoc(numDocForm.trim() || undefined)
  }

  const handleRenew = (subscriptionId: string, tieneExpiracion: boolean) => {
    const today = new Date().toISOString().split('T')[0]
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

  const handleExportCsv = async () => {
    setIsExporting(true)
    try {
      await exportSubscriptionsCsv()
    } catch {
      alert('Error al exportar CSV')
    } finally {
      setIsExporting(false)
    }
  }

  const hasSearched = isAdmin ? true : !!searchNumDoc

  const pageTitle = isAdmin ? 'Monitoreo de Suscripciones' : 'Suscripciones'
  const pageDescription = isAdmin
    ? 'Visualice y gestione todas las suscripciones. Puede filtrar por documento.'
    : 'Busque un cliente por tipo y número de documento para ver sus suscripciones'

  return (
    <PageContainer title={pageTitle} description={pageDescription}>
      {/* Formulario de búsqueda */}
      <Box
        component="form"
        onSubmit={handleSearch}
        sx={{ display: 'flex', gap: 2, mb: 3, alignItems: 'flex-end', flexWrap: 'wrap' }}
      >
        {(isAdmin || !searchNumDoc) && (
          <FormControl size="small" sx={{ minWidth: 120 }}>
            <InputLabel>Tipo Doc.</InputLabel>
            <Select
              value={tipoDocForm}
              label="Tipo Doc."
              onChange={(e) => setTipoDocForm(e.target.value)}
            >
              {TIPOS_DOCUMENTO.map((t) => (
                <MenuItem key={t} value={t}>{t}</MenuItem>
              ))}
            </Select>
          </FormControl>
        )}

        <TextField
          size="small"
          label="Número de documento"
          value={numDocForm}
          onChange={(e) => setNumDocForm(e.target.value)}
          placeholder="Ej: 1234567890"
          sx={{ minWidth: 220 }}
          required={!isAdmin}
        />

        <Button
          type="submit"
          variant="contained"
          startIcon={<SearchIcon />}
          disabled={!isAdmin && !numDocForm.trim()}
        >
          Buscar
        </Button>

        <Button
          variant="outlined"
          startIcon={<FileDownloadIcon />}
          onClick={handleExportCsv}
          disabled={isExporting}
        >
          {isExporting ? 'Exportando...' : 'Exportar CSV'}
        </Button>
      </Box>

      {/* Estado: sin búsqueda (solo para no-admin) */}
      {!hasSearched && (
        <EmptyState
          title="Ingrese un documento para buscar"
          description="Seleccione el tipo de documento e ingrese el número para ver las suscripciones del cliente."
        />
      )}

      {/* Estado: cargando */}
      {hasSearched && isLoading && <FullPageLoader label="Cargando suscripciones..." />}

      {/* Estado: error */}
      {hasSearched && isError && (
        <ErrorState
          message={error instanceof Error ? error.message : 'Error al buscar suscripciones'}
          onRetry={() => refetch()}
        />
      )}

      {/* Resultados */}
      {hasSearched && !isLoading && !isError && (
        <>
          {data && data.data.length === 0 ? (
            <EmptyState
              title="Sin suscripciones"
              description={
                searchNumDoc
                  ? `No se encontraron suscripciones para ${searchTipoDoc} ${searchNumDoc}.`
                  : 'No hay suscripciones registradas.'
              }
            />
          ) : (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                {data?.totalCount ?? 0} suscripción(es)
                {searchNumDoc
                  ? <> encontrada(s) para <strong>{searchTipoDoc} {searchNumDoc}</strong>
                      {data?.clienteNombreCompleto ? ` — ${data.clienteNombreCompleto}` : ''}
                    </>
                  : ' en total'}
              </Typography>
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
                showClientColumn
                renewingId={renewMutation.isPending ? renewMutation.variables?.subscriptionId : undefined}
                cancellingId={cancelMutation.isPending ? cancelMutation.variables : undefined}
                registerIngresoId={ingresoMutation.isPending ? ingresoMutation.variables : undefined}
              />
            </>
          )}
        </>
      )}
    </PageContainer>
  )
}

