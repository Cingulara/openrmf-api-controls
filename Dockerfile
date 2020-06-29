FROM microsoft/dotnet:2.2-sdk AS build-env
RUN mkdir /app
WORKDIR /app

# copy the project and restore as distinct layers in the image
COPY src/*.csproj ./
RUN dotnet restore

# copy the rest and build
COPY ./src/ ./
RUN dotnet build
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
RUN apt-get update && apt-get -y upgrade && apt-get -y dist-upgrade && apt-get -y install ca-certificates &&  apt-get clean

RUN mkdir /app
WORKDIR /app
COPY --from=build-env /app/out .

# Create a group and user
RUN addgroup --system --gid 1001 openrmfgroup \
&& adduser --system -u 1001 --ingroup openrmfgroup --shell /bin/sh openrmfuser
RUN chown openrmfuser:openrmfgroup /app

USER 1001
ENTRYPOINT ["dotnet", "openrmf-api-controls.dll"]