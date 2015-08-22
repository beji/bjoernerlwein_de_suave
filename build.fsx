#I "packages/FAKE.4.1.3/tools/"
#r "packages/FAKE.4.1.3/tools/FakeLib.dll"
#r "packages/AjaxMin.5.14.5506.26202/lib/net40/AjaxMin.dll"

open Fake
open System.IO
open Microsoft.Ajax.Utilities

let buildDir = "./build"

let writeToFile path content = File.WriteAllText(path, content)

let append right left = left + right

let minifier = new Minifier()

let compressCss = minifier.MinifyStyleSheet 

let compressJs = minifier.MinifyJavaScript

Target "Bower" (fun _ ->
    Shell.Exec("bower", "install", "./") |> ignore)

Target "BuildApp" (fun _ ->
    ["./bjoernerlwein_de/bjoernerlwein_de.fsproj"]
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: ")

Target "CssMin" (fun _ ->
    let cssdir = buildDir + "/static/css/"
    cssdir + "pure.css"
    |> File.ReadAllText
    |> append (File.ReadAllText (cssdir + "pure.responsive.css"))
    |> append (File.ReadAllText (cssdir + "style.css"))
    |> compressCss
    |> writeToFile (cssdir + "style.min.css"))

Target "JsMin" (fun _ ->
    let jsDir = buildDir + "/static/"
    File.ReadAllText (jsDir + "bower/angular/angular.js")
    |> append (File.ReadAllText (jsDir + "/bower/angular-route/angular-route.js"))
    |> append (File.ReadAllText (jsDir + "/bower/angularjs-viewhead/angularjs-viewhead.js"))
    |> append (File.ReadAllText (jsDir + "/js/script.js"))
    |> compressJs
    |> writeToFile (jsDir + "js/script.min.js")
    |> ignore)

Target "Run" (fun _ ->
    Shell.Exec("bjoernerlwein_de.exe", "production", buildDir)
    |> ignore)

Target "Clean" (fun _ ->
    CleanDir buildDir)

"Clean"
==> "Bower"
==> "BuildApp"
==> "CssMin"
==> "JsMin"
==> "Run"

RunTargetOrDefault "Run"