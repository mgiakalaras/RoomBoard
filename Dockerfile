# RoomBoard Docker image
# Build:
#   docker build -t roomboard:latest .
# Run:
#   docker run -p 7010:8080 -v roomboard_data:/app/App_Data -v roomboard_uploads:/app/wwwroot/uploads roomboard:latest

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RoomBoard.sln ./
COPY src/RoomBoard.Web/RoomBoard.Web.csproj src/RoomBoard.Web/
RUN dotnet restore src/RoomBoard.Web/RoomBoard.Web.csproj

COPY . .
RUN dotnet publish src/RoomBoard.Web/RoomBoard.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DISABLE_HTTPS_REDIRECTION=true
ENV TZ=Europe/Athens

EXPOSE 8080

COPY --from=build /app/publish .

RUN mkdir -p /app/App_Data /app/wwwroot/uploads

ENTRYPOINT ["dotnet", "RoomBoard.Web.dll"]
