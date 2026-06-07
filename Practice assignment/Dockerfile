FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Practice assignment/Practice assignment.csproj", "Practice assignment/"]
COPY ["PracticeAssignment.Data/PracticeAssignment.Data.csproj", "PracticeAssignment.Data/"]
RUN dotnet restore "Practice assignment/Practice assignment.csproj"
COPY . .
WORKDIR "/src/Practice assignment"
RUN dotnet build "Practice assignment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Practice assignment.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Practice assignment.dll"]