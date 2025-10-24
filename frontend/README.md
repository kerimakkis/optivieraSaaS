# Optiviera SaaS Frontend

Modern ticket management system built with Nuxt 3, Vue 3, and PrimeVue.

## Tech Stack

- **Nuxt 3** - The Intuitive Vue Framework
- **Vue 3** - Progressive JavaScript Framework
- **PrimeVue 4** - Rich UI Component Library
- **Pinia** - State Management
- **TypeScript** - Type Safety

## Features

- Multi-tenant SaaS architecture
- JWT Authentication
- Role-based access control (Admin, Manager, Employee)
- Ticket management system
- Comment system
- Priority management
- User management
- Responsive design with PrimeVue

## Setup

```bash
# Install dependencies
npm install

# Development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

## Environment Variables

Create a `.env` file:

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000/api
```

## Project Structure

```
frontend/
├── assets/         # Styles, images
├── components/     # Vue components
├── composables/    # Composition API utilities
├── layouts/        # Layout templates
├── middleware/     # Route middleware
├── pages/          # File-based routing
├── stores/         # Pinia stores
├── types/          # TypeScript definitions
└── public/         # Static files
```

## Demo Account

Default demo credentials (after backend seeding):
- Email: admin@optiviera.com
- Password: Admin123!

## Deployment

This project is configured for Netlify deployment:

```bash
npm run generate
```

The `dist/` folder can be deployed to any static hosting service.
