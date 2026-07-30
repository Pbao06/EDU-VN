# --- Giai đoạn Build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Source.csproj", "./"]
RUN dotnet restore "Source.csproj"
COPY . .
RUN dotnet publish "Source.csproj" -c Release -o /app/publish

# --- Giai đoạn Chạy ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Source.dll"]