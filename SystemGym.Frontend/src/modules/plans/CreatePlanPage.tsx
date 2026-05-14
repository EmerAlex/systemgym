import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import {
  Alert,
  Button,
  CircularProgress,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  TextField,
} from '@mui/material'
import { Controller, useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { PageContainer } from '../../shared/layout/PageContainer'
import { useCreatePlanMutation } from './useCreatePlanMutation'

const TIPOS_PERIODO = ['Dia', 'Mes'] as const

const PERIODO_LABELS: Record<string, string> = {
  Dia: 'Día',
  Mes: 'Mes',
}

const planSchema = z.object({
  descripcion: z.string().min(3, 'Mínimo 3 caracteres').max(200, 'Máximo 200 caracteres'),
  tipoPeriodo: z.enum(TIPOS_PERIODO, { errorMap: () => ({ message: 'Selecciona Día o Mes' }) }),
  cantidadIntervalosPeriodo: z.coerce
    .number({ invalid_type_error: 'Debe ser un número' })
    .int()
    .min(1, 'Mínimo 1')
    .max(999, 'Máximo 999'),
  valor: z.coerce
    .number({ invalid_type_error: 'Debe ser un número' })
    .min(0, 'No puede ser negativo'),
})

type PlanFormData = z.infer<typeof planSchema>

export function CreatePlanPage() {
  const navigate = useNavigate()
  const mutation = useCreatePlanMutation()

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<PlanFormData>({
    resolver: zodResolver(planSchema),
    defaultValues: { tipoPeriodo: 'Mes', cantidadIntervalosPeriodo: 1, valor: 0 },
  })

  const onSubmit = (data: PlanFormData) => {
    mutation.mutate({
      descripcion: data.descripcion,
      tipoPeriodo: data.tipoPeriodo,
      cantidadIntervalosPeriodo: data.cantidadIntervalosPeriodo,
      valor: data.valor,
    })
  }

  return (
    <PageContainer title="Nuevo plan" description="Crea un plan de suscripción">
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/plans')} sx={{ mb: 3 }}>
        Volver a planes
      </Button>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 600 }}>
        <Stack spacing={3} component="form" onSubmit={handleSubmit(onSubmit)}>
          {mutation.isError && (
            <Alert severity="error">
              {mutation.error instanceof Error ? mutation.error.message : 'Error al crear plan'}
            </Alert>
          )}

          <TextField
            label="Descripción"
            fullWidth
            error={!!errors.descripcion}
            helperText={errors.descripcion?.message}
            {...register('descripcion')}
          />

          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <FormControl fullWidth error={!!errors.tipoPeriodo}>
              <InputLabel id="tipo-periodo-label">Tipo de período</InputLabel>
              <Controller
                name="tipoPeriodo"
                control={control}
                render={({ field }) => (
                  <Select labelId="tipo-periodo-label" label="Tipo de período" {...field}>
                    {TIPOS_PERIODO.map((t) => (
                      <MenuItem key={t} value={t}>{PERIODO_LABELS[t]}</MenuItem>
                    ))}
                  </Select>
                )}
              />
            </FormControl>

            <TextField
              label="Cantidad de períodos"
              type="number"
              fullWidth
              inputProps={{ min: 1, max: 999 }}
              error={!!errors.cantidadIntervalosPeriodo}
              helperText={errors.cantidadIntervalosPeriodo?.message}
              {...register('cantidadIntervalosPeriodo')}
            />
          </Stack>

          <TextField
            label="Valor (COP)"
            type="number"
            fullWidth
            inputProps={{ min: 0, step: 1000 }}
            error={!!errors.valor}
            helperText={errors.valor?.message}
            {...register('valor')}
          />

          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={mutation.isPending}
            startIcon={mutation.isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
            sx={{ alignSelf: 'flex-start' }}
          >
            {mutation.isPending ? 'Guardando...' : 'Crear plan'}
          </Button>
        </Stack>
      </Paper>
    </PageContainer>
  )
}
