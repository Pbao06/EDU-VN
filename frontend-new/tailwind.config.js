/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      // Phần này được để trống vì màu sắc, shadow, và các giá trị theme khác
      // đã được định nghĩa trong file src/app/globals.css
    },
  },
  plugins: [],
};
