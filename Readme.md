# HR Leave Management System

Welcome to the **HR Leave Management System** repository! This project is a comprehensive solution for managing employee leave requests, designed to streamline the leave approval process and ensure efficiency in human resources workflows.

---

## 🚀 Features

- **Employee Management**: Add, update, and manage employee profiles.
- **Leave Request System**: Employees can submit leave requests with details such as leave type, start/end dates, and reasons.
- **Approval Workflow**: Leave requests are reviewed and approved/rejected by managers.
- **Leave Balance Tracking**: Tracks leave balances for each employee.

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core (.NET Framework)
- **Frontend**: Blazor
- **Database**: SQL Server
- **Authentication**: ASP.NET Identity
- **Hosting**: IIS / Docker (optional)
- **Logging**: Serilog + Graylog
- **Monitoring**: Prometheus + Grafana

---

## 📂 Folder Structure

```
HR.LeaveManagement/
├── HR.LeaveManagement.Domain/     # Core business logic and models
├── HR.LeaveManagement.Application # Application services and DTOs
├── HR.LeaveManagement.Persistence # Database context and migrations
├── HR.LeaveManagement.API         # API endpoints
├── HR.LeaveManagement.BlazorUI          # Frontend (Blazor)
└── HR.LeaveManagement.Tests       # Unit and integration tests
```

---

## ⚠️ Prerequisites

Ensure you have the following installed:

- .NET 8 SDK or later
- SQL Server (local or cloud-based)
- Visual Studio, VS Code or Rider
- Git
- Docker (optional, for containerized deployment)

---

## 🔧 Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/mahdi-morovati/HR.LeaveManagement.git
   cd HR.LeaveManagement
   ```

2. **Set up the database:**
    - Update the connection string in `appsettings.json` to match your SQL Server configuration.
    - Run database migrations:
      ```bash
      dotnet ef database update --project HR.LeaveManagement.Persistence
      ```

3. **Run the application:**
   ```bash
   dotnet run --project HR.LeaveManagement.BlazorUI
   dotnet run --project HR.LeaveManagement.Api
   ```

4. **Access the app:**
    - Open your browser and navigate to `http://localhost:5281`.

---

## 🤝 Contributing

Contributions are welcome! To get started:

1. Fork the repository.
2. Create a new branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add some feature"
   ```
4. Push the branch:
   ```bash
   git push origin feature/your-feature-name
   ```
5. Open a Pull Request.

---

## 🧩 Testing

- Run unit tests:
  ```bash
  dotnet test
  ```
- Ensure all tests pass before submitting changes.

---

## 📊 Monitoring (Prometheus + Grafana)

To monitor the performance and behavior of the **HR Leave Management System**, Prometheus and Grafana have been integrated.

### 🧱 Architecture

| Service     | URL                             | Description                          |
|-------------|----------------------------------|--------------------------------------|
| Prometheus  | http://host.docker.internal:9090 | Metric scraping and querying engine  |
| ASP.NET API | http://localhost:5193/metrics    | Exposes app metrics (via `/metrics`) |
| Grafana     | http://localhost:3000            | Visualizes Prometheus metrics        |

---

### ⚙️ docker-compose.yml

```yaml
version: '3.8'

services:
  prometheus:
    image: prom/prometheus
    container_name: prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
    restart: always

  grafana:
    image: grafana/grafana
    container_name: grafana
    ports:
      - "3000:3000"
    restart: always
```

---

### 🔧 prometheus.yml

```yaml
global:
  scrape_interval: 5s

scrape_configs:
  - job_name: 'aspnet-api'
    static_configs:
      - targets: ['host.docker.internal:5193']
```

---

### 🛠️ ASP.NET Core Configuration

In `Program.cs`:

```csharp
using Prometheus;

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpMetrics();   // Enables automatic HTTP metrics
app.MapMetrics();       // Enables /metrics endpoint
```

---

### 📊 How to Access Metrics

- **Raw Metrics**:  
  `http://localhost:5193/metrics`

- **Prometheus UI**:  
  `http://host.docker.internal:9090`

  Example query:
  ```
  http_requests_received_total{controller="LeaveTypes"}
  ```

- **Grafana Dashboard**:  
  `http://localhost:3000`

---

### 🧪 Useful Metrics

| Metric                        | Description                       |
|------------------------------|-----------------------------------|
| `http_requests_received_total` | Total number of HTTP requests     |
| `http_request_duration_seconds` | Request latency distribution     |
| `dotnet_collection_count_total` | .NET garbage collections count  |
| `process_cpu_seconds_total`   | Total CPU time consumed          |

---

## 📋 Logging with Serilog + Graylog

To centralize and analyze logs, Serilog is configured to send logs to Graylog.

### 🛠️ Configuration

In `appsettings.json`:

```json
"Serilog": {
  "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Graylog" ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "Enrich": [ "FromLogContext" ],
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "Graylog",
      "Args": {
        "hostnameOrAddress": "localhost",
        "port": 12201,
        "transportType": "Udp",
        "facility": "HR.LeaveManagement",
        "shortMessageMaxLength": 5000
      }
    }
  ]
}
```

In `Program.cs`:

```csharp
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});
```

---

## 📜 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## 📞 Contact

If you have any questions or feedback, feel free to reach out:
- **Author**: Mahdi Morovati
- **GitHub**: [@mahdi-morovati](https://github.com/mahdi-morovati)
- **Email**: [morovati155@gmail.com](mailto:morovati155@gmail.com)