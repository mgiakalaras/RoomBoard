# RoomBoard release checklist

## Local cleanup

From the project root:

```powershell
dotnet clean
Get-ChildItem -Recurse -Directory -Include bin,obj,publish | Remove-Item -Recurse -Force
dotnet restore
dotnet build -c Release
```

## Git status

```powershell
git status
```

Confirm that no database or uploaded runtime files are staged:

```text
*.db
*.db-shm
*.db-wal
wwwroot/uploads/*
```

## Commit

```powershell
git add .
git commit -m "Release RoomBoard v1.0.0"
git push origin main
```

## Tag

```powershell
git tag v1.0.0
git push origin v1.0.0
```

## GitHub release with GitHub CLI

```powershell
gh release create v1.0.0 --title "RoomBoard v1.0.0" --notes "Initial stable release with teacher/admin timeline, student kiosk, bookings, Excel import, school settings, weekly print and Portainer deployment."
```

If `gh` is not recognized after installation, close and reopen PowerShell or run:

```powershell
$env:Path += ";$env:ProgramFiles\GitHub CLI\"
gh --version
```

## Portainer

Deploy `docker-compose.yml`.

Default URL:

```text
http://SERVER_IP:7010
```
