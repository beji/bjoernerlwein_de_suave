module Posts

open Suave
open Suave.Successful
open Suave.RequestErrors
open Suave.Operators
open Suave.Filters
open Suave.Utils
open System.IO
open System.IO
open Newtonsoft.Json
open FSharp.Markdown
open Types

let postCollection =
    Directory.EnumerateFiles("./content/posts/")
    |> Seq.map (fun path ->
        let content = File.ReadAllText path
        let converted = JsonConvert.DeserializeObject<Content>(content)
        let parsedContent = Markdown.Parse(converted.content)
        {converted with
            content = Markdown.WriteHtml(parsedContent)}
    )
    |> Seq.sortByDescending (fun (item:Content) -> System.DateTime.Parse item.date)
    |> Seq.toList


let routes =
    choose [
        path "/posts/index" >=> Writers.setMimeType "text/html" >=> OK Templates.posts
        path "/posts" >=> Writers.setMimeType "application/json" >=> OK (JsonConvert.SerializeObject postCollection)
        path "/posts/show" >=> Writers.setMimeType "text/html" >=> OK Templates.post
        pathScan "/post/%s" (fun (id) ->
            try
                postCollection
                |> List.find (fun (item) ->
                    item.id = id)
                |> JsonConvert.SerializeObject
                |> OK
            with
            | :? System.Collections.Generic.KeyNotFoundException as msg -> NOT_FOUND "404 not found")
            >=> Writers.setMimeType "application/json"]
