import { AppBootstrap } from './AppBootstrap'
import { AppProviders } from './providers/AppProviders'

export function App() {
  return (
    <AppProviders>
      <AppBootstrap />
    </AppProviders>
  )
}
