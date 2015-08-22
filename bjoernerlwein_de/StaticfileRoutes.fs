﻿module StaticfileRoutes
    open Suave
    open Suave.Http
    open Suave.Http.Applicatives
    open Suave.Http.RequestErrors
    open Suave.Utils
    open System.IO

    let routes =
        choose [
            GET >>= path "/js/angular.js" >>= Files.file "./static/bower/angular/angular.js"
            GET >>= path "/js/angular.route.js" >>= Files.file "./static/bower/angular-route/angular-route.js"
            GET >>= path "/js/angular.viewhead.js" >>= Files.file "./static/bower/angularjs-viewhead/angularjs-viewhead.js"
            GET >>= path "/js/script.js" >>= Files.file "./static/js/script.js"
            GET >>= path "/js/script.min.js" >>= Files.file "./static/js/script.min.js"
            pathScan "/css/%s" (fun (file) -> 
                let path = "./static/css/" + file
                match File.Exists path with
                | false -> NOT_FOUND "404 not found"
                | true -> Files.file path)]