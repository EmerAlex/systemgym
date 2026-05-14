import { Box } from '@mui/material'
import { Outlet } from 'react-router-dom'
import { SidebarNav } from './SidebarNav'
import { Topbar } from './Topbar'

export function AppShell() {
  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', background: 'linear-gradient(135deg, #F5F5F5 0%, #FFFFFF 100%)' }}>
      <SidebarNav />
      <Box sx={{ flex: 1 }}>
        <Topbar />
        <Outlet />
      </Box>
    </Box>
  )
}
