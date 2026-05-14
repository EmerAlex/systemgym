import type { PropsWithChildren } from 'react'
import { QueryProvider } from './QueryProvider'
import { StoreProvider } from './StoreProvider'
import { ThemeProvider } from './ThemeProvider'

export function AppProviders({ children }: PropsWithChildren) {
  return (
    <StoreProvider>
      <QueryProvider>
        <ThemeProvider>{children}</ThemeProvider>
      </QueryProvider>
    </StoreProvider>
  )
}
