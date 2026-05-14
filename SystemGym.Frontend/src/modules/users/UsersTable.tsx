import {
  Alert,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'
import { useState } from 'react'
import type { UserResponse } from './users.types'
import { useDeleteUserMutation } from './useDeleteUserMutation'

interface UsersTableProps {
  rows: UserResponse[]
  totalCount: number
  page: number
  pageSize: number
  onPageChange: (newPage: number) => void
  onPageSizeChange: (newSize: number) => void
}

export function UsersTable({
  rows,
  totalCount,
  page,
  pageSize,
  onPageChange,
  onPageSizeChange,
}: UsersTableProps) {
  const deleteMutation = useDeleteUserMutation()
  const [deleteConfirm, setDeleteConfirm] = useState<UserResponse | null>(null)

  async function handleDelete() {
    if (!deleteConfirm) return
    try {
      await deleteMutation.mutateAsync(deleteConfirm.userId)
      setDeleteConfirm(null)
    } catch (err) {
      console.error('Error deleting user:', err)
    }
  }

  return (
    <>
      <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow sx={{ bgcolor: 'grey.50' }}>
                <TableCell><Typography variant="subtitle2">Usuario</Typography></TableCell>
                <TableCell><Typography variant="subtitle2">Rol</Typography></TableCell>
                <TableCell><Typography variant="subtitle2">Estado</Typography></TableCell>
                <TableCell><Typography variant="subtitle2">Último acceso</Typography></TableCell>
                <TableCell><Typography variant="subtitle2">Registro</Typography></TableCell>
                <TableCell align="center"><Typography variant="subtitle2">Acciones</Typography></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row) => (
                <TableRow key={row.userId} hover>
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {row.username}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={row.role === 'Admin' ? 'Administrador' : 'Estándar'}
                      color={row.role === 'Admin' ? 'primary' : 'default'}
                      size="small"
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={row.habilitado ? 'Activo' : 'Inactivo'}
                      color={row.habilitado ? 'success' : 'default'}
                      size="small"
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell>
                    <Typography variant="caption" color="text.secondary">
                      {row.lastLogin ? new Date(row.lastLogin).toLocaleDateString('es-CO') : '—'}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="caption" color="text.secondary">
                      {new Date(row.createdAt).toLocaleDateString('es-CO')}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">
                    <Stack direction="row" spacing={0.5} justifyContent="center">
                      <Tooltip title="Eliminar">
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => setDeleteConfirm(row)}
                          disabled={deleteMutation.isPending}
                        >
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </Stack>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          component="div"
          count={totalCount}
          page={page}
          rowsPerPage={pageSize}
          onPageChange={(_, p) => onPageChange(p)}
          onRowsPerPageChange={(e) => onPageSizeChange(Number(e.target.value))}
          rowsPerPageOptions={[10, 25, 50]}
          labelRowsPerPage="Filas:"
          labelDisplayedRows={({ from, to, count }) => `${from}–${to} de ${count}`}
        />
      </Paper>

      {/* Dialog: Confirmar eliminación */}
      <Dialog open={!!deleteConfirm} onClose={() => setDeleteConfirm(null)} maxWidth="xs" fullWidth>
        <DialogTitle>Eliminar usuario</DialogTitle>
        <DialogContent>
          <Alert severity="warning" sx={{ mt: 2 }}>
            ¿Estás seguro de que deseas eliminar el usuario <strong>{deleteConfirm?.username}</strong>?
          </Alert>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteConfirm(null)} disabled={deleteMutation.isPending}>
            Cancelar
          </Button>
          <Button
            variant="contained"
            color="error"
            onClick={handleDelete}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending ? 'Eliminando...' : 'Eliminar'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  )
}
