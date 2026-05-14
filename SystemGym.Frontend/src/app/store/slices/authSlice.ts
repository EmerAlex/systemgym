import { createSlice, type PayloadAction } from '@reduxjs/toolkit'
import type { AuthSession, UserRole } from '../../../shared/models/auth.types'

interface AuthState {
  session: AuthSession | null
  bootstrapCompleted: boolean
}

const initialState: AuthState = {
  session: null,
  bootstrapCompleted: false,
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setSession(state, action: PayloadAction<AuthSession>) {
      state.session = action.payload
    },
    clearSession(state) {
      state.session = null
    },
    setBootstrapCompleted(state, action: PayloadAction<boolean>) {
      state.bootstrapCompleted = action.payload
    },
    setRole(state, action: PayloadAction<UserRole>) {
      if (state.session) {
        state.session.role = action.payload
      }
    },
  },
})

export const { setSession, clearSession, setBootstrapCompleted, setRole } = authSlice.actions
export const authReducer = authSlice.reducer
