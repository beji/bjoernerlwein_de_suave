module Static

    open Suave
    open Suave.Http
    open Suave.Http.Applicatives
    open Suave.Http.Successful
    open Suave.Http.RequestErrors
    open System.IO
    open Types


    let styleCss mode = 
        let path = 
            match mode with
            | Production -> "./static/css/style.min.css"
            | Debug -> "./static/css/style.css"
        match File.Exists path with
        | false -> 
            printfn "could not open %s" path
            NOT_FOUND "File not found"
        | true ->
            File.ReadAllText path
            |> OK
