module Posts
    open Suave
    open Suave.Http
    open Suave.Http.Applicatives
    open Suave.Http.Successful
    open Suave.Http.RequestErrors
    open Suave.Utils
    open System.IO

    let routes =
        choose [
            GET >>= path "/posts/index" >>= Files.file "./static/html/posts.html"
            GET >>= path "/posts" >>= OK "{}"]

