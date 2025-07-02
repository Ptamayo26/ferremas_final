/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Colores personalizados para Ferremas
        ferremas: {
          primary: '#2D3142',     // Gris oscuro elegante
          'primary-dark': '#1B1E2A',  // Gris aún más oscuro
          secondary: '#E8A44B',   // Naranja cálido
          accent: '#D8973C',      // Naranja más oscuro
          success: '#4F9D69',     // Verde sobrio
          warning: '#E8A44B',     // Naranja para advertencias
          danger: '#D64045',      // Rojo sobrio
          background: '#F7F7F7',  // Fondo claro
          surface: '#FFFFFF',     // Superficie
          orange: {
            50: '#FFF7ED',
            100: '#FFEDD5',
            200: '#FED7AA',
            300: '#FDBA74',
            400: '#FB923C',
            500: '#E8A44B',
            600: '#D8973C',
            700: '#C2821F',
            800: '#9A6617',
            900: '#7C4F11',
          },
          green: {
            50: '#F0FDF4',
            100: '#DCFCE7',
            200: '#BBF7D0',
            300: '#86EFAC',
            400: '#4ADE80',
            500: '#4F9D69',
            600: '#3B8255',
            700: '#2C6A42',
            800: '#1F4D31',
            900: '#143321',
          },
          gray: {
            50: '#F7F7F7',
            100: '#E1E1E1',
            200: '#CFCFCF',
            300: '#B1B1B1',
            400: '#9E9E9E',
            500: '#7E7E7E',
            600: '#626262',
            700: '#515151',
            800: '#3B3B3B',
            900: '#222222',
          }
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      boxShadow: {
        'custom': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        'elevated': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
        'glow': '0 0 15px rgba(232, 164, 75, 0.3)',
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
        'gradient-pattern': 'linear-gradient(45deg, var(--tw-gradient-stops))',
      }
    },
  },
  plugins: [],
}