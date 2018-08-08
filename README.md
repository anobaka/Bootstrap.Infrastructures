# Bootstrap.Components

This project provides a series components to help building web projects quickly.

For now, this project is a package of many components so there maybe heavy references.

## Status

+ Under development

## Infrastructures


## Get Started

### Simple Error handling

#### Restful

1. Just use the defaults or customize your own `RestfulApiSimpleExceptionHandlingMiddleware` and `RestfulApiSimpleInvalidDataFilter`.
2. Add `RestfulApiSimpleInvalidDataFilter` to MVC options and add `app.UseRestfulApiSimpleExceptionHandler` in `Startup.cs`.

#### Non Restful

1. Just use the defaults or customize your own `SimpleExceptionHandlingMiddleware` and `SimpleInvalidDataFilter`.
2. Add `SimpleInvalidDataFilter` to MVC options and add `app.UseSimpleExceptionHandler` in `Startup.cs`.

### ElapsedTime

1. Just use the defaults or customize your own `ElapsedTimeMiddleware`.
2. Add `app.UseElapsedTime()` in `Startup.cs`.
2. Use `HttpContext.Items.AddElapsedTimeInfo(this IDictionary<object, object> items, object key, object info)` to add an elapsed time information into collection, and they will be logged before `ElapsedTimeMiddleware` existing.

### Base Models

#### Base Response Models

See Models.ResponseModels for further information.

## TODO
