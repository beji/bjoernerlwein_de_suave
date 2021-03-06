﻿module Staticpages


open Suave
open Suave.Successful
open Suave.RequestErrors
open Suave.Utils
open Suave.Filters
open System.IO
open Newtonsoft.Json
open FSharp.Markdown
open Types
open Suave.Operators

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
        path "/staticpages" >=> Writers.setMimeType "application/json"
            >=> OK (JsonConvert.SerializeObject (List.map (fun (item:Content) ->
                {item with content = ""} //No need to return the full html content, it isn't used anyways
            ) staticpageCollection))
        path "/staticpages/show" >=> Writers.setMimeType "text/html" >=> OK Templates.staticpage
        pathScan "/staticpage/%s" (fun (id) ->
            try
                staticpageCollection
                |> List.find (fun (item) ->
                    item.id = id)
                |> JsonConvert.SerializeObject
                |> OK
            with
            | :? System.Collections.Generic.KeyNotFoundException as msg -> NOT_FOUND "404 not found")
            >=> Writers.setMimeType "application/json"]
