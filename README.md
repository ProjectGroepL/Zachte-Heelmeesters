# Zachte Heelmeesters

Fullstack application for the "Zachte Heelmeesters" Hospital

## 🏗️ Architecture

This is a monorepo containing:

- **Client** (`apps/client`): Vue.js 3 frontend with TypeScript, Tailwind CSS, and Pinia for state management
- **Server** (`apps/server`): ASP.NET Core 8.0 Web API with Swagger documentation
- **Infrastructure** (`infra`): Docker configurations for local development and production

## 🛠️ Tech Stack

### Frontend
- **Vue.js 3** with Composition API
- **TypeScript** for type safety
- **Tailwind CSS** for styling
- **shadcn-vue** for UI components (New York style)
- **Lucide Vue** for icons
- **Pinia** for state management
- **Vue Router** for navigation
- **Vite** for build tooling
- **Vitest** for testing

### Backend
- **ASP.NET Core 8.0** Web API
- **Entity Framework** (implied from project structure)
- **Swagger/OpenAPI** for API documentation
- **SQL Server** for database

### DevOps & Infrastructure
- **Docker** for containerization
- **Turborepo** for monorepo management
- **nginx** for reverse proxy

## 📋 Prerequisites

Make sure you have the following installed:

- **Node.js** >= 18 (recommended: >= 20.19.0 || >= 22.12.0)
- **.NET 8.0 SDK**
- **Docker** and **Docker Compose**
- **npm** 11.3.0 or later

## 🚀 Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/ProjectGroepL/Zachte-Heelmeesters.git
cd Zachte-Heelmeesters
```

### 2. Install dependencies
**Install npm packages**
```bash
npm install
```

**Install nuget packages**
```bash
cd /apps/server
npm run prepare
```

### 3. Start the development environment

**Start the database:**
```bash
cd /infra/dev
docker-compose up -d
```

**Start the client and server**
```bash
# Start all services (client, and server)
npm run dev
```



**Start the client (Vue.js):**
```bash
cd apps/client
npm run dev

OR

npm run dev -w @zhm/client
```

**Start the server (ASP.NET Core):**
```bash
cd apps/server
npm run dev

OR

npm run dev -w @zhm/server
```

## 📁 Project Structure

```
.
├── apps/
│   ├── client/           # Vue.js frontend application
│   │   ├── src/
│   │   │   ├── components/   # Vue components
│   │   │   ├── router/       # Vue Router configuration
│   │   │   ├── stores/       # Pinia stores
│   │   │   └── lib/          # Utility functions
│   │   └── __tests__/        # Test files
│   └── server/           # ASP.NET Core backend
│       └── server/
│           ├── Controllers/  # API controllers
│           ├── Data/         # Database context and data access
│           ├── Dtos/         # Data Transfer Objects
│           ├── Models/       # Domain models and entities
│           └── Properties/   # Launch settings

├── infra/
│   ├── dev/              # Development environment
│   │   ├── docker-compose.yml  # Local development database
│   │   └── .env          # Development environment variables
│   └── prod/             # Production environment
│       ├── docker-compose.yml  # Full production stack
│       ├── .env          # Production environment variables
│       └── nginx/        # nginx configuration
├── package.json          # Root package.json
└── turbo.json           # Turborepo configuration
```

## 🔧 Available Scripts

### Root Level Commands
- `npm run dev` - Start all applications in development mode
- `npm run build` - Build all applications for production
- `npm run test` - Run tests for all applications

### Client Commands (apps/client)
- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run test` - Run unit tests
- `npm run preview` - Preview production build

### Server Commands (apps/server)
- `npm run dev` - Start the development server
- `npm run prepare` - Prepare the server environment
- `npm run build` - Build the server for production

## 🎨 UI Components (shadcn-vue)

This project uses **shadcn-vue** for UI components with the following configuration:

- **Style**: New York
- **Base Color**: Neutral
- **Icon Library**: Lucide
- **CSS Variables**: Enabled

### Adding New Components

To add new shadcn-vue components to the client:

```bash
cd apps/client
npx shadcn-vue@latest add [component-name]
```

**Examples:**
```bash
# Add a card component
npx shadcn-vue@latest add card

# Add a dialog component  
npx shadcn-vue@latest add dialog

# Add multiple components
npx shadcn-vue@latest add button input label
```

### Component Structure

Components are organized as follows:
- **UI Components**: `apps/client/src/components/ui/` - shadcn-vue components
- **Custom Components**: `apps/client/src/components/` - Application-specific components
- **Utilities**: `apps/client/src/lib/utils.ts` - Utility functions (includes `cn` helper)

### Available Components

Check the current components in `apps/client/src/components/ui/` or visit the [shadcn-vue documentation](https://shadcn-vue.com/docs/components) for the full component library.

## 🐳 Docker Development

### Local Development
Start the development database:

```bash
cd /infra/dev
docker-compose.yml up -d
```

This will start:
- SQL Server database on the port specified in `infra/dev/.env` (default: 1433)

### Database Connection
- **Host:** localhost
- **Port:** Configured in `infra/dev/.env`
- **Username:** sa
- **Password:** Configured in `infra/dev/.env`

### Production Deployment
For production deployment with full stack:

```bash
cd /infra/prod
docker-compose up -d
```

This will start:
- SQL Server database
- ASP.NET Core API server
- Vue.js client application
- nginx reverse proxy

## 🧪 Testing

### Entire fullstack app
```bash
npm run test
```

### Frontend Testing
```bash
cd apps/client
npm run test
```

### Backend Testing
```bash
cd apps/server
npm run test
```

## 📦 Building for Production

### Build All Applications
```bash
npm run build
```

### Build Individual Applications

**Client:**
```bash
cd apps/client
npm run build
```

**Server:**
```bash
cd apps/server
dotnet build --configuration Release
```

## 🚀 Deployment

The project includes Docker configurations for production deployment:

```bash
cd /infra/prod
docker-compose up -d
```

## 👥 Contributing

1. Clone the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the ISC License.

## 👨‍💻 Authors

**GroepL** - *Initial work*

---

## 🔗 Additional Resources

- [Vue.js Documentation](https://vuejs.org/)
- [shadcn-vue Documentation](https://shadcn-vue.com/)
- [Tailwind CSS Documentation](https://tailwindcss.com/)
- [Reka UI Documentation](https://reka-ui.com/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Turborepo Documentation](https://turbo.build/repo/docs)
- [Docker Documentation](https://docs.docker.com/)