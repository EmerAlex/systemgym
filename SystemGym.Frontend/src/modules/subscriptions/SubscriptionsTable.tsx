import CancelIcon from '@mui/icons-material/Cancel'
import RefreshIcon from '@mui/icons-material/Refresh'
import LoginIcon from '@mui/icons-material/Login'
import {
  Chip,
  IconButton,
  Paper,
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
import type { SubscriptionResponse } from './subscriptions.types'

interface SubscriptionsTableProps {
  rows: SubscriptionResponse[]
  totalCount: number
  page: number
  pageSize: number
  onPageChange: (p: number) => void
  onPageSizeChange: (s: number) => void
  onRenew: (subscriptionId: string, tieneExpiracion: boolean) => void
  onCancel: (subscriptionId: string) => void
  onRegisterIngreso: (subscriptionId: string) => void
  isAdmin: boolean
  renewingId?: string
  cancellingId?: string
  registerIngresoId?: string
  showClientColumn?: boolean
}

function formatDate(iso: string | null) {
  if (!iso) return '—'
  // Extraer solo la parte de fecha (YYYY-MM-DD) sin convertir a zona local
  const parts = iso.split('T')[0].split('-') // ["2026", "05", "14"]
  const [year, month, day] = parts
  const monthNames = ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic']
  return `${day} de ${monthNames[parseInt(month) - 1]} de ${year}`
}

function formatCOP(value: number) {
  return new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 }).format(value)
}

export function SubscriptionsTable({
  rows,
  totalCount,
  page,
  pageSize,
  onPageChange,
  onPageSizeChange,
  onRenew,
  onCancel,
  onRegisterIngreso,
  isAdmin,
  renewingId,
  cancellingId,
  registerIngresoId,
  showClientColumn = false,
}: SubscriptionsTableProps) {
  const isIngresoHoy = (ultimoIngreso?: string) => {
    if (!ultimoIngreso) return false
    return new Date(ultimoIngreso).toDateString() === new Date().toDateString()
  }

  return (
    <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow sx={{ bgcolor: 'grey.50' }}>
              {showClientColumn && <TableCell><Typography variant="subtitle2">Documento</Typography></TableCell>}
              {showClientColumn && <TableCell><Typography variant="subtitle2">Cliente</Typography></TableCell>}
              <TableCell><Typography variant="subtitle2">Plan</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Inicio</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Vence</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Valor</Typography></TableCell>
              <TableCell align="center"><Typography variant="subtitle2">Ingresos</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Último Ingreso</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Estado</Typography></TableCell>
              <TableCell align="center"><Typography variant="subtitle2">Acciones</Typography></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.subscriptionId} hover>
                {showClientColumn && (
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 500 }}>
                      {row.clientTipoDocumento ?? '—'} {row.clientNumeroDocumento ?? ''}
                    </Typography>
                  </TableCell>
                )}
                {showClientColumn && (
                  <TableCell>
                    <Typography variant="body2">
                      {row.clientNombreCompleto ?? '—'}
                    </Typography>
                  </TableCell>
                )}
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>{row.planDescripcion}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="caption">{formatDate(row.inicioVigencia)}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="caption">
                    {row.tieneExpiracion ? formatDate(row.finVigencia) : '∞ Sin expiración'}
                  </Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2" fontWeight={600} color="primary.main">
                    {formatCOP(row.valor)}
                  </Typography>
                </TableCell>
                <TableCell align="center">
                  <Chip
                    label={row.cantidadIngresos}
                    color={row.cantidadIngresos > 0 ? 'info' : 'default'}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell>
                  <Typography variant="caption" color={isIngresoHoy(row.ultimoIngreso) ? 'success.main' : 'text.secondary'}>
                    {row.ultimoIngreso ? formatDate(row.ultimoIngreso) : '—'}
                    {isIngresoHoy(row.ultimoIngreso) && ' ✓'}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={row.activa ? 'Activa' : 'Inactiva'}
                    color={row.activa ? 'success' : 'default'}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell align="center" sx={{ whiteSpace: 'nowrap' }}>
                  <Tooltip title={isIngresoHoy(row.ultimoIngreso) ? 'Ya registró ingreso hoy' : 'Registrar ingreso'}>
                    <span>
                      <IconButton
                        size="small"
                        color="success"
                        disabled={
                          !row.activa ||
                          row.cantidadIngresos <= 0 ||
                          isIngresoHoy(row.ultimoIngreso) ||
                          registerIngresoId === row.subscriptionId
                        }
                        onClick={() => onRegisterIngreso(row.subscriptionId)}
                      >
                        <LoginIcon fontSize="small" />
                      </IconButton>
                    </span>
                  </Tooltip>
                  <Tooltip title="Renovar">
                    <span>
                      <IconButton
                        size="small"
                        color="primary"
                        disabled={renewingId === row.subscriptionId}
                        onClick={() => onRenew(row.subscriptionId, row.tieneExpiracion)}
                      >
                        <RefreshIcon fontSize="small" />
                      </IconButton>
                    </span>
                  </Tooltip>
                  {isAdmin && (
                    <Tooltip title="Cancelar">
                      <span>
                        <IconButton
                          size="small"
                          color="error"
                          disabled={!row.activa || cancellingId === row.subscriptionId}
                          onClick={() => onCancel(row.subscriptionId)}
                        >
                          <CancelIcon fontSize="small" />
                        </IconButton>
                      </span>
                    </Tooltip>
                  )}
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
        rowsPerPageOptions={[10, 25]}
        labelRowsPerPage="Filas:"
        labelDisplayedRows={({ from, to, count }) => `${from}–${to} de ${count}`}
      />
    </Paper>
  )
}
