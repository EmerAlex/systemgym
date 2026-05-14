import CheckCircleIcon from '@mui/icons-material/CheckCircle'
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
import type { SaleResponse } from './sales.types'

interface SalesTableProps {
  rows: SaleResponse[]
  totalCount: number
  page: number
  pageSize: number
  onPageChange: (p: number) => void
  onPageSizeChange: (s: number) => void
  onMarkPaid: (saleId: string) => void
  markingId?: string
}

function formatCOP(value: number) {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
  }).format(value)
}

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString('es-CO', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

export function SalesTable({
  rows,
  totalCount,
  page,
  pageSize,
  onPageChange,
  onPageSizeChange,
  onMarkPaid,
  markingId,
}: SalesTableProps) {
  return (
    <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow sx={{ bgcolor: 'grey.50' }}>
              <TableCell><Typography variant="subtitle2">Fecha</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Cliente</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Producto/Plan</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Monto</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Medio pago</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Vendedor</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Estado</Typography></TableCell>
              <TableCell align="center"><Typography variant="subtitle2">Acción</Typography></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.saleId} hover>
                <TableCell>
                  <Typography variant="caption">{formatDate(row.fechaVenta)}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>{row.clienteNombre}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">{row.productoDescripcion}</Typography>
                  {row.subscriptionId && (
                    <Typography variant="caption" color="text.secondary"> · Suscripción</Typography>
                  )}
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2" fontWeight={600} color="primary.main">
                    {formatCOP(row.monto)}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="caption">{row.medioPago ?? '—'}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="caption" sx={{ fontWeight: 500 }}>{row.userName}</Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={row.pagado ? 'Pagado' : 'Pendiente'}
                    color={row.pagado ? 'success' : 'warning'}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell align="center">
                  {!row.pagado && (
                    <Tooltip title="Marcar como pagada">
                      <span>
                        <IconButton
                          size="small"
                          color="success"
                          disabled={markingId === row.saleId}
                          onClick={() => onMarkPaid(row.saleId)}
                        >
                          <CheckCircleIcon fontSize="small" />
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
        rowsPerPageOptions={[10, 25, 50]}
        labelRowsPerPage="Filas:"
        labelDisplayedRows={({ from, to, count }) => `${from}–${to} de ${count}`}
      />
    </Paper>
  )
}
