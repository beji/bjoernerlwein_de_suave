// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Web
open Suave.Types
open Suave.Utils


let greetings q =
  defaultArg (Option.ofChoice(q ^^ "name")) "World" |> sprintf "Hello %s"

let sample : WebPart =
    choose 
        [   path "/hello" >>= choose [
              GET  >>= request(fun r -> 
                  printfn "%A" r.url.LocalPath
                  OK <| greetings r.query)
              POST >>= request(fun r -> OK <| greetings r.form)
              RequestErrors.NOT_FOUND "Found no handlers" ]
            pathScan "/json/%s.json" (fun (json) ->
                OK json
            ) ]

[<EntryPoint>]
let main argv = 
    startWebServer defaultConfig sample
    0 // return an integer exit code

