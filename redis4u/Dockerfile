FROM microsoft/aspnetcore-build:2.0.8-2.1.200-jessie AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish --framework netcoreapp2.0 --configuration Release --output dist

# Build runtime image
FROM microsoft/aspnetcore:2.0.8
WORKDIR /app
COPY --from=build-env /app/dist .

# run the application
ENTRYPOINT ["dotnet", "redis4u.dll"]
