# Url Shortener

This repository is to host a url shortener I created for the purposes of wanting to learn about `MongoDB`, alongside creating a minimal API from scratch.

## Dependencies
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* A way to send API requests, e.g [Postman](https://www.postman.com/), [Bruno](https://www.usebruno.com/).

## Getting Started
* Clone the repository however you like.
* Navigate to `docker/` and run `docker compose up -d`
* Run the `UrlShortener` project.
* Hit the API with some requests (samples below or in the `Postman` collection).

## Sample API Requests

A `Postman` collection and environment can be found at `postman/`. You can import these and select the `UrlShortener` environment, updating any variables as needed, for testing purposes.

The base URL has been `http://localhost:5002/` during development. Your IDE may change this, so double check if you're running into issues or have a port conflict.

### Shorten a URL - `{url}/shorten/ (POST)`

Sample Request
```
{
  "Url": "http://google.com",
  "Length": 5
}
```

Sample Response
```
{
  "originalUrl": "http://google.com",
  "shortenedUrl": "qKedo",
  "detail": "Success."
}
```

### Get redirected with a previously shortened URL - `{url}/url/qKedo (GET)`

Simply enter the URL with the `shortenedUrl` from the previous step in your browser and you will be magically redirected to the URL you shortened.

### Purge stale URLs - `{url}/purge (DELETE)`

This is an authenticated endpoint, so you'll need to grab the value of `ApiKey` from the `appsettings.json` file and add it as the value to an `x-api-key` header.

Sample Request
```
{
  "RemoveAfterDays": 1
}
```

Sample Response

```
{
  "detail": "3 URL(s) successfully purged."
}
```
