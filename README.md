# Optiviera SaaS

**Multi-tenant IT Helpdesk & Ticketing Platform**

![Status](https://img.shields.io/badge/Status-Active%20Development-green)
![Platform](https://img.shields.io/badge/Platform-SaaS%20Web%20App-blue)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20Core%208.0-purple)
![Frontend](https://img.shields.io/badge/Frontend-Nuxt%203-00DC82)
![Database](https://img.shields.io/badge/Database-MySQL%208.0-4479A1)
![License](https://img.shields.io/badge/License-MIT-blue)

Modern, cloud-based helpdesk and ticketing system with **multi-tenant architecture**. Transformed from a desktop Electron application to a scalable SaaS platform.

---

## 🚀 Project Status

**Current Phase:** SaaS Platform - Phase 1 Complete ✅

### ✅ Completed (Phase 1)

**Backend (ASP.NET Core 8.0)**
- [x] Multi-tenant database architecture (Tenant, TenantId integration)
- [x] MySQL database migration (EF Core)
- [x] JWT authentication & authorization
- [x] REST API Controllers (Auth, Tickets, Comments, Priorities, Users)
- [x] Tenant Middleware (automatic context extraction)
- [x] Role-based access control (Admin, Manager, Employee)
- [x] Shared DTOs architecture
- [x] Swagger/OpenAPI documentation
- [x] CORS configuration
- [x] Comprehensive logging

**Frontend (Nuxt 3 + Vue 3)**
- [x] Nuxt 3 project setup with TypeScript
- [x] PrimeVue 4 UI library integration
- [x] Authentication system (Login/Register)
- [x] Pinia state management
- [x] Route middleware (auth, guest)
- [x] Protected dashboard
- [x] Type-safe API integration
- [x] Responsive design

### 🔄 In Progress (Phase 2)
- [ ] Tickets management UI (list, create, edit, delete)
- [ ] Comments UI component
- [ ] Priority management UI
- [ ] User management UI
- [ ] Dashboard analytics
- [ ] Dark mode support

---

## 📁 Project Structure

```
OptivieraSaaS/
├── backend/                    # ASP.NET Core 8.0 Web API
│   ├── Controllers/            # REST API Controllers
│   │   ├── AuthController.cs       # Authentication (register, login, me)
│   │   ├── TicketsController.cs    # Ticket CRUD with tenant isolation
│   │   ├── CommentsController.cs   # Comment management
│   │   ├── PrioritiesController.cs # Priority management (Admin)
│   │   └── UsersController.cs      # User management (Admin/Manager)
│   ├── DTOs/                   # Data Transfer Objects
│   │   ├── AuthDTOs.cs            # Login, Register, AuthResponse
│   │   ├── TicketDTOs.cs          # Ticket-related DTOs
│   │   ├── CommentDTOs.cs         # Comment DTOs
│   │   ├── PriorityDTOs.cs        # Priority DTOs
│   │   └── UserDTOs.cs            # User DTOs
│   ├── Models/                 # Domain Models
│   │   ├── Tenant.cs              # Multi-tenant model
│   │   ├── WaveUser.cs            # User with TenantId
│   │   ├── Ticket.cs              # Ticket with TenantId
│   │   ├── Comment.cs             # Comment with TenantId
│   │   ├── Priority.cs            # Priority with TenantId
│   │   └── Enums/                 # Enumerations (TicketStatus, Roles)
│   ├── Data/                   # Database Context
│   │   ├── ApplicationDbContext.cs
│   │   ├── ApplicationDbContextFactory.cs
│   │   └── SeedData.cs            # Demo data seeding
│   ├── Services/               # Business Logic
│   │   ├── AuthService.cs         # JWT token generation
│   │   ├── TicketService.cs       # Ticket operations
│   │   └── Interfaces/            # Service contracts
│   ├── Middleware/             # Custom Middleware
│   │   └── TenantMiddleware.cs    # Tenant context extraction
│   ├── Migrations/             # EF Core Migrations
│   ├── Program.cs              # Application Entry Point
│   ├── appsettings.json        # Configuration
│   ├── DATABASE_SETUP.md       # Database setup guide
│   └── .env.example            # Environment variables template
│
├── frontend/                   # Nuxt 3 + Vue 3 + PrimeVue
│   ├── pages/                  # File-based routing
│   │   ├── index.vue              # Landing page
│   │   ├── login.vue              # Login page
│   │   ├── register.vue           # Registration page
│   │   └── dashboard/
│   │       └── index.vue          # Dashboard home
│   ├── layouts/                # Layout templates
│   │   └── default.vue            # Default layout
│   ├── stores/                 # Pinia stores
│   │   └── auth.ts                # Authentication store
│   ├── middleware/             # Route middleware
│   │   ├── auth.ts                # Protected route guard
│   │   └── guest.ts               # Guest route guard
│   ├── components/             # Vue components
│   ├── composables/            # Composition API utilities
│   ├── types/                  # TypeScript definitions
│   │   └── index.ts               # All type definitions
│   ├── assets/                 # Styles and assets
│   │   └── css/
│   │       └── main.scss          # Global styles
│   ├── public/                 # Static files
│   ├── nuxt.config.ts          # Nuxt configuration
│   ├── package.json            # Dependencies
│   ├── tsconfig.json           # TypeScript config
│   ├── .env.example            # Environment variables template
│   └── README.md               # Frontend documentation
│
└── optiviera/                  # Legacy Desktop App (Archive - DO NOT MODIFY)
    └── [Original Electron project]
```

---

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0 Web API
- **Database:** MySQL 8.0
- **ORM:** Entity Framework Core with Pomelo MySQL provider
- **Authentication:** JWT Bearer Tokens (HS256)
- **Authorization:** Role-based (Admin, Manager, Employee)
- **API Documentation:** Swagger/OpenAPI (Swashbuckle)
- **Architecture:** Multi-Tenant SaaS with data isolation

### Frontend
- **Framework:** Nuxt 3.13 (Vue 3 Composition API)
- **UI Library:** PrimeVue 4.0 with Aura theme
- **State Management:** Pinia 2.1
- **Language:** TypeScript (strict mode)
- **HTTP Client:** Nuxt $fetch (built-in)
- **Icons:** PrimeIcons 7.0
- **Styling:** SCSS + PrimeVue theming
- **Deployment:** Netlify (static generation)

---

## 🏗️ Multi-Tenant Architecture

### Core Entities
All entities include `TenantId` for complete data isolation:

- **Tenant** - Company/Organization (root entity)
- **WaveUser** - Users (scoped to Tenant, includes roles)
- **Ticket** - Helpdesk tickets (scoped to Tenant)
- **Comment** - Ticket comments (scoped to Tenant)
- **Priority** - Priority levels (scoped to Tenant)

### Database Relationships
```
Tenant (1) ─┬─ (N) WaveUser
            ├─ (N) Ticket ──── (N) Comment
            └─ (N) Priority
```

### Tenant Isolation
- **Database Level:** TenantId foreign keys on all entities
- **Application Level:** TenantMiddleware extracts TenantId from JWT
- **Query Level:** All queries automatically filtered by TenantId
- **Index Level:** Composite indexes on (TenantId, Id) for performance

---

## 🔧 Getting Started

### Prerequisites
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL 8.0+** - [Installation Guide](backend/DATABASE_SETUP.md)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **npm** or **yarn**

### Quick Start

#### 1. Clone the repository
```bash
git clone git@github.com:kerimakkis/optivieraSaaS.git
cd OptivieraSaaS
```

#### 2. Backend Setup

```bash
cd backend

# Install MySQL and create database
mysql -u root -p -e "CREATE DATABASE optiviera_saas_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"

# Update connection string in appsettings.Development.json
# Set your MySQL password

# Apply migrations
dotnet ef database update

# Run backend API
dotnet run

# API will be available at:
# http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

#### 3. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Run development server
npm run dev

# Frontend will be available at:
# http://localhost:3000
```

#### 4. Login with Demo Account

After running migrations, a demo account is created:
- **Email:** admin@optiviera.com
- **Password:** Admin123!
- **Company:** Demo Company

---

## 📚 API Endpoints

### Authentication
```
POST   /api/auth/register       # Create new tenant & admin user
POST   /api/auth/login          # Get JWT token
GET    /api/auth/me             # Get current user info
```

### Tickets (Requires Auth)
```
GET    /api/tickets             # List all tickets (tenant-scoped)
GET    /api/tickets/{id}        # Get ticket details with comments
POST   /api/tickets             # Create new ticket
PUT    /api/tickets/{id}        # Update ticket
DELETE /api/tickets/{id}        # Delete ticket (Admin only)
```

### Comments (Requires Auth)
```
GET    /api/tickets/{ticketId}/comments       # List comments
GET    /api/tickets/{ticketId}/comments/{id}  # Get comment
POST   /api/tickets/{ticketId}/comments       # Add comment
PUT    /api/tickets/{ticketId}/comments/{id}  # Update own comment
DELETE /api/tickets/{ticketId}/comments/{id}  # Delete own comment
```

### Priorities (Requires Auth)
```
GET    /api/priorities          # List priorities (tenant-scoped)
GET    /api/priorities/{id}     # Get priority
POST   /api/priorities          # Create priority (Admin only)
PUT    /api/priorities/{id}     # Update priority (Admin only)
DELETE /api/priorities/{id}     # Delete priority (Admin only)
```

### Users (Requires Admin/Manager)
```
GET    /api/users               # List users in tenant
GET    /api/users/{id}          # Get user details
POST   /api/users               # Create user (Admin only, respects MaxUsers)
PUT    /api/users/{id}          # Update user (Admin only)
DELETE /api/users/{id}          # Delete user (Admin only)
```

---

## 🔐 Authentication & Authorization

### JWT Token Flow
1. **Register/Login** → Receive JWT token
2. **Store Token** → Frontend stores in localStorage
3. **API Requests** → Include in `Authorization: Bearer {token}` header
4. **Token Validation** → Backend validates and extracts claims
5. **Tenant Context** → TenantMiddleware extracts TenantId from token

### JWT Token Claims
```json
{
  "nameid": "user-id",
  "email": "user@company.com",
  "name": "John Doe",
  "TenantId": "123",
  "FirstName": "John",
  "LastName": "Doe",
  "role": "Admin"
}
```

### Roles & Permissions
- **Admin** - Full access to all resources, user management
- **Manager** - View users, manage tickets, manage priorities
- **Employee** - Create and view tickets, add comments

---

## 🎯 Roadmap

### Phase 1: Foundation ✅ (COMPLETED)
- [x] Multi-tenant backend architecture
- [x] MySQL database with migrations
- [x] JWT authentication & authorization
- [x] REST API controllers (Auth, Tickets, Comments, Priorities, Users)
- [x] Tenant middleware & data isolation
- [x] Frontend Nuxt 3 setup
- [x] Authentication UI (Login/Register)
- [x] Protected routing
- [x] State management with Pinia

### Phase 2: Core Features 🔄 (CURRENT)
- [ ] Tickets management UI
  - [ ] Tickets list with filters
  - [ ] Ticket detail view
  - [ ] Create ticket form
  - [ ] Update ticket
  - [ ] Ticket status workflow
- [ ] Comments UI
  - [ ] Comment list component
  - [ ] Add comment form
  - [ ] Real-time comment updates
- [ ] Priority management UI
- [ ] User management UI
- [ ] Dashboard with analytics

### Phase 3: Advanced Features
- [ ] Real-time notifications (SignalR)
- [ ] File attachments for tickets
- [ ] Email notifications (SendGrid/SMTP)
- [ ] Advanced search & filtering
- [ ] Reporting & analytics dashboard
- [ ] Audit logs
- [ ] Activity timeline
- [ ] Export to PDF/Excel
- [ ] Dark mode theme

### Phase 4: Enterprise Features
- [ ] Multi-language support (i18n)
- [ ] SLA management
- [ ] Custom fields
- [ ] Workflow automation
- [ ] API rate limiting
- [ ] Webhooks
- [ ] SSO integration (OAuth, SAML)
- [ ] Mobile app (Vue + Capacitor)

### Phase 5: Deployment & DevOps
- [ ] Backend deployment (AkkisHost/Azure)
- [ ] Frontend deployment (Netlify/Vercel)
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Production database setup
- [ ] Monitoring & logging (Application Insights)
- [ ] Backup strategy
- [ ] Performance optimization
- [ ] Security audit

---

## 🧪 Development

### Backend Development
```bash
cd backend

# Run in development mode
dotnet run

# Run with hot reload
dotnet watch run

# Build for production
dotnet build -c Release

# Run tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName
```

### Frontend Development
```bash
cd frontend

# Install dependencies
npm install

# Run dev server (http://localhost:3000)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Generate static site
npm run generate

# Type checking
npm run type-check
```

---

## 📝 Environment Variables

### Backend (`appsettings.Development.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=optiviera_saas_dev;Uid=root;Pwd=yourpassword;"
  },
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyMinimum32CharactersLong123456789!",
    "Issuer": "https://api.optiviera.com",
    "Audience": "https://app.optiviera.com",
    "ExpiryInDays": 7
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "https://app.optiviera.com"
    ]
  }
}
```

### Frontend (`.env`)
```env
NUXT_PUBLIC_API_BASE=http://localhost:5000/api
```

---

## 🚀 Deployment

### Backend (AkkisHost/Azure)
```bash
cd backend

# Publish for production
dotnet publish -c Release -o ./publish

# Deploy to server
# Upload publish folder to server
# Configure reverse proxy (nginx/IIS)
# Set production connection string
# Enable HTTPS
```

### Frontend (Netlify)
```bash
cd frontend

# Build static site
npm run generate

# Deploy to Netlify
# dist/ folder is ready for deployment
# Configure environment variables in Netlify
```

---

## 📖 Documentation

- [Backend API Documentation](backend/README.md)
- [Frontend Documentation](frontend/README.md)
- [Database Setup Guide](backend/DATABASE_SETUP.md)
- [API Reference](http://localhost:5000/swagger) (when backend is running)

---

## 🧪 Testing

### Demo Account
After seeding, use these credentials:
- **Email:** admin@optiviera.com
- **Password:** Admin123!
- **Role:** Admin
- **Tenant:** Demo Company

### API Testing
- **Swagger UI:** http://localhost:5000/swagger
- **Postman Collection:** (Coming soon)

---

## 🤝 Contributing

This project is currently under active development. Contributions are welcome!

### Development Workflow
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Commit Convention
We follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code style changes
- `refactor:` Code refactoring
- `test:` Test changes
- `chore:` Build/tooling changes

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Contact

**Kerim Akkış**
- GitHub: [@kerimakkis](https://github.com/kerimakkis)
- Email: support@akkistech.com
- Website: [akkistech.com](https://akkistech.com)

---

## 🙏 Acknowledgments

- Original Optiviera Desktop App (archived in `/optiviera/`)
- ASP.NET Core Team for excellent framework
- Nuxt & Vue.js teams for amazing frontend tools
- PrimeVue for beautiful UI components
- Open source community

---

## 📊 Project Stats

- **Lines of Code:** ~15,000+
- **API Endpoints:** 25+
- **Database Tables:** 10+
- **Frontend Pages:** 4+ (growing)
- **Commits:** 10+
- **Development Time:** Active

---

**Note:** This project is in active development. The original Desktop App is preserved in `optiviera/` directory for reference only. **DO NOT modify files in `optiviera/` directory.**

---

**Built with ❤️ using ASP.NET Core 8.0, Nuxt 3, and modern web technologies**
