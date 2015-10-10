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

let timestamp = System.DateTime.UtcNow.ToString("yyyyMMddHHmmss")

let logRoute (context:HttpContext) =

  let useragent =
    List.pick (fun tpl ->
      match tpl with
      | ("user-agent", x) -> Some(x)
      | _ -> None
      ) context.request.headers

  "<<< " + (string context.request.url.PathAndQuery) + " from " + (string context.request.ipaddr) + " | " + useragent
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
    printfn "%s" context.request.url.LocalPath
    let key = "ETag"
    let value = timestamp + path |> MD5Hash
    { context with response = { context.response with headers = (key, value) :: (context.response.headers |> List.filter (fun (k,_) -> k <> key))  } }
    |> succeed

let expireHead = Writers.setHeader "Cache-Control" ("max-age=" + (string (60*60*24*30)))

let app mode : WebPart =

  let routes = [StaticfileRoutes.routes
                Posts.routes
                Staticpages.routes
                Sitemap.route
                GET >>= path "/" >>= OK (page mode)
                NOT_FOUND "404 not found"]

  match mode with
  | Debug -> logRoute >>= choose routes
  | Production -> logRoute >>= expireHead >>= setEtag >>= choose routes

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
