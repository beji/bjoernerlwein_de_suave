module Sitemap


open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Staticpages
open Posts
open Types

let generateRoutesFromContentList contentList listType =
    let fixedPart = "http://bjoernerlwein.de/#/"
    List.map (fun (content:Content) ->
        match listType with
        | StaticPages -> fixedPart + "staticpage/" + content.id
        | Posts -> fixedPart + "post/" + content.id
    ) contentList

let urlList =
    List.concat [ ["http://bjoernerlwein.de"]; generateRoutesFromContentList staticpageCollection ListType.StaticPages; generateRoutesFromContentList postCollection ListType.Posts]
    |> String.concat "\n"

let route = GET >>= path "/sitemap" >>= OK urlList
