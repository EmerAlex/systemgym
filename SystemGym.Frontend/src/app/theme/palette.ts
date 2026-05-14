export const palette = {
  // Naranja como color primario - escala cromática vibrante
  primary: {
    light: '#FFB84D',      // Naranja claro - uso en hover/disabled states
    main: '#FF8C00',       // Naranja vibrante Darkorange - color primario principal
    dark: '#CC7000',       // Naranja oscuro - uso en pressed/focus states
    contrastText: '#FFFFFF', // Texto sobre naranja
  },
  
  // Negro como color secundario - escala cromática neutral y profesional
  secondary: {
    light: '#424242',      // Gris oscuro - uso en elementos secundarios
    main: '#1a1a1a',       // Negro - color secundario principal
    dark: '#000000',       // Negro puro - máximo contraste
    contrastText: '#FFFFFF', // Texto sobre negro
  },

  // Escala de grises neutros para fondos y superficies
  background: {
    default: '#F5F5F5',    // Gris muy claro - fondo general
    paper: '#FFFFFF',      // Blanco puro - cards, papers, superficie
  },

  // Escala de texto con buen contraste sobre fondos
  text: {
    primary: '#1a1a1a',      // Negro - texto principal
    secondary: '#666666',    // Gris medio - texto secundario
    disabled: '#BDBDBD',     // Gris claro - texto deshabilitado
  },

  // Colores de estado y feedback
  success: {
    main: '#4CAF50',      // Verde - operaciones exitosas
    light: '#81C784',
    dark: '#388E3C',
  },
  warning: {
    main: '#FFC107',      // Ámbar - advertencias
    light: '#FFD54F',
    dark: '#FFA000',
  },
  error: {
    main: '#F44336',      // Rojo - errores
    light: '#EF5350',
    dark: '#D32F2F',
  },
  info: {
    main: '#2196F3',      // Azul - información
    light: '#64B5F6',
    dark: '#1976D2',
  },

  // Tonos adicionales para componentes específicos
  divider: '#E0E0E0',     // Líneas divisoras
  action: {
    active: '#FF8C00',    // Color activo (naranja primario)
    hover: 'rgba(255, 140, 0, 0.08)',
    selected: 'rgba(255, 140, 0, 0.12)',
    disabled: '#BDBDBD',
    disabledBackground: '#F5F5F5',
  },
}
