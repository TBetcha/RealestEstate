module UserHandlers

open Giraffe
open Serilog
open Types.User

let handlerWithLogging2: HttpHandler =
  handleContext (fun ctx ->
    task {
      let! userInput = ctx.BindJsonAsync<UserCreateDto>()
      Log.Information("From the logger")

      // Do more async stuff
      return! ctx.WriteTextAsync "Done working"
    })
