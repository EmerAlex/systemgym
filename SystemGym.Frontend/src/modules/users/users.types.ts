export interface UserResponse {
  userId: string
  username: string
  role: 'Admin' | 'Standard'
  habilitado: boolean
  createdAt: string
  updatedAt: string
  lastLogin?: string
}

export interface UsersListResponse {
  data: UserResponse[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface CreateUserRequest {
  username: string
  password: string
  role: 'Admin' | 'Standard'
}

export interface UpdateUserRequest {
  role: 'Admin' | 'Standard'
  habilitado: boolean
}
