# BinanceApp
Prerequisites: Installed .NET SDK.

Open Windows PowerShell and navigate to project folder with the following command:
  * cd ..../BinanceApp.

To build and run the app, execute the following commands:
  * dotnet build - Build the project and its dependencies.
  * dotnet run - Run the source code.

App can be reached at http://localhost:5049.

For publishing the app, run the following command:
  * dotnet publish --configuration Release --output [Desired folder path] - Publish the application and its dependencies to a folder for deployment to a hosting system.

Navigate to the publish folder and run the following command:
  * dotnet BinanceWebsocketApp.dll - Run the published application.

Published app by default can be reached at http://localhost:5000.
