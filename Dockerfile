# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
# Use official .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

#For Dev
#COPY ["FarmServer/FarmServer.csproj", "FarmServer/"]
#RUN dotnet restore "./FarmServer/FarmServer.csproj"

#For Prod
COPY ["FarmServer.csproj", "."]
RUN dotnet restore "./FarmServer.csproj"

# Copy remaining source files
COPY . .

#For dev
WORKDIR "/src/FarmServer"

#For Prod
WORKDIR "/src"

RUN dotnet build "./FarmServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FarmServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
# Final image for production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# Set environment variable
ENV ASPNETCORE_ENVIRONMENT=Production
#ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "FarmServer.dll"]