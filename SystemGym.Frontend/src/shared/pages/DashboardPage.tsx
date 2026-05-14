import GroupIcon from '@mui/icons-material/Group'
import InventoryIcon from '@mui/icons-material/Inventory'
import ReceiptIcon from '@mui/icons-material/Receipt'
import StyleIcon from '@mui/icons-material/Style'
import { Box, Grid, Paper, Skeleton, Stack, Typography } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { useDashboardMetrics } from '../../modules/dashboard/useDashboardMetrics'
import { PageContainer } from '../layout/PageContainer'

interface MetricCardProps {
  label: string
  value: string | number
  sub?: string
  icon: React.ReactNode
  to?: string
  loading?: boolean
}

function MetricCard({ label, value, sub, icon, to, loading }: MetricCardProps) {
  const navigate = useNavigate()

  return (
    <Paper
      elevation={0}
      onClick={to ? () => navigate(to) : undefined}
      sx={{
        p: 3,
        borderRadius: 5,
        border: '1px solid',
        borderColor: 'divider',
        cursor: to ? 'pointer' : 'default',
        transition: 'box-shadow 0.2s',
        '&:hover': to ? { boxShadow: 4 } : {},
      }}
    >
      <Stack direction="row" spacing={2} alignItems="flex-start">
        <Box
          sx={{
            width: 48,
            height: 48,
            borderRadius: 3,
            bgcolor: 'primary.main',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: 'white',
            flexShrink: 0,
          }}
        >
          {icon}
        </Box>
        <Stack spacing={0.5} sx={{ flex: 1 }}>
          <Typography variant="body2" color="text.secondary">{label}</Typography>
          {loading ? (
            <Skeleton width={80} height={36} />
          ) : (
            <Typography variant="h4" fontWeight={700} lineHeight={1}>
              {value}
            </Typography>
          )}
          {sub && !loading && (
            <Typography variant="caption" color="text.secondary">{sub}</Typography>
          )}
        </Stack>
      </Stack>
    </Paper>
  )
}

function formatCOP(value: number) {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
    notation: value >= 1_000_000 ? 'compact' : 'standard',
  }).format(value)
}

export function DashboardPage() {
  const metrics = useDashboardMetrics()
  const today = new Date().toLocaleDateString('es-CO', { dateStyle: 'long' })

  return (
    <PageContainer title="Dashboard" description="Resumen operativo del gimnasio">
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            label="Clientes registrados"
            value={metrics.totalClients}
            icon={<GroupIcon />}
            to="/clients"
            loading={metrics.isLoading}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            label="Planes disponibles"
            value={metrics.totalPlans}
            icon={<StyleIcon />}
            to="/plans"
            loading={metrics.isLoading}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            label="Total suscripciones"
            value={metrics.totalSubscriptions}
            icon={<ReceiptIcon />}
            to="/subscriptions"
            loading={metrics.isLoading}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            label="Suscripciones este mes"
            value={metrics.subscriptionsThisMonth}
            icon={<InventoryIcon />}
            loading={metrics.isLoading}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            label="Ventas hoy"
            value={metrics.salesToday}
            sub={today}
            icon={<ReceiptIcon />}
            to="/sales"
            loading={metrics.isLoading}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, xl: 3 }}>
          <MetricCard
            label="Recaudo hoy"
            value={formatCOP(metrics.salesAmountToday)}
            sub={today}
            icon={<InventoryIcon />}
            to="/sales"
            loading={metrics.isLoading}
          />
        </Grid>
      </Grid>
    </PageContainer>
  )
}
