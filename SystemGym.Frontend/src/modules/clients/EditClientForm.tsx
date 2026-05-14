import { zodResolver } from '@hookform/resolvers/zod'
import {
  Alert,
  Button,
  CircularProgress,
  FormControl,
  FormControlLabel,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Switch,
  TextField,
} from '@mui/material'
import { useForm, Controller } from 'react-hook-form'
import { z } from 'zod'
import type { UpdateClientRequest } from './clients.types'

const TIPOS_DOCUMENTO = ['CC', 'CE', 'TI', 'PP', 'NIT'] as const

const clientSchema = z.object({
  tipoDocumento: z.enum(TIPOS_DOCUMENTO, { errorMap: () => ({ message: 'Selecciona un tipo de documento' }) }),
  numeroDocumento: z
    .string()
    .min(5, 'Mínimo 5 caracteres')
    .max(20, 'Máximo 20 caracteres')
    .regex(/^\d+$/, 'Solo números'),
  nombreCompleto: z.string().min(3, 'Mínimo 3 caracteres').max(100, 'Máximo 100 caracteres'),
  celular: z
    .string()
    .regex(/^\d{10}$/, 'Debe tener exactamente 10 dígitos')
    .optional()
    .or(z.literal('')),
  habilitado: z.boolean(),
})

type ClientFormData = z.infer<typeof clientSchema>

interface EditClientFormProps {
  initialData: {
    tipoDocumento: string
    numeroDocumento: string
    nombreCompleto: string
    celular?: string
    habilitado: boolean
  }
  onSubmit: (data: UpdateClientRequest) => void
  isPending: boolean
  error?: string
}

export function EditClientForm({ initialData, onSubmit, isPending, error }: EditClientFormProps) {
  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<ClientFormData>({
    resolver: zodResolver(clientSchema),
    defaultValues: {
      tipoDocumento: initialData.tipoDocumento as typeof TIPOS_DOCUMENTO[number],
      numeroDocumento: initialData.numeroDocumento,
      nombreCompleto: initialData.nombreCompleto,
      celular: initialData.celular || '',
      habilitado: initialData.habilitado,
    },
  })

  const handleFormSubmit = (data: ClientFormData) => {
    onSubmit({
      tipoDocumento: data.tipoDocumento,
      numeroDocumento: data.numeroDocumento,
      nombreCompleto: data.nombreCompleto,
      celular: data.celular || undefined,
      habilitado: data.habilitado,
    })
  }

  return (
    <Stack spacing={3} component="form" onSubmit={handleSubmit(handleFormSubmit)}>
      {error && <Alert severity="error">{error}</Alert>}

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
        <FormControl sx={{ minWidth: 140 }} error={!!errors.tipoDocumento}>
          <InputLabel id="tipo-doc-label">Tipo</InputLabel>
          <Controller
            name="tipoDocumento"
            control={control}
            render={({ field }) => (
              <Select labelId="tipo-doc-label" label="Tipo" {...field}>
                {TIPOS_DOCUMENTO.map((t) => (
                  <MenuItem key={t} value={t}>{t}</MenuItem>
                ))}
              </Select>
            )}
          />
        </FormControl>
        <TextField
          label="Número de documento"
          fullWidth
          error={!!errors.numeroDocumento}
          helperText={errors.numeroDocumento?.message}
          {...register('numeroDocumento')}
        />
      </Stack>

      <TextField
        label="Nombre completo"
        fullWidth
        error={!!errors.nombreCompleto}
        helperText={errors.nombreCompleto?.message}
        {...register('nombreCompleto')}
      />

      <TextField
        label="Celular (opcional)"
        fullWidth
        placeholder="3001234567"
        error={!!errors.celular}
        helperText={errors.celular?.message}
        {...register('celular')}
      />

      <Controller
        name="habilitado"
        control={control}
        render={({ field }) => (
          <FormControlLabel
            control={<Switch {...field} checked={field.value} />}
            label={field.value ? 'Cliente activo' : 'Cliente inactivo'}
          />
        )}
      />

      <Button
        type="submit"
        variant="contained"
        size="large"
        disabled={isPending}
        startIcon={isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
        sx={{ alignSelf: 'flex-start' }}
      >
        {isPending ? 'Guardando...' : 'Guardar cambios'}
      </Button>
    </Stack>
  )
}
