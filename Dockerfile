FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /Server

FROM node:lts AS frontend
WORKDIR /client
COPY client .
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /Server
COPY Server .
COPY --from=frontend /client/build wwwroot
RUN dotnet dev-certs https -ep /https/aspnetapp.pfx -p "myP@ssW0rd"
RUN dotnet dev-certs https --trust
RUN dotnet restore Server.csproj
RUN dotnet build Server.csproj -c Release -o out
RUN dotnet publish -c Release -o out

FROM base AS final
WORKDIR /Server
COPY --chmod=0755 --from=build /https/* /https/
COPY --from=build Server/out .
ENTRYPOINT ["dotnet", "Server.dll"]
