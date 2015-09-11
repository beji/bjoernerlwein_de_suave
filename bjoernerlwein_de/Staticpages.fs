module Staticpages


open Suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Utils
open System.IO
open Newtonsoft.Json
open FSharp.Markdown
open Types

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

let routes =
    choose [
        GET >>= path "/staticpages" >>= Writers.setMimeType "application/json"
            >>= OK (JsonConvert.SerializeObject (List.map (fun (item:Content) ->
                {item with content = ""} //No need to return the full html content, it isn't used anyways
            ) staticpageCollection))
        GET >>= path "/staticpages/show" >>= Writers.setMimeType "text/html" >>= OK Templates.staticpage
        GET >>= pathScan "/staticpage/%s" (fun (id) ->
            try
                staticpageCollection
                |> List.find (fun (item) ->
                    item.id = id)
                |> JsonConvert.SerializeObject
                |> OK
            with
            | :? System.Collections.Generic.KeyNotFoundException as msg -> NOT_FOUND "404 not found")
            >>= Writers.setMimeType "application/json"]
