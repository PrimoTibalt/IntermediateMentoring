FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
RUN dotnet dev-certs https
RUN dotnet dev-certs https --trust
WORKDIR /RestApi
COPY RestApi/*.csproj .
RUN dotnet restore
COPY RestApi .
RUN dotnet publish -c Release -o /publish
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 80
COPY --from=build-env /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "RestApi.dll"]