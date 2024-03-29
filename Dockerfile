# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
ARG src="Project 1640/."
COPY *.sln .
COPY  ${src} ./Project_1640/
WORKDIR /source/Project_1640
RUN dotnet publish -c release -o /app

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Project 1640.dll"]
