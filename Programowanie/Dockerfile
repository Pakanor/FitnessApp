# Wybieramy obraz bazowy Windows z .NET Framework
FROM mcr.microsoft.com/dotnet/framework/desktop:4.8-windowsservercore-ltsc2022 AS build

# Ustawiamy katalog roboczy
WORKDIR /src

# Kopiujemy projekt do obrazu
COPY . .

# Przywracamy zależności
RUN dotnet restore "FitnessApp/fitnessapp.csproj"

# Budujemy aplikację
RUN dotnet build "FitnessApp/fitnessapp.csproj" -c Release

# Publikujemy aplikację
RUN dotnet publish "FitnessApp/fitnessapp.csproj" -c Release -o /app/publish

# Używamy Windows Server Core z .NET Framework jako bazowego obrazu dla uruchamiania aplikacji
FROM mcr.microsoft.com/dotnet/framework/desktop:4.8-windowsservercore-ltsc2022 AS runtime

# Ustawiamy katalog roboczy w kontenerze
WORKDIR /app

# Kopiujemy opublikowaną aplikację do kontenera
COPY --from=build /app/publish .

# Definiujemy punkt wejścia
ENTRYPOINT ["FitnessApp.exe"]
