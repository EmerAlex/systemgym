import { Box, List, ListItemButton, ListItemText, Stack, Typography } from '@mui/material'
import { NavLink, useLocation } from 'react-router-dom'
import { useAppSelector } from '../../app/store/hooks'

const commonItems = [
  { label: 'Clientes', to: '/clients' },
  { label: 'Planes', to: '/plans' },
  { label: 'Suscripciones', to: '/subscriptions' },
  { label: 'Ventas', to: '/sales' },
]

const adminOnlyItems = [
  { label: 'Dashboard', to: '/dashboard' },
  { label: 'Usuarios', to: '/admin/users' },
  { label: 'Inventario', to: '/inventory' },
]

export function SidebarNav() {
  const role = useAppSelector((state) => state.auth.session?.role)
  const location = useLocation()
  const items = role === 'Admin' ? [...commonItems, ...adminOnlyItems] : commonItems

  const isActive = (itemPath: string) => {
    return location.pathname === itemPath || location.pathname.startsWith(itemPath + '/')
  }

  return (
    <Box
      sx={{
        width: 280,
        minHeight: '100vh',
        px: 2,
        py: 3,
        borderRight: '2px solid',
        borderColor: 'primary.main',
        bgcolor: '#FFFFFF',
        backdropFilter: 'blur(8px)',
      }}
    >
      <Stack spacing={1.5} alignItems="center" sx={{ mb: 4, pb: 2, borderBottom: '1px solid', borderColor: 'divider' }}>
        <Box
          component="img"
          src="/logo.png"
          alt="Euphoria Logo"
          sx={{ height: 48, objectFit: 'contain' }}
        />
        <Typography variant="h6" fontWeight={700} textAlign="center" sx={{ color: 'primary.main' }}>EUPHORIA</Typography>
      </Stack>
      <List>
        {items.map((item) => {
          const active = isActive(item.to)
          return (
            <ListItemButton 
              key={item.to} 
              component={NavLink} 
              to={item.to}
              sx={{ 
                borderRadius: 2,
                mb: 0.75,
                pl: 2,
                transition: 'all 0.25s ease',
                borderLeft: active ? '4px solid #FF8C00' : '4px solid transparent',
                bgcolor: active ? 'rgba(255, 140, 0, 0.15)' : 'transparent',
                color: active ? 'primary.main' : 'text.primary',
                fontWeight: active ? 700 : 500,
                '&:hover': {
                  bgcolor: active ? 'rgba(255, 140, 0, 0.2)' : 'rgba(255, 140, 0, 0.08)',
                  color: 'primary.main',
                  transform: 'translateX(4px)',
                },
              }}
            >
              <ListItemText 
                primary={item.label}
                primaryTypographyProps={{
                  variant: active ? 'body1' : 'body2',
                  fontWeight: active ? 700 : 500,
                }}
              />
            </ListItemButton>
          )
        })}
      </List>
    </Box>
  )
}
