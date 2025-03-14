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

### Option 1: Deploy using GitHub Actions

1. Create an Azure Web App
   ```
   az webapp create --resource-group BookSearchAppResourceGroup --plan BookSearchAppServicePlan --name booksearchapp --runtime "DOTNET|9.0"
   ```

2. Get the publish profile from the Azure portal:
   - Go to your Web App in the Azure portal
   - Click on "Get publish profile" and download the file

3. Add the publish profile as a GitHub secret:
   - Go to your GitHub repository
   - Navigate to Settings > Secrets > Actions
   - Create a new secret named `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Paste the contents of the publish profile file

4. Push to the main branch to trigger the deployment:
   ```
   git push origin main
   ```

### Option 2: Deploy using Azure CLI

1. Create an Azure Web App
   ```
   az webapp create --resource-group BookSearchAppResourceGroup --plan BookSearchAppServicePlan --name booksearchapp --runtime "DOTNET|9.0"
   ```

2. Deploy the application:
   ```
   az webapp deployment source config --name booksearchapp --resource-group BookSearchAppResourceGroup --repo-url https://github.com/noxwei/MonkeysDotnetLibrary.git --branch main --manual-integration
   ```

## Application Settings

To configure Application Insights, add the following application setting in Azure:

```
ApplicationInsights__ConnectionString=<your-connection-string>
```

## License

MIT 