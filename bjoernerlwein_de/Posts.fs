module Posts
    open Suave
    open Suave.Http
    open Suave.Http.Applicatives
    open Suave.Http.Successful
    open Suave.Http.RequestErrors
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
            GET >>= path "/posts/index" >>= Files.file "./static/html/posts.html"
            GET >>= path "/posts" >>= Writers.setMimeType "application/json" >>= OK (JsonConvert.SerializeObject postCollection)
            GET >>= path "/posts/show" >>= Files.file "./static/html/post.html"
            GET >>= pathScan "/post/%s" (fun (id) ->
                try
                    postCollection
                    |> List.find (fun (item) ->
                        item.id = id)
                    |> JsonConvert.SerializeObject
                    |> OK
                with
                | :? System.Collections.Generic.KeyNotFoundException as msg -> NOT_FOUND "404 not found")
                >>= Writers.setMimeType "application/json"]

