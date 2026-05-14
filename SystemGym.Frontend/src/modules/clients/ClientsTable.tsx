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
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import VisibilityIcon from '@mui/icons-material/Visibility'
import { useNavigate } from 'react-router-dom'
import type { ClientResponse } from './clients.types'

interface ClientsTableProps {
  rows: ClientResponse[]
  totalCount: number
  page: number
  pageSize: number
  onPageChange: (newPage: number) => void
  onPageSizeChange: (newSize: number) => void
}

export function ClientsTable({
  rows,
  totalCount,
  page,
  pageSize,
  onPageChange,
  onPageSizeChange,
}: ClientsTableProps) {
  const navigate = useNavigate()

  return (
    <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow sx={{ bgcolor: 'grey.50' }}>
              <TableCell><Typography variant="subtitle2">Documento</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Nombre</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Celular</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Estado</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Registro</Typography></TableCell>
              <TableCell align="center"><Typography variant="subtitle2">Acciones</Typography></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.clientId} hover>
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>
                    {row.tipoDocumento} {row.numeroDocumento}
                  </Typography>
                </TableCell>
                <TableCell>{row.nombreCompleto}</TableCell>
                <TableCell>{row.celular ?? '—'}</TableCell>
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
                    {new Date(row.createdAt).toLocaleDateString('es-CO')}
                  </Typography>
                </TableCell>
                <TableCell align="center">
                  <Tooltip title="Ver detalle">
                    <IconButton size="small" onClick={() => navigate(`/clients/${row.clientId}`)}>
                      <VisibilityIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
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
  )
}
