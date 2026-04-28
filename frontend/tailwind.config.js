/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  important: '#root',
  corePlugins: { preflight: false },
  theme: { extend: {} },
  plugins: [],
}
