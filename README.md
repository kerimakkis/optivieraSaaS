# Optiviera SaaS

**Multi-tenant IT Helpdesk & Ticketing Platform**

![Status](https://img.shields.io/badge/Status-In%20Development-yellow)
![Platform](https://img.shields.io/badge/Platform-SaaS%20Web%20App-blue)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20Core%208.0-purple)
![Frontend](https://img.shields.io/badge/Frontend-Nuxt%203-green)
![License](https://img.shields.io/badge/License-MIT-blue)

Optiviera is being transformed from a desktop Electron application to a modern SaaS platform with multi-tenant architecture.

---

## 🚀 Project Status

**Current Phase:** Desktop → SaaS Transformation (In Progress)

### ✅ Completed
- [x] Multi-tenant database models (Tenant, TenantId integration)
- [x] Backend Web API infrastructure (JWT, CORS, Swagger)
- [x] Project structure reorganization
- [x] SQLite → MySQL migration preparation

### 🔄 In Progress
- [ ] Backend packages update (MySQL, JWT)
- [ ] Auth Service (JWT token generation)
- [ ] Tenant Middleware
- [ ] Auth API Controllers (Login/Register)
- [ ] Tickets API endpoints
- [ ] Nuxt 3 Frontend setup

---

## 📁 Project Structure

```
optivieraSaaS/
├── backend/                    # ASP.NET Core Web API
│   ├── Controllers/            # API Controllers
│   ├── Models/                 # Domain Models (Tenant, Ticket, Comment, Priority, WaveUser)
│   ├── Data/                   # DbContext, SeedData
│   ├── Services/               # Business Logic
│   ├── Middleware/             # Tenant Middleware (upcoming)
│   └── Program.cs              # Application Entry Point
│
├── frontend/                   # Nuxt 3 + Vue 3 (upcoming)
│   └── [To be created]
│
└── Optiviera/                  # Legacy Desktop App (Archive)
    └── [Original Electron project]
```

---

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0 Web API
- **Database:** MySQL (migrating from SQLite)
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **API Documentation:** Swagger/OpenAPI
- **Architecture:** Multi-Tenant SaaS

### Frontend (Planned)
- **Framework:** Nuxt 3 + Vue 3
- **UI Library:** PrimeVue
- **State Management:** Pinia
- **HTTP Client:** Axios
- **Styling:** Tailwind CSS

---

## 🏗️ Multi-Tenant Architecture

### Core Entities
All entities include `TenantId` for data isolation:

- **Tenant** - Company/Organization
- **WaveUser** - Users (scoped to Tenant)
- **Ticket** - Helpdesk tickets (scoped to Tenant)
- **Comment** - Ticket comments (scoped to Tenant)
- **Priority** - Priority levels (scoped to Tenant)

### Database Relationships
```
Tenant (1) ─── (N) WaveUser
         └─── (N) Ticket
               └─── (N) Comment
         └─── (N) Priority
```

---

## 🔧 Getting Started

### Prerequisites
- .NET 8.0 SDK
- MySQL 8.0+
- Node.js 18+ (for frontend)

### Backend Setup

1. **Clone the repository**
```bash
git clone git@github.com:kerimakkis/optivieraSaaS.git
cd optivieraSaaS/backend
```

2. **Update database connection**
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=optiviera_saas;Uid=root;Pwd=yourpassword;"
  }
}
```

3. **Install packages** (when ready)
```bash
dotnet restore
```

4. **Run migrations** (upcoming)
```bash
dotnet ef database update
```

5. **Run the API**
```bash
dotnet run
```

API will be available at: `https://localhost:5001`
Swagger UI: `https://localhost:5001/swagger`

---

## 📚 API Endpoints (Planned)

### Authentication
```
POST   /api/auth/register       # Create new tenant & admin user
POST   /api/auth/login          # Get JWT token
POST   /api/auth/refresh        # Refresh token
```

### Tickets
```
GET    /api/tickets             # List all tickets (tenant-scoped)
GET    /api/tickets/{id}        # Get ticket details
POST   /api/tickets             # Create new ticket
PUT    /api/tickets/{id}        # Update ticket
DELETE /api/tickets/{id}        # Delete ticket
```

### Comments
```
GET    /api/tickets/{id}/comments    # List comments
POST   /api/tickets/{id}/comments    # Add comment
```

### Priorities
```
GET    /api/priorities          # List priorities
POST   /api/priorities          # Create priority
PUT    /api/priorities/{id}     # Update priority
DELETE /api/priorities/{id}     # Delete priority
```

---

## 🔐 Authentication

The platform uses **JWT Bearer Token** authentication:

1. **Register:** Create a new tenant account
2. **Login:** Get JWT token
3. **API Calls:** Include token in `Authorization: Bearer {token}` header
4. **Token includes:** UserId, TenantId, Email, Role

---

## 🎯 Roadmap

### Phase 1: Backend Foundation ✅ (Current)
- [x] Multi-tenant models
- [x] Web API infrastructure
- [ ] JWT authentication service
- [ ] Auth API endpoints
- [ ] Tenant middleware
- [ ] Tickets API endpoints

### Phase 2: Frontend Development
- [ ] Nuxt 3 project setup
- [ ] PrimeVue integration
- [ ] Login/Register pages
- [ ] Dashboard
- [ ] Tickets module
- [ ] Comments module

### Phase 3: Advanced Features
- [ ] Real-time notifications (SignalR)
- [ ] File uploads
- [ ] Email notifications
- [ ] Reporting & Analytics
- [ ] User management
- [ ] Role-based permissions

### Phase 4: Deployment
- [ ] Backend deployment (AkkisHost)
- [ ] Frontend deployment (Netlify/Vercel)
- [ ] CI/CD pipeline
- [ ] Production database setup

---

## 🧪 Development

### Backend Development
```bash
cd backend

# Run in development mode
dotnet run

# Run with hot reload
dotnet watch run

# Run tests (upcoming)
dotnet test
```

### Frontend Development (upcoming)
```bash
cd frontend/optiviera-web

# Install dependencies
npm install

# Run dev server
npm run dev

# Build for production
npm run build
```

---

## 📝 Environment Variables

### Backend
Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=optiviera_saas_dev;Uid=root;Pwd=dev123;"
  },
  "Jwt": {
    "SecretKey": "YourDevelopmentSecretKey32CharsMin!",
    "Issuer": "http://localhost:5001",
    "Audience": "http://localhost:3000",
    "ExpiryInDays": 7
  }
}
```

### Frontend (upcoming)
Create `.env`:
```
API_BASE_URL=http://localhost:5001/api
```

---

## 🤝 Contributing

This project is currently under active development. Contributions will be welcomed once the foundation is complete.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Contact

**Kerim Akkış**
- GitHub: [@kerimakkis](https://github.com/kerimakkis)
- Email: support@akkistech.com

---

## 🙏 Acknowledgments

- Original Optiviera Desktop App (archived in `/Optiviera`)
- Built with ASP.NET Core & Vue.js
- Multi-tenant architecture inspired by modern SaaS best practices

---

**Note:** This project is in active development. The Desktop App code is preserved in the `Optiviera/` directory for reference.
