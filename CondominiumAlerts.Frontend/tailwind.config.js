/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
    "./node_modules/primeng/**/*.{html,ts}"
  ],
  theme: {
    extend: {},
  },
  theme: {
    extend: {
      spacing: {
        '19': '7.90rem',
      },
    },
  },
  plugins: [require('tailwindcss-primeui')]
}


