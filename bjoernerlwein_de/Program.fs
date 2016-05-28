// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Suave
open Suave.Successful
open Suave.RequestErrors
open Suave.Web
open Suave.Utils
open Suave.Filters
open Suave.Operators
open System.IO
open Types
open Templates

let timestamp = System.DateTime.UtcNow.ToString("yyyyMMddHHmmss")

let logRoute (context:HttpContext) =

  let useragent =
    List.pick (fun tpl ->
      match tpl with
      | ("user-agent", x) -> Some(x)
      | _ -> None
      ) context.request.headers
  "<<< " + (string context.request.url.PathAndQuery) + " from " + (string context.clientIpTrustProxy) + " | " + useragent
  |> Logger.logInfo

  async.Return <| Some context

let MD5Hash (input : string) =
   use md5 = System.Security.Cryptography.MD5.Create()
   input
   |> System.Text.Encoding.ASCII.GetBytes
   |> md5.ComputeHash
   |> Seq.map (fun c -> c.ToString("X2"))
   |> Seq.reduce (+)

let setEtag (context:HttpContext) =
    let path = context.request.url.LocalPath
    let key = "ETag"
    let value = timestamp + path |> MD5Hash
    { context with response = { context.response with headers = (key, value) :: (context.response.headers |> List.filter (fun (k,_) -> k <> key))  } }
    |> succeed

let expireHead = Writers.setHeader "Cache-Control" ("max-age=" + (string (60*60*24*30)))

let app mode : WebPart =

  let routes =
    choose [
        POST >=> TwilightImperium.routes
        GET >=> choose [StaticfileRoutes.routes
                        Posts.routes
                        Staticpages.routes
                        Sitemap.route
                        path "/" >=> OK (page mode)
                        NOT_FOUND "404 not found"]
    ]
  match mode with
  | Debug -> logRoute >=> routes
  | Production -> logRoute >=> expireHead >=> setEtag >=> routes

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
