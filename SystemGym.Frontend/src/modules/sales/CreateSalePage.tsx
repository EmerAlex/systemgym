import React from 'react'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import {
  Alert,
  Button,
  Checkbox,
  CircularProgress,
  FormControl,
  FormControlLabel,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { Controller, useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery } from '@tanstack/react-query'
import { PageContainer } from '../../shared/layout/PageContainer'
import { useCreateSaleMutation } from './useCreateSaleMutation'
import { getClients } from '../clients/clients-api'
import { getProducts } from '../inventory/inventory-api'

const saleSchema = z.object({
  clientId: z.string().min(1, 'Selecciona un cliente'),
  productId: z.string().min(1, 'Selecciona un producto'),
  fechaVenta: z.string().min(1, 'Selecciona la fecha'),
  pagado: z.boolean(),
  medioPago: z.string().optional(),
  referencia: z.string().optional(),
})

type SaleFormData = z.infer<typeof saleSchema>

function todayISO() {
  return new Date().toISOString().split('T')[0]
}

export function CreateSalePage() {
  const navigate = useNavigate()
  const mutation = useCreateSaleMutation()

  const clientsQuery = useQuery({
    queryKey: ['clients-all'],
    queryFn: () => getClients({ pageNumber: 1, pageSize: 200 }),
    retry: 2,
  })

  const productsQuery = useQuery({
    queryKey: ['products-all'],
    queryFn: () => getProducts(1, 200),
    retry: 2,
  })

  const {
    register,
    handleSubmit,
    control,
    watch,
    formState: { errors },
    getValues,
  } = useForm<SaleFormData>({
    resolver: zodResolver(saleSchema),
    defaultValues: {
      clientId: '',
      productId: '',
      fechaVenta: todayISO(),
      pagado: false,
      medioPago: '',
      referencia: '',
    },
  })

  const pagado = watch('pagado')

  const onSubmit = (data: SaleFormData) => {
    console.log('📤 CreateSalePage.onSubmit:', {
      clientId: data.clientId,
      productId: data.productId,
      fechaVenta: data.fechaVenta,
      pagado: data.pagado,
    })

    mutation.mutate({
      clientId: data.clientId,
      productId: data.productId,
      fechaVenta: new Date(data.fechaVenta).toISOString(),
      pagado: data.pagado,
      medioPago: data.medioPago || undefined,
      referencia: data.referencia || undefined,
    })
  }

  const clients = clientsQuery.data?.data ?? []
  const products = productsQuery.data?.data ?? []

  // Log cuando datos carguen
  React.useEffect(() => {
    if (clients.length > 0) {
      console.log('✅ Clientes cargados:', clients.length, clients)
    }
  }, [clients])

  React.useEffect(() => {
    if (products.length > 0) {
      console.log('✅ Productos cargados:', products.length, products)
    }
  }, [products])

  return (
    <PageContainer title="Nueva venta" description="Registra la venta de un producto a un cliente">
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/sales')} sx={{ mb: 3 }}>
        Volver a ventas
      </Button>

      <Paper elevation={0} sx={{ p: 4, borderRadius: 4, maxWidth: 640 }}>
        <Stack spacing={3} component="form" onSubmit={handleSubmit(onSubmit)}>
          {mutation.isError && (
            <Alert severity="error">
              {mutation.error instanceof Error ? mutation.error.message : 'Error al registrar venta'}
            </Alert>
          )}

          {clientsQuery.isError && (
            <Alert severity="warning">
              ⚠️ Error cargando clientes: {clientsQuery.error instanceof Error ? clientsQuery.error.message : 'Error desconocido'}
            </Alert>
          )}

          {productsQuery.isError && (
            <Alert severity="warning">
              ⚠️ Error cargando productos: {productsQuery.error instanceof Error ? productsQuery.error.message : 'Error desconocido'}
            </Alert>
          )}

          {/* Cliente */}
          <FormControl fullWidth error={!!errors.clientId} disabled={clientsQuery.isError}>
            <InputLabel id="cliente-label">Cliente</InputLabel>
            <Controller
              name="clientId"
              control={control}
              render={({ field }) => (
                <Select
                  labelId="cliente-label"
                  label="Cliente"
                  {...field}
                  disabled={clientsQuery.isLoading || clientsQuery.isError || clients.length === 0}
                >
                  {clientsQuery.isLoading && (
                    <MenuItem disabled value="">Cargando clientes...</MenuItem>
                  )}
                  {clients.length === 0 && !clientsQuery.isLoading && (
                    <MenuItem disabled value="">No hay clientes disponibles</MenuItem>
                  )}
                  {clients.map((c) => (
                    <MenuItem key={c.clientId} value={c.clientId}>
                      {c.nombreCompleto} — {c.numeroDocumento}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
            {errors.clientId && (
              <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.5 }}>
                {errors.clientId.message}
              </Typography>
            )}
          </FormControl>

          {/* Producto */}
          <FormControl fullWidth error={!!errors.productId} disabled={productsQuery.isError}>
            <InputLabel id="producto-label">Producto</InputLabel>
            <Controller
              name="productId"
              control={control}
              render={({ field }) => (
                <Select
                  labelId="producto-label"
                  label="Producto"
                  {...field}
                  disabled={productsQuery.isLoading || productsQuery.isError || products.length === 0}
                >
                  {productsQuery.isLoading && (
                    <MenuItem disabled value="">Cargando productos...</MenuItem>
                  )}
                  {products.length === 0 && !productsQuery.isLoading && (
                    <MenuItem disabled value="">No hay productos disponibles</MenuItem>
                  )}
                  {products.map((p) => (
                    <MenuItem key={p.productId} value={p.productId}>
                      {p.descripcion} — {new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 }).format(p.valor)}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
            {errors.productId && (
              <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.5 }}>
                {errors.productId.message}
              </Typography>
            )}
          </FormControl>

          {/* Fecha */}
          <TextField
            label="Fecha de venta"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            error={!!errors.fechaVenta}
            helperText={errors.fechaVenta?.message}
            {...register('fechaVenta')}
          />

          {/* Pagado */}
          <Controller
            name="pagado"
            control={control}
            render={({ field }) => (
              <FormControlLabel
                control={<Checkbox {...field} checked={field.value} />}
                label="Marcar como pagada"
              />
            )}
          />

          {/* Medio de pago y referencia — solo si pagado */}
          {pagado && (
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <TextField
                label="Medio de pago"
                fullWidth
                placeholder="Efectivo, Transferencia..."
                {...register('medioPago')}
              />
              <TextField
                label="Referencia"
                fullWidth
                placeholder="N° transacción..."
                {...register('referencia')}
              />
            </Stack>
          )}

          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={mutation.isPending}
            startIcon={mutation.isPending ? <CircularProgress size={18} color="inherit" /> : undefined}
            sx={{ alignSelf: 'flex-start' }}
          >
            {mutation.isPending ? 'Guardando...' : 'Registrar venta'}
          </Button>
        </Stack>
      </Paper>
    </PageContainer>
  )
}
