import type { PropsWithChildren } from 'react'
import { Box, Typography } from '@mui/material'

interface PageContainerProps extends PropsWithChildren {
  title: string
  description?: string
}

export function PageContainer({ title, description, children }: PageContainerProps) {
  return (
    <Box sx={{ p: { xs: 2, md: 4 } }}>
      <Typography variant="h3" gutterBottom>
        {title}
      </Typography>
      {description ? (
        <Typography color="text.secondary" sx={{ mb: 3 }}>
          {description}
        </Typography>
      ) : null}
      {children}
    </Box>
  )
}
