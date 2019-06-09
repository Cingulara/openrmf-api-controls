FROM microsoft/dotnet:2.2-sdk AS build-env
RUN mkdir /app
WORKDIR /app

# copy the project and restore as distinct layers in the image
COPY src/*.csproj ./
RUN dotnet restore

# copy the rest and build
COPY src/ ./
RUN dotnet build
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
RUN mkdir /app
WORKDIR /app
# RUN apt-get update && apt-get -y install ca-certificates

COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "openrmf-api-controls.dll"]