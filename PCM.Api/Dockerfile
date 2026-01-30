FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["PCM.Api/PCM.Api.csproj", "PCM.Api/"]
RUN dotnet restore "PCM.Api/PCM.Api.csproj"

# Copy everything and publish
COPY . .
WORKDIR /src/PCM.Api
RUN dotnet publish "PCM.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PCM.Api.dll"]
