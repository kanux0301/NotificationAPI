# NotificationAPI

A scalable notification orchestrator API built with .NET 10, Clean Architecture, and Domain-Driven Design (DDD).

## Project Structure

```
NotificationAPI/
├── src/
│   ├── Notification.WebAPI/           # API layer
│   ├── Notification.Application/      # Business logic (CQRS)
│   ├── Notification.Domain/           # Domain entities & interfaces
│   └── Notification.Infrastructor/    # Data access & external services
├── test/
│   └── Notification.WebAPI/           # Unit tests
└── deployment/
    └── build.yaml                     # CI/CD pipeline
```

## Features

- **Multi-channel notifications**: Email, SMS, Push, Webhook, In-App
- **Template support**: Reusable notification templates with variables
- **Message queue integration**: RabbitMQ, Azure Service Bus, or InMemory
- **Scheduled notifications**: Send notifications at a future time
- **Priority levels**: Low, Normal, High, Critical
- **Status tracking**: Pending, Processing, Sent, Delivered, Failed, Cancelled

## Tech Stack

- .NET 10
- Entity Framework Core 9
- MediatR (CQRS)
- FluentValidation
- RabbitMQ.Client / Azure.Messaging.ServiceBus
- Scalar (API Documentation)

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (optional - uses InMemory by default)
- RabbitMQ or Azure Service Bus (optional - uses InMemory by default)

### Access

- **API Documentation**: https://localhost:7003/scalar/v1
- **OpenAPI Spec**: https://localhost:7003/openapi/v1.json
- **Health Check**: https://localhost:7003/health

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""  // Leave empty for InMemory DB
  },
  "Messaging": {
    "Provider": "InMemory"   // Options: InMemory, RabbitMQ, AzureServiceBus
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  },
  "AzureServiceBus": {
    "ConnectionString": ""
  }
}
```

## API Endpoints

### Notifications

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/notifications` | Send a notification |
| GET | `/api/notifications/{id}` | Get notification by ID |
| GET | `/api/notifications/status/{status}` | List by status |
| GET | `/api/notifications/stats` | Get statistics |
| POST | `/api/notifications/{id}/retry` | Retry failed notification |
| POST | `/api/notifications/{id}/cancel` | Cancel pending notification |

### Templates

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/templates` | Create template |
| GET | `/api/templates` | List all templates |
| GET | `/api/templates/{id}` | Get template by ID |

## Usage Examples

### Send Email Notification

```bash
curl -X POST https://localhost:7003/api/notifications \
  -H "Content-Type: application/json" \
  -d '{
    "recipientAddress": "user@example.com",
    "recipientName": "John Doe",
    "channel": 0,
    "subject": "Welcome!",
    "body": "Hello, welcome to our platform!",
    "priority": 1,
    "isHtml": false
  }'
```

### Channel Types

| Channel | Value |
|---------|-------|
| Email | 0 |
| SMS | 1 |
| Push | 2 |
| Webhook | 3 |
| InApp | 4 |

### Priority Levels

| Priority | Value |
|----------|-------|
| Low | 0 |
| Normal | 1 |
| High | 2 |
| Critical | 3 |

## Queue Names

Channel microservices should consume from these queues:

| Queue | Channel |
|-------|---------|
| `notifications.email` | Email service |
| `notifications.sms` | SMS service |
| `notifications.push` | Push service |
| `notifications.webhook` | Webhook service |
| `notifications.inapp` | In-App service |
| `notifications.status` | Status updates (orchestrator consumes) |

## License

MIT
