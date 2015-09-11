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
open ListML.Core
open ListML.HTML

let timestamp =
    let zone = System.TimeZone.CurrentTimeZone.GetUtcOffset System.DateTime.Now
    System.DateTime.UtcNow.ToString("yyyyMMddHHmmss")

let page mode =

    let stylesheets =
      match mode with
      | Debug -> ["normalize.css"; "style.css"]
      | Production -> ["style.min.css"]
      |> List.fold (fun acc elem ->
          acc + link [href ("/css/" + elem + "?v=" + timestamp); attr "rel" "stylesheet"] []
        ) ""

    let scripts =
      match mode with
      | Debug -> ["angular.js";"angular.route.js";"angular.viewhead.js";"script.js"];
      | Production -> ["script.min.js"]
      |> List.fold (fun acc elem ->
          acc + script [attr "src" ("/js/" + elem + "?v=" + timestamp)] [""]
        ) ""

    doctype +
    html [attr "lang" "de"; attr "ng-app" "bjoernerlweinde"] [
      head [] [
        meta [attr "charset" "UTF-8"]
        title [attr "ng-bind-template" "Bjoernerlwein.de | {{viewTitle}}"] ["Bjoernerlwein.de"]
        stylesheets
        link [href "http://fonts.googleapis.com/css?family=Source+Sans+Pro:400,700,400italic,700italic"; attr "rel" "stylesheet"; attr "type" "text/css"] []
        scripts
        ]
      body [] [
        div [classAttr "sidebar"] [
          header [] [
            h1 [] [
              a [attr "ng-href" "/#/"; href "/"] ["Bjoernerlwein.de"]]
            small [classAttr "sub-header"] ["{{viewTitle}}"]]
          nav [attr "data-ng-controller" "staticPagesController"; attr "data-ng-init" "index()"; classAttr "navigation"] [
            h2 [] ["Pages"]
            ul [] [
              li [attr "ng-repeat" "page in pages"] [
                a [href "/#/staticpage/{{page.id}}"] ["{{page.title}}"]]]]
          footer [] [
            a [href "http://creativecommons.org/licenses/by-sa/4.0/"; attr "rel" "license"] [
              img [src "http://i.creativecommons.org/l/by-sa/4.0/88x31.png"; alt "Creative Commons License"; attr "style" "border-width: 0;"]]
            br []
            span [] ["bjoernerlwein.de"]
            " by "
            a [href "http://bjoernerlwein.de"; attr "rel" "cc:attributionURL"] ["Björn Erlwein"]
            " is licensed under a "
            a [href "http://creativecommons.org/licenses/by-sa/4.0/"; attr "rel" "license"] ["Creative Commons Attribution-ShareAlike 4.0
              International License"]
            "."]]
        div [classAttr "content"; attr "ng-view" "ng-view"] []]]


let app mode : WebPart =

    choose [
        StaticfileRoutes.routes
        Posts.routes
        Staticpages.routes
        Sitemap.route
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
