module RealestEstate.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.EndpointRouting
open RealestEstate.Routes
open Microsoft.Extensions.Configuration
open DbUp
open DbUp.ScriptProviders
open Npgsql
open Serilog
open Giraffe.SerilogExtensions
// ---------------------------------
// Error handler
// ---------------------------------
let webAppWithLogging = SerilogAdapter.Enable(routes, Logging.serilogConfig)

// ---------------------------------

// Config and Main
// ---------------------------------

let dbUpgrade (connectionString: string) =
  let fs = FileSystemScriptOptions()
  printfn "%A" connectionString
  fs.IncludeSubDirectories <- true
  DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsFromFileSystem("Migrations", fs)
    .LogToConsole()
    .Build()
    .PerformUpgrade()

let configureAppConfiguration (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
  config
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile(sprintf "appsettings.%s.json" context.HostingEnvironment.EnvironmentName, true)

let configureCors (builder: CorsPolicyBuilder) =
  builder
    .WithOrigins("http://localhost:5000", "https://localhost:5001")
    .AllowAnyMethod()
    .AllowAnyHeader()
  |> ignore

let configureApp (app: IApplicationBuilder) =
  let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
  (match env.IsDevelopment() with
   | true -> app.UseDeveloperExceptionPage()
   | false -> app.UseHttpsRedirection())
    .UseCors(configureCors)
    .UseGiraffe
    webAppWithLogging

let configureServices (services: IServiceCollection) =
  let env = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>()
  let config = services.BuildServiceProvider().GetRequiredService<IConfiguration>()
  services.AddCors() |> ignore
  services.AddGiraffe() |> ignore
  printfn "Migrations: %A"
  <| dbUpgrade (config.GetConnectionString("defaultConnection"))
  printfn "%A" <| config.GetConnectionString("conn")
  // let serializer = NewtonsoftJsonSerializer.DefaultSettings
  // serializer.Converters.Add(NodaPatternConverter<NodaTime.ZonedDateTime>(NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:sso<g>",NodaTime.DatetimeZoneProvider.Tzdb)))
  ignore <| NpgsqlConnection.GlobalTypeMapper.UseNodaTime()
  ignore <| services

let configureLogging (builder: ILoggingBuilder) = builder.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =
  let contentRoot = Directory.GetCurrentDirectory()
  Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(fun webHostBuilder ->
      webHostBuilder
        .UseContentRoot(contentRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
      |> ignore)
    .Build()
    .Run()
  0
