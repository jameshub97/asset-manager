# backend/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore
COPY *.sln ./
COPY backend/backend.csproj ./backend/
RUN dotnet restore backend/backend.csproj

# copy everything and build
COPY backend/. ./backend/
WORKDIR /src/backend
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5001
ENTRYPOINT ["dotnet", "backend.dll"]
