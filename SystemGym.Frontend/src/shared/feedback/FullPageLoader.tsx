import { Box, CircularProgress, Typography } from '@mui/material'

interface FullPageLoaderProps {
  label?: string
}

export function FullPageLoader({ label = 'Cargando' }: FullPageLoaderProps) {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'column',
        gap: 2,
      }}
    >
      <CircularProgress />
      <Typography variant="body1">{label}</Typography>
    </Box>
  )
}
