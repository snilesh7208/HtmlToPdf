# HTML to PDF Web API 📄🚀

A high-performance, enterprise-grade REST API built with **.NET 8** that seamlessly converts HTML content and external web pages into high-quality PDF documents. 

This project is built following **Clean Architecture** principles and implements the **CQRS pattern** using MediatR. It utilizes headless Chromium via **PuppeteerSharp** to accurately render modern HTML, CSS, JavaScript, and even lazy-loaded images into pixel-perfect PDFs.

---

## 🌟 Key Features

- **Accurate PDF Rendering**: Uses `PuppeteerSharp` (Headless Chromium) to generate PDFs exactly as they appear in a modern browser.
- **Handles Lazy-Loaded Images**: Automatically disables lazy loading (`loading="lazy"`) and injects `<base>` tags to ensure all external assets and images are fully captured.
- **Clean Architecture & CQRS**: Separation of concerns using Domain, Application, Infrastructure, and API layers. MediatR is used for clean command/query handling.
- **Resilient External Calls**: Implements `Polly` for resilient and retry-enabled HTTP requests to external websites (e.g., fetching Wikipedia articles).
- **Validation Pipeline**: Uses `FluentValidation` to validate API requests automatically.
- **Structured Logging**: Pre-configured with `Serilog` for rich, structured console and file logging.
- **Global Error Handling**: Custom middleware to cleanly catch exceptions and return standardized API error responses.

## 🛠️ Technology Stack

- **Framework**: .NET 8.0 Web API
- **PDF Engine**: PuppeteerSharp
- **Architecture**: MediatR (CQRS), Clean Architecture
- **Resilience**: Polly (Retry Policies)
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Documentation**: Swagger / OpenAPI

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.

### Installation & Running

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/HtmlToPdf.git
   cd HtmlToPdf
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   cd src
   dotnet run
   ```

4. **Access the API Documentation:**
   Open your browser and navigate to `https://localhost:<port>/swagger` to view the Swagger UI.

## 📖 API Usage

### Generate PDF Endpoint
**`POST /api/pdf/generate`**

Converts a Wikipedia article identifier or a direct URL into a PDF file.

**Request Body (JSON):**
```json
{
  "identifier": "Hydrogen" 
  // OR use a full URL:
  // "identifier": "https://en.wikipedia.org/wiki/Hydrogen"
}
```

**Response:**
Returns a binary file (`application/pdf`) which will prompt a download in your browser or API client.

## 🏗️ Architecture Overview

- **Api Layer**: Contains Controllers, Swagger configuration, and Global Error Middleware.
- **Application Layer**: Contains business logic, CQRS Handlers (MediatR), Validators (FluentValidation), and Interface definitions.
- **Domain Layer**: Contains custom Exceptions and core business models.
- **Infrastructure Layer**: Contains the implementation of external services (`PuppeteerPdfGenerator`, `ExternalDataClient`).

## 🤝 Contributing
Contributions, issues, and feature requests are welcome!

## 📝 License
This project is licensed under the MIT License.
