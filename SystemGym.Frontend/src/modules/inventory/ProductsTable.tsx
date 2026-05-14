import TuneIcon from '@mui/icons-material/Tune'
import {
  Chip,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material'
import type { ProductResponse } from './inventory.types'

interface ProductsTableProps {
  rows: ProductResponse[]
  onAdjust: (product: ProductResponse) => void
}

function formatCOP(value: number) {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
  }).format(value)
}

export function ProductsTable({ rows, onAdjust }: ProductsTableProps) {
  return (
    <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow sx={{ bgcolor: 'grey.50' }}>
              <TableCell><Typography variant="subtitle2">Producto</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Precio</Typography></TableCell>
              <TableCell align="right"><Typography variant="subtitle2">Stock</Typography></TableCell>
              <TableCell><Typography variant="subtitle2">Estado</Typography></TableCell>
              <TableCell align="center"><Typography variant="subtitle2">Ajustar</Typography></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.productId} hover>
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>{row.descripcion}</Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2">{formatCOP(row.valor)}</Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography
                    variant="body2"
                    fontWeight={700}
                    color={row.cantidadActual <= 5 ? 'error.main' : 'text.primary'}
                  >
                    {row.cantidadActual}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Chip
                    label={row.habilitado ? 'Activo' : 'Inactivo'}
                    color={row.habilitado ? 'success' : 'default'}
                    size="small"
                    variant="outlined"
                  />
                </TableCell>
                <TableCell align="center">
                  <Tooltip title="Ajustar inventario">
                    <IconButton size="small" color="primary" onClick={() => onAdjust(row)}>
                      <TuneIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Paper>
  )
}
