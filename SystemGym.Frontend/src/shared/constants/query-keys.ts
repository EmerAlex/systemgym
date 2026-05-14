export const queryKeys = {
  dashboard: ['dashboard'] as const,
  clients: ['clients'] as const,
  plans: ['plans'] as const,
  subscriptions: (clientId?: string) => ['subscriptions', clientId] as const,
  allSubscriptions: (page?: number, tipoDocumento?: string, numeroDocumento?: string) => ['subscriptions', 'all', page, tipoDocumento, numeroDocumento] as const,
  sales: ['sales'] as const,
  inventoryLogs: ['inventory-logs'] as const,
}
