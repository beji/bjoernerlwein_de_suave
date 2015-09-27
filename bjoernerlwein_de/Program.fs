// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Web
open Suave.Types
open Suave.Utils
open System.IO
open Types
open Templates

let logRoute (context:HttpContext) =

  "<<< " + (string context.request.url.PathAndQuery) + " from " + (string context.request.ipaddr)
  |> Logger.logInfo

  async.Return <| Some context

let app mode : WebPart =
    logRoute >>=
    choose [
        StaticfileRoutes.routes
        Posts.routes
        Staticpages.routes
        Sitemap.route
        GET >>= path "/" >>= OK (page mode)
        NOT_FOUND "404 not found"]

[<EntryPoint>]
let main argv =

    Logger.logInfo "Starting Web Server"

    let mode =
        match argv.Length with
        | 1 ->
            match argv.[0] with
            | "production" -> Cfg.Production
            | _ -> Cfg.Debug
        | _ -> Cfg.Debug

    let webConfig =
        match mode with
        | Debug ->
            { defaultConfig with
                logger   = Logger.adapter
            }
        | Production ->
            { defaultConfig with
                logger   = Logger.adapter
                bindings = [HttpBinding.mk HTTP (System.Net.IPAddress.Parse "0.0.0.0") 8083us]
            }

    startWebServer webConfig <| app mode
    0 // return an integer exit code
