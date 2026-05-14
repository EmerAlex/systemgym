import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import {
  Alert,
  Button,
  CircularProgress,
  Paper,
  Stack,
  TextField,
} from '@mui/material'
import { useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { PageContainer } from '../../shared/layout/PageContainer'
import { useCreateProductMutation } from './useCreateProductMutation'

const productSchema = z.object({
  descripcion: z.string().min(3, 'Mínimo 3 caracteres').max(200, 'Máximo 200 caracteres'),
  valor: z.coerce
    .number({ invalid_type_error: 'Debe ser un número' })
    .min(0, 'No puede ser negativo'),
})

type ProductFormData = z.infer<typeof productSchema>

export function CreateProductPage() {
  const navigate = useNavigate()
  const mutation = useCreateProductMutation()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: { valor: 0 },
  })

  const onSubmit = (data: ProductFormData) => {
    mutation.mutate({ descripcion: data.descripcion, valor: data.valor })
  }

  return (
    <PageContainer title="Nuevo producto" description="Registra un producto en el inventario">
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/inventory')} sx={{ mb: 3 }}>
        Volver a inventario
      </Button>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 600 }}>
        <Stack spacing={3} component="form" onSubmit={handleSubmit(onSubmit)}>
          {mutation.isError && (
            <Alert severity="error">
              {mutation.error instanceof Error ? mutation.error.message : 'Error al crear producto'}
            </Alert>
          )}

          <TextField
            label="Descripción"
            fullWidth
            error={!!errors.descripcion}
            helperText={errors.descripcion?.message}
            {...register('descripcion')}
          />

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
            {mutation.isPending ? 'Guardando...' : 'Crear producto'}
          </Button>
        </Stack>
      </Paper>
    </PageContainer>
  )
}
