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
open RazorEngine
open RazorEngine.Templating

//let testjson = File.ReadAllText "./content/pages/impressum.json"

//let deserialized = JsonConvert.DeserializeObject<Content> testjson

let template = File.ReadAllText "./static/html/layout.html"

let timestamp = 
    let zone = System.TimeZone.CurrentTimeZone.GetUtcOffset System.DateTime.Now
    System.DateTime.UtcNow.ToString("yyyyMMddHHmmss")

let page mode = 
    let pageVars = 
        match mode with
        | Debug -> 
            let pageVars:PageVars = 
                {Stylesheets = [|"pure.css";"style.css"|];
                Scripts = [|"angular.js";"angular.route.js";"angular.viewhead.js";"script.js"|];
                timestamp = timestamp}
            pageVars
        | Production ->
            let pageVars:PageVars = 
                {Stylesheets = [|"style.min.css"|];
                Scripts = [|"script.min.js"|];
                timestamp = timestamp}
            pageVars
    Engine.Razor.RunCompile(template, "basePage", typeof<PageVars>, pageVars)

let app mode : WebPart =

    choose [
        StaticfileRoutes.routes
        Posts.routes
        Staticpages.routes
        GET >>= path "/" >>= OK (page mode)
        NOT_FOUND "404 not found"]

[<EntryPoint>]
let main argv = 

    Logger.logMsg "Starting Web Server"

    let mode = 
        match argv.Length with
        | 1 ->
            match argv.[0] with
            | "production" -> Cfg.Production
            | _ -> Cfg.Debug
        | _ -> Cfg.Debug

    let webConfig =
        { defaultConfig with
            logger   = Logger.adapter
        }

    startWebServer webConfig <| app mode
    0 // return an integer exit code

