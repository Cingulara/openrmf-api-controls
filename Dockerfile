FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
RUN mkdir /app
WORKDIR /app

# copy the project and restore as distinct layers in the image
COPY src/*.csproj ./
RUN dotnet restore

# copy the rest and build
COPY src/ ./
RUN dotnet build
RUN dotnet publish --runtime linux-musl-x64 -c Release -o out --self-contained true

# build runtime image with DoD CA Certificates
FROM docker.io/cingulara/openrmf-base:1.14.03
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

LABEL org.opencontainers.image.source=https://github.com/Cingulara/openrmf-api-controls
LABEL org.opencontainers.image.authors="dale.bingham@cingulara.com"
LABEL org.opencontainers.image.description="This is the controls internal API to talk to the backend for controls and CCI type data for OpenRMF OSS"
LABEL org.opencontainers.image.vendor="Cingulara LLC and Tutela LLC"
LABEL org.opencontainers.image.title="OpenRMF OSS Controls internal API"