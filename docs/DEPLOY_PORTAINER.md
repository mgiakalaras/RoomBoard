# RoomBoard - Portainer deployment

This project is ready to run as a Docker/Portainer stack.

## Default port

The compose file maps:

```text
server:7010 -> container:8080
```

Open:

```text
http://SERVER_IP:7010
```

## Portainer stack

1. Push the project to GitHub.
2. In Portainer, go to **Stacks**.
3. Choose **Add stack**.
4. Use either:
   - **Repository** mode: point Portainer to the GitHub repository and `docker-compose.yml`, or
   - **Web editor** mode: paste the contents of `docker-compose.yml`.
5. Deploy the stack.

## Persistent data

The compose file creates two named volumes:

```text
roomboard_data     -> /app/App_Data
roomboard_uploads  -> /app/wwwroot/uploads
```

These preserve:

- SQLite database
- school logo uploads
- kiosk spotlight image uploads

## Health check

The app exposes:

```text
/health
```

Expected response:

```text
OK
```

## Changing port

To use another external port, edit:

```yaml
ports:
  - "7010:8080"
```

Example for 7020:

```yaml
ports:
  - "7020:8080"
```

Do not change the container port unless you also change `ASPNETCORE_URLS`.
