FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only project files for restore
COPY LaundryCleaning.Models/LaundryCleaning.Models.csproj ./LaundryCleaning.Models/
COPY LaundryCleaning.Service/LaundryCleaning.Service.csproj ./LaundryCleaning.Service/
RUN dotnet restore ./LaundryCleaning.Service/LaundryCleaning.Service.csproj

# Copy source code for build
COPY LaundryCleaning.Models/ ./LaundryCleaning.Models/
COPY LaundryCleaning.Service/ ./LaundryCleaning.Service/

# Copy Libary DinkToPdf
COPY LaundryCleaning.Service/Storages/Lib/libwkhtmltox/ /app/Storages/Lib/libwkhtmltox/

WORKDIR /src/LaundryCleaning.Service
RUN dotnet publish -c Release -o /app

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Install dependencies for wkhtmltopdf
RUN apt-get update && apt-get install -y \
    libxrender1 \
    libfontconfig1 \
    libxext6 \
    libx11-dev \
    libjpeg62-turbo \
    libpng16-16 \
    libssl3 \
    libglu1-mesa \
    libgtk2.0-0 \
    xfonts-base \
    xfonts-75dpi \
    --no-install-recommends && \
    rm -rf /var/lib/apt/lists/*
	
WORKDIR /app
COPY --from=build /app .

# Copy library wkhtmltox
COPY LaundryCleaning.Service/Storages/Lib/libwkhtmltox/ /app/Storages/Lib/libwkhtmltox/

ENTRYPOINT ["dotnet", "LaundryCleaning.Service.dll"]