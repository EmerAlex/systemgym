import { zodResolver } from '@hookform/resolvers/zod'
import { useEffect, useState } from 'react'
import { Alert, Box, Button, CircularProgress, Paper, Stack, TextField, Typography } from '@mui/material'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { useLoginMutation } from '../../modules/auth/useLoginMutation'

const loginSchema = z.object({
  username: z.string().min(1, 'El usuario es requerido'),
  password: z.string().min(1, 'La contraseña es requerida'),
})

type LoginFormData = z.infer<typeof loginSchema>

export function LoginPage() {
  const loginMutation = useLoginMutation()
  const [sessionExpiredMessage, setSessionExpiredMessage] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  })

  const onSubmit = (data: LoginFormData) => {
    loginMutation.mutate(data)
  }

  useEffect(() => {
    const message = window.sessionStorage.getItem('systemgym_auth_error')
    if (message) {
      setSessionExpiredMessage(message)
      window.sessionStorage.removeItem('systemgym_auth_error')
    }
  }, [])

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'grid',
        placeItems: 'center',
        background: 'linear-gradient(135deg, #FF8C00 0%, #CC7000 50%, #1a1a1a 100%)',
        p: 3,
      }}
    >
      <Paper sx={{ width: '100%', maxWidth: 460, p: 4, borderRadius: 6, boxShadow: '0 20px 60px rgba(0,0,0,0.3)' }} elevation={0}>
        <Stack spacing={3} component="form" onSubmit={handleSubmit(onSubmit)}>
          <Stack spacing={1} alignItems="center">
            <Box
              component="img"
              src="/logo.png"
              alt="Euphoria Logo"
              sx={{ height: 60, objectFit: 'contain' }}
            />
            <Typography variant="h4" fontWeight={700} textAlign="center">EUPHORIA</Typography>
            <Typography color="text.secondary" textAlign="center">Ingresa tus credenciales para continuar</Typography>
          </Stack>

          {sessionExpiredMessage && (
            <Alert severity="warning">{sessionExpiredMessage}</Alert>
          )}

          {loginMutation.isError && (
            <Alert severity="error">
              {loginMutation.error instanceof Error
                ? loginMutation.error.message
                : 'Error al iniciar sesión'}
            </Alert>
          )}

          <TextField
            label="Usuario"
            fullWidth
            autoComplete="username"
            autoFocus
            error={!!errors.username}
            helperText={errors.username?.message}
            {...register('username')}
          />

          <TextField
            label="Contraseña"
            type="password"
            fullWidth
            autoComplete="current-password"
            error={!!errors.password}
            helperText={errors.password?.message}
            {...register('password')}
          />

          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={loginMutation.isPending}
            startIcon={loginMutation.isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {loginMutation.isPending ? 'Ingresando...' : 'Ingresar'}
          </Button>
        </Stack>
      </Paper>
    </Box>
  )
}
