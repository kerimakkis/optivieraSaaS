// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-10-24',

  devtools: { enabled: true },

  modules: [
    '@pinia/nuxt',
    'nuxt-primevue'
  ],

  primevue: {
    options: {
      theme: 'aura',
      ripple: true
    },
    components: {
      include: ['Button', 'InputText', 'Password', 'Card', 'DataTable', 'Column', 'Dialog', 'Toast', 'Toolbar', 'Menu', 'Badge', 'Avatar', 'Dropdown', 'Calendar', 'Textarea', 'Tag', 'ProgressSpinner']
    }
  },

  css: [
    'primeicons/primeicons.css',
    '~/assets/css/main.scss'
  ],

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5000/api'
    }
  },

  app: {
    head: {
      title: 'Optiviera - Ticket Management SaaS',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Modern ticket management system for businesses' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },

  typescript: {
    strict: true,
    typeCheck: false
  },

  nitro: {
    preset: 'netlify'
  }
})
