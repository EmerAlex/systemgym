import { Box, Button, CircularProgress, Stack, TextField, Typography } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { PageContainer } from '../../shared/layout/PageContainer'
import { useUsersQuery } from './useUsersQuery'
import { UsersTable } from './UsersTable'

export function UsersListPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(10)
  const [searchTerm, setSearchTerm] = useState('')

  const { data, isLoading, isError } = useUsersQuery({
    pageNumber: page + 1,
    pageSize,
    searchTerm: searchTerm || undefined,
  })

  return (
    <PageContainer title="Gestión de Usuarios" description="Administra los usuarios del sistema">
      <Stack spacing={3}>
        <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between">
          <TextField
            placeholder="Buscar usuario..."
            size="small"
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value)
              setPage(0)
            }}
            sx={{ width: 280 }}
          />
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/admin/users/new')}
          >
            Nuevo Usuario
          </Button>
        </Stack>

        {isLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : isError ? (
          <Typography color="error">Error al cargar usuarios</Typography>
        ) : data?.data.length === 0 ? (
          <Typography color="text.secondary" textAlign="center" sx={{ py: 4 }}>
            No hay usuarios registrados
          </Typography>
        ) : (
          <UsersTable
            rows={data?.data ?? []}
            totalCount={data?.totalCount ?? 0}
            page={page}
            pageSize={pageSize}
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
          />
        )}
      </Stack>
    </PageContainer>
  )
}
