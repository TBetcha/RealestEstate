module RealestEstate.Routes

open Giraffe

let routes: HttpHandler =
  choose
    [ POST >=> route "/user/create"
      route "/bar" >=> text "Bar"

      // If none of the routes matched then return a 404
      RequestErrors.NOT_FOUND "Not Found" ]
