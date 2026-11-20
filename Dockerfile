# Backend StayMaster (ASP.NET Core 9). Dùng cùng docker-compose.yml: xem DEPLOY.md, mục "Chạy bằng Docker".
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY HotelManagement.API.csproj ./
RUN dotnet restore HotelManagement.API.csproj
COPY . .
RUN dotnet publish HotelManagement.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
# Image đã có tzdata; giờ Việt Nam cho DateTime.Now đặt bằng TZ trong docker-compose.yml
COPY --from=build /app/publish .
# Thư mục ảnh người dùng tải lên (gắn volume), thuộc user không phải root của image
RUN mkdir -p wwwroot/uploads && chown -R $APP_UID wwwroot/uploads
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HotelManagement.API.dll"]
