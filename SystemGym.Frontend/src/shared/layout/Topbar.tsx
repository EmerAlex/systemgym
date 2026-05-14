import { AppBar, Box, Button, Stack, Toolbar, Typography } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../../app/store/hooks'
import { clearSession as clearSessionAction } from '../../app/store/slices/authSlice'
import { clearSession as clearLocalStorage } from '../../modules/auth/session-storage'

export function Topbar() {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const session = useAppSelector((state) => state.auth.session)

  const handleLogout = () => {
    clearLocalStorage()
    dispatch(clearSessionAction())
    navigate('/login', { replace: true })
  }

  return (
    <AppBar position="sticky" color="transparent" elevation={0} sx={{ borderBottom: '1px solid', borderColor: 'divider' }}>
      <Toolbar sx={{ justifyContent: 'space-between', bgcolor: 'rgba(255,255,255,0.95)', backdropFilter: 'blur(12px)' }}>
        <Box>
          <Typography variant="h6">Panel Operativo</Typography>
          <Typography variant="body2" color="text.secondary">
            {session?.username ?? 'Invitado'} · {session?.role ?? 'Sin sesión'}
          </Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" onClick={handleLogout}>
            Cerrar sesión
          </Button>
        </Stack>
      </Toolbar>
    </AppBar>
  )
}
