FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore project references
COPY LaundryCleaning.Models/LaundryCleaning.Models.csproj ./LaundryCleaning.Models/
COPY LaundryCleaning.Service/LaundryCleaning.Service.csproj ./LaundryCleaning.Service/
COPY LaundryCleaning.Scheduler/LaundryCleaning.Scheduler.csproj ./LaundryCleaning.Scheduler/
RUN dotnet restore ./LaundryCleaning.Scheduler/LaundryCleaning.Scheduler.csproj

# Copy actual code
COPY LaundryCleaning.Models/ ./LaundryCleaning.Models/
COPY LaundryCleaning.Service/ ./LaundryCleaning.Service/
COPY LaundryCleaning.Scheduler/ ./LaundryCleaning.Scheduler/

# Hapus appsettings.json dari Service (jaga-jaga)
RUN rm -f ./LaundryCleaning.Service/appsettings.json
RUN rm -f ./LaundryCleaning.Service/appsettings.Development.json
RUN rm -f ./LaundryCleaning.Service/appsettings-example.json

WORKDIR /src/LaundryCleaning.Scheduler
RUN dotnet publish -c Release -o /app

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Copy cron-value.yaml 
COPY LaundryCleaning.Scheduler/cron-value.yaml /app/cron-value.yaml

ENTRYPOINT ["dotnet", "LaundryCleaning.Scheduler.dll"]