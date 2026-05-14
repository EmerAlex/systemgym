import { zodResolver } from '@hookform/resolvers/zod'
import {
  Alert,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { useAdjustInventoryMutation } from './inventory-hooks'
import type { ProductResponse } from './inventory.types'

const adjustSchema = z.object({
  cambio: z
    .number({ invalid_type_error: 'Ingresa un número' })
    .int('Debe ser entero')
    .refine((v) => v !== 0, 'El cambio no puede ser 0'),
  motivoAuditoria: z.string().min(5, 'Mínimo 5 caracteres').max(200, 'Máximo 200 caracteres'),
})

type AdjustFormData = z.infer<typeof adjustSchema>

interface AdjustInventoryDialogProps {
  product: ProductResponse | null
  onClose: () => void
}

export function AdjustInventoryDialog({ product, onClose }: AdjustInventoryDialogProps) {
  const mutation = useAdjustInventoryMutation()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AdjustFormData>({
    resolver: zodResolver(adjustSchema),
    defaultValues: { cambio: 0, motivoAuditoria: '' },
  })

  useEffect(() => {
    if (product) reset({ cambio: 0, motivoAuditoria: '' })
  }, [product, reset])

  const onSubmit = (formData: AdjustFormData) => {
    if (!product) return
    mutation.mutate(
      {
        productId: product.productId,
        cambio: formData.cambio,
        motivoAuditoria: formData.motivoAuditoria,
      },
      {
        onSuccess: () => {
          onClose()
          mutation.reset()
        },
      },
    )
  }

  return (
    <Dialog open={!!product} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Ajustar inventario</DialogTitle>
      <Divider />
      <DialogContent>
        <Stack spacing={3} sx={{ pt: 1 }}>
          {product && (
            <Stack>
              <Typography variant="subtitle1" fontWeight={600}>{product.descripcion}</Typography>
              <Typography variant="body2" color="text.secondary">
                Stock actual: <strong>{product.cantidadActual}</strong>
              </Typography>
            </Stack>
          )}

          {mutation.isError && (
            <Alert severity="error">
              {mutation.error instanceof Error ? mutation.error.message : 'Error al ajustar'}
            </Alert>
          )}

          <TextField
            label="Cambio (positivo = entrada, negativo = salida)"
            type="number"
            fullWidth
            error={!!errors.cambio}
            helperText={errors.cambio?.message}
            {...register('cambio', { valueAsNumber: true })}
          />

          <TextField
            label="Motivo / Auditoría"
            multiline
            rows={2}
            fullWidth
            error={!!errors.motivoAuditoria}
            helperText={errors.motivoAuditoria?.message}
            {...register('motivoAuditoria')}
          />
        </Stack>
      </DialogContent>
      <Divider />
      <DialogActions sx={{ p: 2, gap: 1 }}>
        <Button onClick={onClose} disabled={mutation.isPending}>
          Cancelar
        </Button>
        <Button
          variant="contained"
          onClick={handleSubmit(onSubmit)}
          disabled={mutation.isPending}
          startIcon={mutation.isPending ? <CircularProgress size={16} color="inherit" /> : undefined}
        >
          {mutation.isPending ? 'Guardando...' : 'Aplicar ajuste'}
        </Button>
      </DialogActions>
    </Dialog>
  )
}
