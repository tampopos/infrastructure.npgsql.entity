# tmpps.infrastructure.npgsql.entity

## command

#### build

`dotnet build Tmpps.Infrastructure.Npgsql.Entity.Tests/`

#### test

`dotnet test Tmpps.Infrastructure.Npgsql.Entity.Tests/`

#### register nuget

```bash
dotnet build -c Release Tmpps.Infrastructure.Npgsql.Entity
# replace version,api-key
dotnet pack -c Release --include-source -p:PackageVersion=${version} Tmpps.Infrastructure.Npgsql.Entity
dotnet nuget push ./Tmpps.Infrastructure.Npgsql.Entity/bin/Release/Tmpps.Infrastructure.Npgsql.Entity.${version}.nupkg -k ${api-key} -s https://api.nuget.org/v3/index.json
```

## use circleCI CLI

#### validation config

`circleci config validate`

#### test

`circleci local execute --job test`

#### release

```bash
git tag X.Y.Z
git push origin --tags
```
