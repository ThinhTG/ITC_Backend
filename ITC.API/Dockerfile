# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ITC.API/ITC.API.csproj", "ITC.API/"]
COPY ["ITC.Mapping/ITC.Mapping.csproj", "ITC.Mapping/"]
COPY ["ITC.BusinessObject/ITC.BusinessObject.csproj", "ITC.BusinessObject/"]
COPY ["ITC.Core/ITC.Core.csproj", "ITC.Core/"]
COPY ["ITC.Services/ITC.Services.csproj", "ITC.Services/"]
COPY ["ITC.Repositories/ITC.Repositories.csproj", "ITC.Repositories/"]
RUN dotnet restore "./ITC.API/ITC.API.csproj"
COPY . .
WORKDIR "/src/ITC.API"
RUN dotnet build "./ITC.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ITC.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ITC.API.dll"]