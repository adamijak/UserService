FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ApiTest/ApiTest.csproj", "ApiTest/"]
RUN dotnet restore "ApiTest/ApiTest.csproj"
COPY . .
WORKDIR "/src/ApiTest"
RUN dotnet build "ApiTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "test", "ApiTest.dll"]
