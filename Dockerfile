FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
RUN mkdir /app
WORKDIR /app

# copy the project and restore as distinct layers in the image
COPY src/*.csproj ./
RUN dotnet restore

# copy the rest and build
COPY src/ ./
RUN dotnet build
RUN dotnet publish --runtime alpine-x64 -c Release -o out --self-contained true /p:PublishTrimmed=true

# build runtime image with DoD CA Certificates
FROM cingulara/openrmf-base:1.09.01
RUN apk update && apk upgrade && rm -rf /var/cache/apk/*

RUN mkdir /app
WORKDIR /app
COPY --from=build-env /app/out .
# Fix for broken build on Docker in GH is to put RUN true between multiple COPY statements :(
RUN true
COPY src/nlog.config /app/nlog.config
RUN true

# Create a group and user
RUN addgroup --system --gid 1001 openrmfgroup \
&& adduser --system -u 1001 --ingroup openrmfgroup --shell /bin/sh openrmfuser
RUN chown openrmfuser:openrmfgroup /app

USER 1001
# start the application
ENTRYPOINT ["./openrmf-api-controls"]

LABEL org.opencontainers.image.source https://github.com/Cingulara/openrmf-api-controls
LABEL maintainer="dale.bingham@cingulara.com"
