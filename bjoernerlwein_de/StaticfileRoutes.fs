module StaticfileRoutes

open Suave
open Suave.RequestErrors
open Suave.Files
open Suave.Filters
open System.IO
open Suave.Operators

let routes =
    choose [
        path "/js/vue.js" >=> Files.file "./static/bower/vue/dist/vue.js"
        path "/js/nanoajax.js" >=> Files.file "./static/bower/nanoajax/nanoajax.min.js"
        path "/js/script.js" >=> Files.file "./static/js/script.js"
        path "/js/script.min.js" >=> Files.file "./static/js/script.min.js"
        path "/css/normalize.css" >=> Files.file "./static/bower/normalize-css/normalize.css"
        pathScan "/css/%s" (fun (file) ->
            let path = "./static/css/" + file
            match File.Exists path with
            | false -> NOT_FOUND "404 not found"
            | true -> Files.file path)]
