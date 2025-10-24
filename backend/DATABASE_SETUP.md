# MySQL Database Setup Guide

This guide will help you set up MySQL for the Optiviera SaaS backend.

## Prerequisites

- MySQL 8.0 or higher installed on your machine
- .NET 8.0 SDK installed

## Installation

### macOS (using Homebrew)

```bash
# Install MySQL
brew install mysql

# Start MySQL service
brew services start mysql

# Secure MySQL installation (optional but recommended)
mysql_secure_installation
```

### Windows

1. Download MySQL Installer from [MySQL Downloads](https://dev.mysql.com/downloads/installer/)
2. Run the installer and follow the setup wizard
3. Choose "Developer Default" or "Server only" setup type
4. Set a root password during installation

### Linux (Ubuntu/Debian)

```bash
# Install MySQL
sudo apt update
sudo apt install mysql-server

# Start MySQL service
sudo systemctl start mysql

# Secure MySQL installation
sudo mysql_secure_installation
```

## Database Configuration

### 1. Create Database

```bash
# Login to MySQL
mysql -u root -p

# Create database
CREATE DATABASE optiviera_saas_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# Create database user (recommended for production)
CREATE USER 'optiviera'@'localhost' IDENTIFIED BY 'YourSecurePassword123!';
GRANT ALL PRIVILEGES ON optiviera_saas_dev.* TO 'optiviera'@'localhost';
FLUSH PRIVILEGES;

# Exit MySQL
EXIT;
```

### 2. Update Connection String

Update `appsettings.Development.json` with your MySQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=optiviera_saas_dev;Uid=root;Pwd=YOUR_PASSWORD_HERE;"
  }
}
```

**Important:** Never commit your actual password to Git. Use environment variables or user secrets for production.

### 3. Apply Migrations

```bash
# Navigate to backend directory
cd backend

# Apply migrations to create database schema
dotnet ef database update

# You should see output like:
# Applying migration '20251024185229_InitialMultiTenant'.
# Done.
```

### 4. Verify Database Setup

```bash
# Login to MySQL
mysql -u root -p

# Use the database
USE optiviera_saas_dev;

# Show tables
SHOW TABLES;

# You should see tables like:
# - Tenants
# - AspNetUsers
# - AspNetRoles
# - Tickets
# - Comments
# - Priorities
# etc.
```

## Running the Application

```bash
# Navigate to backend directory
cd backend

# Run the application
dotnet run

# The API should start at:
# http://localhost:5000
# https://localhost:5001

# Swagger UI available at:
# http://localhost:5000/swagger
```

## Demo Account

After the first run, a demo tenant and admin user will be created automatically:

- **Email:** admin@optiviera.com
- **Password:** Admin123!
- **Company:** Demo Company

You can use these credentials to test the API endpoints.

## Troubleshooting

### Connection Issues

If you get "Unable to connect to MySQL" errors:

1. Check if MySQL is running:
   ```bash
   # macOS
   brew services list | grep mysql

   # Linux
   sudo systemctl status mysql

   # Windows (in PowerShell)
   Get-Service MySQL*
   ```

2. Verify connection details:
   - Server: localhost
   - Port: 3306 (default)
   - Username: root (or your custom user)
   - Password: Your MySQL root password

3. Test MySQL connection:
   ```bash
   mysql -u root -p -h localhost
   ```

### Migration Issues

If migrations fail:

```bash
# Remove last migration
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list

# Force database update
dotnet ef database update --force
```

### Reset Database (Development Only)

```bash
# Drop the database
mysql -u root -p -e "DROP DATABASE optiviera_saas_dev;"

# Recreate database
mysql -u root -p -e "CREATE DATABASE optiviera_saas_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"

# Reapply migrations
dotnet ef database update
```

## Production Considerations

1. **Use a dedicated database user** instead of root
2. **Set strong passwords** for database users
3. **Enable SSL/TLS** for database connections
4. **Configure firewall rules** to restrict database access
5. **Regular backups** using mysqldump or automated backup solutions
6. **Use environment variables** or Azure Key Vault for connection strings
7. **Connection pooling** is handled by Entity Framework Core by default

## Environment Variables (Production)

Instead of hardcoding connection strings, use environment variables:

```bash
export ConnectionStrings__DefaultConnection="Server=prod-server;Port=3306;Database=optiviera_saas;Uid=optiviera_user;Pwd=SecurePassword;SslMode=Required;"
```

Or use Azure App Configuration / AWS Systems Manager Parameter Store.
