import {
  Chip,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Typography,
} from '@mui/material'
import type { InventoryLogResponse } from './inventory.types'

interface InventoryLogsTableProps {
  rows: InventoryLogResponse[]
  totalCount: number
  page: number
  pageSize: number
  onPageChange: (p: number) => void
  onPageSizeChange: (s: number) => void
}

export function InventoryLogsTable({
  rows,
  totalCount,
  page,
  pageSize,
  onPageChange,
  onPageSizeChange,
}: InventoryLogsTableProps) {
  return (
    <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow sx={{ bgcolor: 'grey.50' }}>
              <TableCell><Typography variant="subtitle2">Fecha</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Producto</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Operación</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Anterior</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Nuevo</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Δ</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Motivo</Typography></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.logId} hover>
                <TableCell>
                  <Typography variant="caption">
                    {new Date(row.createdAt).toLocaleDateString('es-CO', {
                      year: 'numeric', month: 'short', day: 'numeric',
                    })}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">{row.productoDescripcion}</Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={row.operacion}
                    color={row.operacion === 'Add' ? 'success' : 'error'}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2">{row.cantidadAnterior}</Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2" fontWeight={700}>{row.cantidadNueva}</Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography
                    variant="body2"
                    fontWeight={600}
                    color={row.diferencia > 0 ? 'success.main' : 'error.main'}
                  >
                    {row.diferencia > 0 ? `+${row.diferencia}` : row.diferencia}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="caption" color="text.secondary">{row.motivoAuditoria}</Typography>
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
