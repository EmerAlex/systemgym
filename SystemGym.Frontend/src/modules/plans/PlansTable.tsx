import {
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
  MenuItem,
  Alert,
} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import { useState } from 'react'
import { useAppSelector } from '../../app/store/hooks'
import type { PlanResponse } from './plans.types'
import { useUpdatePlanMutation } from './useUpdatePlanMutation'
import { useDeletePlanMutation } from './useDeletePlanMutation'

interface PlansTableProps {
  rows: PlanResponse[]
}

const PERIODO_LABELS: Record<string, string> = {
  Dia: 'día',
  Mes: 'mes',
}

function formatPeriodo(plan: PlanResponse): string {
  const n = plan.cantidadIntervalosPeriodo
  const label = PERIODO_LABELS[plan.tipoPeriodo] ?? plan.tipoPeriodo.toLowerCase()
  const plural = plan.tipoPeriodo === 'Mes' ? `${label}es` : `${label}s`
  return n === 1 ? `1 ${label}` : `${n} ${plural}`
}

function formatValor(valor: number): string {
  return new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 }).format(valor)
}

export function PlansTable({ rows }: PlansTableProps) {
  const role = useAppSelector((state) => state.auth.session?.role)
  const isAdmin = role === 'Admin'

  // Edit dialog state
  const [editPlan, setEditPlan] = useState<PlanResponse | null>(null)
  const [editForm, setEditForm] = useState({ descripcion: '', tipoPeriodo: 'Mes', cantidadIntervalosPeriodo: 1, valor: 0 })
  const [editError, setEditError] = useState<string | null>(null)
  const updateMutation = useUpdatePlanMutation()

  // Delete dialog state
  const [deletePlan, setDeletePlan] = useState<PlanResponse | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)
  const deleteMutation = useDeletePlanMutation()

  function openEdit(plan: PlanResponse) {
    setEditPlan(plan)
    setEditForm({
      descripcion: plan.descripcion,
      tipoPeriodo: plan.tipoPeriodo,
      cantidadIntervalosPeriodo: plan.cantidadIntervalosPeriodo,
      valor: plan.valor,
    })
    setEditError(null)
  }

  function closeEdit() {
    setEditPlan(null)
    setEditError(null)
  }

  async function handleEditSubmit() {
    if (!editPlan) return
    setEditError(null)
    try {
      await updateMutation.mutateAsync({ planId: editPlan.planId, payload: editForm })
      closeEdit()
    } catch (err) {
      setEditError(err instanceof Error ? err.message : 'Error al actualizar plan')
    }
  }

  function openDelete(plan: PlanResponse) {
    setDeletePlan(plan)
    setDeleteError(null)
  }

  function closeDelete() {
    setDeletePlan(null)
    setDeleteError(null)
  }

  async function handleDeleteConfirm() {
    if (!deletePlan) return
    setDeleteError(null)
    try {
      await deleteMutation.mutateAsync(deletePlan.planId)
      closeDelete()
    } catch (err) {
      setDeleteError(err instanceof Error ? err.message : 'Error al eliminar plan')
    }
  }

  return (
    <>
      <Paper elevation={0} sx={{ borderRadius: 2, overflow: 'hidden', border: '1px solid', borderColor: 'divider' }}>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow sx={{ bgcolor: 'grey.50' }}>
                <TableCell><Typography variant="subtitle2">Descripción</Typography></TableCell>
                <TableCell><Typography variant="subtitle2">Duración</Typography></TableCell>
                <TableCell align="right"><Typography variant="subtitle2">Valor</Typography></TableCell>
                <TableCell><Typography variant="subtitle2">Estado</Typography></TableCell>
                {isAdmin && <TableCell align="center"><Typography variant="subtitle2">Acciones</Typography></TableCell>}
              </TableRow>
            </TableHead>
            <TableBody>
              {rows.map((row) => (
                <TableRow key={row.planId} hover>
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>{row.descripcion}</Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2">{formatPeriodo(row)}</Typography>
                  </TableCell>
                  <TableCell align="right">
                    <Typography variant="body2" fontWeight={600} color="primary.main">
                      {formatValor(row.valor)}
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
                  {isAdmin && (
                    <TableCell align="center">
                      <Stack direction="row" spacing={0.5} justifyContent="center">
                        <Tooltip title="Editar">
                          <IconButton size="small" onClick={() => openEdit(row)} color="primary">
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Eliminar">
                          <IconButton size="small" onClick={() => openDelete(row)} color="error">
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </Stack>
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Dialog: Editar plan */}
      <Dialog open={!!editPlan} onClose={closeEdit} maxWidth="xs" fullWidth>
        <DialogTitle>Editar plan</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {editError && <Alert severity="error">{editError}</Alert>}
            <TextField
              label="Descripción"
              value={editForm.descripcion}
              onChange={(e) => setEditForm((f) => ({ ...f, descripcion: e.target.value }))}
              fullWidth
              size="small"
            />
            <TextField
              label="Tipo periodo"
              select
              value={editForm.tipoPeriodo}
              onChange={(e) => setEditForm((f) => ({ ...f, tipoPeriodo: e.target.value }))}
              fullWidth
              size="small"
            >
              <MenuItem value="Mes">Mes</MenuItem>
              <MenuItem value="Dia">Día</MenuItem>
            </TextField>
            <TextField
              label="Cantidad de intervalos"
              type="number"
              value={editForm.cantidadIntervalosPeriodo}
              onChange={(e) => setEditForm((f) => ({ ...f, cantidadIntervalosPeriodo: Number(e.target.value) }))}
              fullWidth
              size="small"
              inputProps={{ min: 1 }}
            />
            <TextField
              label="Valor (COP)"
              type="number"
              value={editForm.valor}
              onChange={(e) => setEditForm((f) => ({ ...f, valor: Number(e.target.value) }))}
              fullWidth
              size="small"
              inputProps={{ min: 0 }}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={closeEdit} disabled={updateMutation.isPending}>Cancelar</Button>
          <Button variant="contained" onClick={handleEditSubmit} disabled={updateMutation.isPending}>
            {updateMutation.isPending ? 'Guardando...' : 'Guardar'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Dialog: Confirmar eliminar */}
      <Dialog open={!!deletePlan} onClose={closeDelete} maxWidth="xs" fullWidth>
        <DialogTitle>Eliminar plan</DialogTitle>
        <DialogContent>
          {deleteError && <Alert severity="error" sx={{ mb: 2 }}>{deleteError}</Alert>}
          <DialogContentText>
            ¿Estás seguro de que deseas eliminar el plan <strong>{deletePlan?.descripcion}</strong>? Esta acción no se puede deshacer.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={closeDelete} disabled={deleteMutation.isPending}>Cancelar</Button>
          <Button variant="contained" color="error" onClick={handleDeleteConfirm} disabled={deleteMutation.isPending}>
            {deleteMutation.isPending ? 'Eliminando...' : 'Eliminar'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  )
}

