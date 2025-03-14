# Book Search App

A responsive ASP.NET Core MVC application for searching and filtering books.

## Features

- Search books by keywords, title, summary, and more
- Filter by category, subgenres, and authors
- Responsive design for mobile and desktop
- Bootstrap 5 UI

## Prerequisites

- .NET 9.0 SDK
- Git

## Local Development

1. Clone the repository:
   ```
   git clone https://github.com/noxwei/MonkeysDotnetLibrary.git
   cd MonkeysDotnetLibrary/BookSearchApp
   ```

2. Run the application:
   ```
   dotnet run
   ```

3. Open your browser and navigate to:
   - http://localhost:5000
   - https://localhost:5001

## Deploying to Azure

### Current Azure Environment

- **Resource Group**: monkeys
- **Web App**: monkeysarch-isitworking
- **Domain**: monkeysarchive.com (custom) and monkeysarch-isitworking-fqdnadc0ggbtameu.eastus2-01.azurewebsites.net (default)
- **App Service Plan**: ASP-Monkeys-83fc (S1: 1) - Windows
- **Runtime Stack**: .NET 9.0
- **Application Insights**: MonkeysArch (East US 2)

### Deployment using GitHub Actions

1. Get the publish profile from the Azure portal:
   - Go to your Web App in the Azure portal (monkeysarch-isitworking)
   - Click on "Get publish profile" and download the file

2. Add the publish profile as a GitHub secret:
   - Go to your GitHub repository
   - Navigate to Settings > Secrets > Actions
   - Create a new secret named `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Paste the contents of the publish profile file

3. Push to the main branch to trigger the deployment:
   ```
   git push origin main
   ```

### Manual Deployment using Azure CLI

If GitHub Actions deployment fails, you can deploy manually:

1. Publish the application locally:
   ```
   dotnet publish -c Release
   ```

2. Deploy using Azure CLI:
   ```
   az webapp deployment source config-zip --resource-group monkeys --name monkeysarch-isitworking --src ./bin/Release/net9.0/publish/BookSearchApp.zip
   ```

## Application Settings

To configure Application Insights, add the following application settings in Azure:

```
ApplicationInsights__InstrumentationKey=<your-instrumentation-key>
ApplicationInsights__ConnectionString=<your-connection-string>
```

You can find these values in the Azure portal under your Application Insights resource (MonkeysArch).

## License

MIT 