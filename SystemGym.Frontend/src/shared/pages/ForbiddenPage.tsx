import { Box, Button, Stack, Typography } from '@mui/material'
import { Link as RouterLink } from 'react-router-dom'

export function ForbiddenPage() {
  return (
    <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center', p: 3 }}>
      <Stack spacing={2} sx={{ maxWidth: 420 }}>
        <Typography variant="h2">403</Typography>
        <Typography variant="h5">Acceso denegado</Typography>
        <Typography color="text.secondary">
          Tu sesión no tiene permisos para acceder a este recurso.
        </Typography>
        <Button component={RouterLink} to="/dashboard" variant="contained">
          Volver al dashboard
        </Button>
      </Stack>
    </Box>
  )
}
