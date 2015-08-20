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
open Newtonsoft.Json
open FSharp.Markdown

//let testjson = File.ReadAllText "./content/pages/impressum.json"

//let deserialized = JsonConvert.DeserializeObject<Content> testjson

let staticpageCollection =
    Directory.EnumerateFiles("./content/pages/")
    |> Seq.map (fun path ->
        let content = File.ReadAllText path
        let converted = JsonConvert.DeserializeObject<Content>(content)
        let parsedContent = Markdown.Parse(converted.content)
        {converted with
            content = Markdown.WriteHtml(parsedContent)}
    )
    |> Seq.toList

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
        GET >>= path "/" >>= OK (page mode)
        GET >>= path "/js/angular.js" >>= Files.file "./static/bower/angular/angular.js"
        GET >>= path "/js/angular.route.js" >>= Files.file "./static/bower/angular-route/angular-route.js"
        GET >>= path "/js/angular.viewhead.js" >>= Files.file "./static/bower/angularjs-viewhead/angularjs-viewhead.js"
        GET >>= path "/js/script.js" >>= Files.file "./static/js/script.js"
        GET >>= path "/js/script.min.js" >>= Files.file "./static/js/script.min.js"
        GET >>= path "/staticpages" >>= Writers.setMimeType "application/json" 
            >>= OK (JsonConvert.SerializeObject (List.map (fun (item:Content) ->
                {item with content = ""}
            ) staticpageCollection))
        GET >>= path "/staticpages/show" >>= Files.file "./static/html/staticpage.html"
        GET >>= pathScan "/staticpage/%s" (fun (id) ->
            try
                staticpageCollection
                |> List.find (fun (item) ->
                    item.id = id)
                |> JsonConvert.SerializeObject
                |> OK
            with
            | :? System.Collections.Generic.KeyNotFoundException as msg -> NOT_FOUND "404 not found")
            >>= Writers.setMimeType "application/json"
        GET >>= path "/posts/index" >>= Files.file "./static/html/posts.html"
        GET >>= path "/posts" >>= OK "{}"
        pathScan "/css/%s" (fun (file) -> 
            let path = "./static/css/" + file
            match File.Exists path with
            | false -> NOT_FOUND "404 not found"
            | true -> Files.file path)
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

