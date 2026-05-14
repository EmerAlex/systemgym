import AddIcon from '@mui/icons-material/Add'
import { Button, Divider, Stack, Tab, Tabs, Typography } from '@mui/material'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { EmptyState } from '../../shared/feedback/EmptyState'
import { ErrorState } from '../../shared/feedback/ErrorState'
import { FullPageLoader } from '../../shared/feedback/FullPageLoader'
import { PageContainer } from '../../shared/layout/PageContainer'
import { AdjustInventoryDialog } from './AdjustInventoryDialog'
import { InventoryLogsTable } from './InventoryLogsTable'
import { useInventoryLogsQuery, useProductsQuery } from './inventory-hooks'
import type { ProductResponse } from './inventory.types'
import { ProductsTable } from './ProductsTable'

export function InventoryPage() {
  const navigate = useNavigate()
  const [tab, setTab] = useState(0)
  const [selectedProduct, setSelectedProduct] = useState<ProductResponse | null>(null)
  const [logsPage, setLogsPage] = useState(0)
  const [logsPageSize, setLogsPageSize] = useState(10)

  const productsQuery = useProductsQuery()
  const logsQuery = useInventoryLogsQuery(logsPage, logsPageSize)

  if (productsQuery.isLoading) return <FullPageLoader label="Cargando inventario..." />
  if (productsQuery.isError) return (
    <ErrorState
      message={productsQuery.error instanceof Error ? productsQuery.error.message : 'Error al cargar productos'}
      onRetry={() => productsQuery.refetch()}
    />
  )

  return (
    <PageContainer title="Inventario" description="Gestión de productos y movimientos de stock">
      <Button
        variant="contained"
        startIcon={<AddIcon />}
        onClick={() => navigate('/inventory/products/new')}
        sx={{ mb: 3, display: tab === 0 ? 'inline-flex' : 'none' }}
      >
        Nuevo producto
      </Button>

      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label={`Productos (${productsQuery.data?.totalCount ?? 0})`} />
        <Tab label="Movimientos" />
      </Tabs>
      <Divider sx={{ mb: 3 }} />

      {tab === 0 && (
        <>
          {productsQuery.data && productsQuery.data.data.length === 0 ? (
            <EmptyState
              title="Sin productos"
              description="No hay productos registrados en el inventario."
              action={
                <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/inventory/products/new')}>
                  Agregar primer producto
                </Button>
              }
            />
          ) : (
            <ProductsTable
              rows={productsQuery.data?.data ?? []}
              onAdjust={setSelectedProduct}
            />
          )}
        </>
      )}

      {tab === 1 && (
        <>
          {logsQuery.isLoading && (
            <Stack alignItems="center" py={4}>
              <Typography color="text.secondary">Cargando movimientos...</Typography>
            </Stack>
          )}
          {logsQuery.isError && (
            <ErrorState
              message="Error al cargar movimientos"
              onRetry={() => logsQuery.refetch()}
            />
          )}
          {logsQuery.data && logsQuery.data.data.length === 0 && (
            <EmptyState title="Sin movimientos" description="Los movimientos se registran automáticamente al ajustar el inventario de un producto." />
          )}
          {logsQuery.data && logsQuery.data.data.length > 0 && (
            <InventoryLogsTable
              rows={logsQuery.data.data}
              totalCount={logsQuery.data.totalCount}
              page={logsPage}
              pageSize={logsPageSize}
              onPageChange={setLogsPage}
              onPageSizeChange={(s) => { setLogsPageSize(s); setLogsPage(0) }}
            />
          )}
        </>
      )}

      <AdjustInventoryDialog
        product={selectedProduct}
        onClose={() => setSelectedProduct(null)}
      />
    </PageContainer>
  )
}
