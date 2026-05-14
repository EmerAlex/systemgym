import { zodResolver } from '@hookform/resolvers/zod'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import {
  Alert,
  Button,
  Checkbox,
  CircularProgress,
  FormControl,
  FormControlLabel,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  TextField,
} from '@mui/material'
import { Controller, useForm } from 'react-hook-form'
import { useNavigate, useParams } from 'react-router-dom'
import { z } from 'zod'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import { usePlansQuery } from '../plans/usePlansQuery'
import { useCreateSubscriptionMutation } from './subscriptions-mutations'

const subSchema = z.object({
  planId: z.string().min(1, 'Selecciona un plan'),
  inicioVigencia: z.string().min(1, 'Selecciona la fecha de inicio'),
  tieneExpiracion: z.boolean(),
})

type SubFormData = z.infer<typeof subSchema>

export function CreateSubscriptionPage() {
  const { clientId } = useParams<{ clientId: string }>()
  const navigate = useNavigate()
  const plansQuery = usePlansQuery()
  const mutation = useCreateSubscriptionMutation(clientId ?? '')

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<SubFormData>({
    resolver: zodResolver(subSchema),
    defaultValues: {
      tieneExpiracion: true,
      inicioVigencia: new Date().toISOString().substring(0, 10),
    },
  })

  const onSubmit = (formData: SubFormData) => {
    mutation.mutate({
      clientId: clientId!,
      planId: formData.planId,
      inicioVigencia: new Date(formData.inicioVigencia).toISOString(),
      tieneExpiracion: formData.tieneExpiracion,
    })
  }

  if (plansQuery.isLoading) return <FullPageLoader label="Cargando planes..." />
  if (plansQuery.isError) return (
    <ErrorState message="No se pudieron cargar los planes" onRetry={() => plansQuery.refetch()} />
  )

  const activePlans = plansQuery.data?.data.filter((p) => p.habilitado) ?? []

  return (
    <PageContainer title="Nueva suscripción" description="Asocia un plan al cliente">
      <Button
        startIcon={<ArrowBackIcon />}
        onClick={() => navigate(`/clients/${clientId}/subscriptions`)}
        sx={{ mb: 3 }}
      >
        Volver a suscripciones
      </Button>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 520 }}>
        <Stack spacing={3} component="form" onSubmit={handleSubmit(onSubmit)}>
          {mutation.isError && (
            <Alert severity="error">
              {mutation.error instanceof Error ? mutation.error.message : 'Error al crear suscripción'}
            </Alert>
          )}

          <FormControl fullWidth error={!!errors.planId}>
            <InputLabel id="plan-label">Plan</InputLabel>
            <Controller
              name="planId"
              control={control}
              render={({ field }) => (
                <Select labelId="plan-label" label="Plan" {...field}>
                  {activePlans.map((p) => (
                    <MenuItem key={p.planId} value={p.planId}>
                      {p.descripcion} — {new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 }).format(p.valor)}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
          </FormControl>

          <TextField
            label="Fecha de inicio"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            error={!!errors.inicioVigencia}
            helperText={errors.inicioVigencia?.message}
            {...register('inicioVigencia')}
          />

          <Controller
            name="tieneExpiracion"
            control={control}
            render={({ field }) => (
              <FormControlLabel
                control={<Checkbox checked={field.value} onChange={field.onChange} />}
                label="Tiene fecha de expiración"
              />
            )}
          />

          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={mutation.isPending}
            startIcon={mutation.isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
            sx={{ alignSelf: 'flex-start' }}
          >
            {mutation.isPending ? 'Guardando...' : 'Crear suscripción'}
          </Button>
        </Stack>
      </Paper>
    </PageContainer>
  )
}
