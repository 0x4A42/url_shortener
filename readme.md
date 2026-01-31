# Url Shortener

This repository is to host a url shortener I created for the purposes of wanting to learn about `MongoDB`, alongside creating a minimal API from scratch.

## Dependencies
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* A way to send API requests, e.g [Postman](https://www.postman.com/), [Bruno](https://www.usebruno.com/).

## Getting Started
* Clone the repository however you like.
* Navigate to `UrlShortener\Docker` and run `docker compose up -d`
* Run the `UrlShortener` project.
* Hit the API with some requests (samples below).

## Sample API Requests

### Shorten a URL - `{url}/url/ (POST)`

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

### Purge stale URLs - `/purge (DELETE)`

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

## TODO/Future Improvements
* Maybe add some Api collections for ease of use/testing