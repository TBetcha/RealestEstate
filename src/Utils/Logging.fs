module Logging

open System
open System.Collections.Generic

open Giraffe
open Giraffe.SerilogExtensions
open Serilog
open Serilog.Events
open Serilog.Formatting.Json
open Giraffe
open Giraffe.SerilogExtensions
open Serilog
open Serilog.Events




Log.Logger <-
    LoggerConfiguration()
        .MinimumLevel.Override(("Microsoft.AspNetCore": string), (LogEventLevel.Warning: LogEventLevel))
        .MinimumLevel.Debug()
        .Destructure.FSharpTypes()
        .WriteTo.Console(theme = Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
        .CreateLogger()

let serilogConfig =
  { SerilogConfig.defaults with
      ErrorHandler =
          fun ex httpContext ->
              setStatusCode 500
              >=> text "An unhandled exception has occurred while executing the request." }

