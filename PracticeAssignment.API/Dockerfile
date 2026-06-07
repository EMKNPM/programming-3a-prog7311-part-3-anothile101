FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PracticeAssignment.API/PracticeAssignment.API.csproj", "PracticeAssignment.API/"]
COPY ["PracticeAssignment.Data/PracticeAssignment.Data.csproj", "PracticeAssignment.Data/"]
RUN dotnet restore "PracticeAssignment.API/PracticeAssignment.API.csproj"
COPY . .
WORKDIR "/src/PracticeAssignment.API"
RUN dotnet build "PracticeAssignment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PracticeAssignment.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PracticeAssignment.API.dll"]