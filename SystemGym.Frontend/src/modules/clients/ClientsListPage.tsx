import AddIcon from '@mui/icons-material/Add'
import SearchIcon from '@mui/icons-material/Search'
import { Button, InputAdornment, Stack, TextField } from '@mui/material'
import { useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import { ClientsTable } from './ClientsTable'
import { useClientsQuery } from './useClientsQuery'

export function ClientsListPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(10)
  const [search, setSearch] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null)

  const { data, isLoading, isError, error, refetch } = useClientsQuery({
    pageNumber: page + 1,
    pageSize,
    searchTerm: debouncedSearch || undefined,
  })

  const handleSearch = (value: string) => {
    setSearch(value)
    setPage(0)
    if (debounceRef.current) clearTimeout(debounceRef.current)
    debounceRef.current = setTimeout(() => setDebouncedSearch(value), 400)
  }

  return (
    <PageContainer
      title="Clientes"
      description={`${data?.totalCount ?? 0} clientes registrados`}
    >
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 3 }} alignItems="center">
        <TextField
          placeholder="Buscar por nombre o documento..."
          value={search}
          onChange={(e) => handleSearch(e.target.value)}
          size="small"
          sx={{ flex: 1, maxWidth: 400 }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon fontSize="small" />
              </InputAdornment>
            ),
          }}
        />
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate('/clients/new')}
        >
          Nuevo cliente
        </Button>
      </Stack>

      {isLoading ? (
        <FullPageLoader label="Cargando clientes..." />
      ) : isError ? (
        <ErrorState
          message={error instanceof Error ? error.message : 'Error al cargar clientes'}
          onRetry={() => refetch()}
        />
      ) : data && (
        <ClientsTable
          rows={data.data}
          totalCount={data.totalCount}
          page={page}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={(s) => { setPageSize(s); setPage(0) }}
        />
      )}
    </PageContainer>
  )
}
