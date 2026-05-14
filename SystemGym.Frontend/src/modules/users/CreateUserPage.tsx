import { Button, CircularProgress, FormControl, InputLabel, MenuItem, Select, Stack, TextField, Typography } from '@mui/material'
import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { PageContainer } from '../../shared/layout/PageContainer'
import { useCreateUserMutation } from './useCreateUserMutation'
import type { CreateUserRequest } from './users.types'

const schema = z.object({
  username: z.string().min(3, 'Mínimo 3 caracteres').max(50, 'Máximo 50 caracteres'),
  password: z.string().min(8, 'La contraseña debe tener mínimo 8 caracteres'),
  role: z.enum(['Admin', 'Standard']),
})

type FormData = z.infer<typeof schema>

export function CreateUserPage() {
  const navigate = useNavigate()
  const mutation = useCreateUserMutation()
  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      username: '',
      password: '',
      role: 'Standard',
    },
  })

  const role = watch('role')

  async function onSubmit(data: FormData) {
    try {
      await mutation.mutateAsync(data as CreateUserRequest)
      navigate('/admin/users', { replace: true })
    } catch (err) {
      console.error('Error creating user:', err)
    }
  }

  return (
    <PageContainer title="Crear Usuario" description="Registra un nuevo usuario en el sistema">
      <Stack
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        spacing={3}
        sx={{ maxWidth: 480 }}
      >
        <TextField
          label="Usuario"
          fullWidth
          placeholder="nombre_usuario"
          error={!!errors.username}
          helperText={errors.username?.message}
          {...register('username')}
        />

        <TextField
          label="Contraseña"
          type="password"
          fullWidth
          placeholder="Min. 8 caracteres"
          error={!!errors.password}
          helperText={errors.password?.message}
          {...register('password')}
        />

        <FormControl fullWidth>
          <InputLabel>Rol</InputLabel>
          <Select label="Rol" {...register('role')} value={role}>
            <MenuItem value="Standard">Estándar</MenuItem>
            <MenuItem value="Admin">Administrador</MenuItem>
          </Select>
        </FormControl>

        {role === 'Admin' && (
          <Typography variant="body2" color="warning.main" sx={{ p: 2, bgcolor: 'warning.light', borderRadius: 1 }}>
            ⚠️ Este usuario tendrá acceso total al sistema incluyendo gestión de usuarios e inventario.
          </Typography>
        )}

        <Stack direction="row" spacing={2} justifyContent="flex-end">
          <Button onClick={() => navigate('/admin/users')}>Cancelar</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={mutation.isPending}
            startIcon={mutation.isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {mutation.isPending ? 'Creando...' : 'Crear Usuario'}
          </Button>
        </Stack>

        {mutation.isError && (
          <Typography color="error" variant="body2">
            Error: {mutation.error instanceof Error ? mutation.error.message : 'Error al crear usuario'}
          </Typography>
        )}
      </Stack>
    </PageContainer>
  )
}
