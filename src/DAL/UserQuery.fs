module UserQuery

open Npgsql.FSharp

let connectionString : string =
    Sql.host Env.pgHost
    |> Sql.database Env.pgDbName
    |> Sql.username Env.pgUsername
    |> Sql.password Env.pgPassword
    |> Sql.port (int Env.pgPort)
    |> Sql.formatConnectionString  

