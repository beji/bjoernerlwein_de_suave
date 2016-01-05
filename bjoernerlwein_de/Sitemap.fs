module Sitemap

open Suave.Successful
open Staticpages
open Posts
open Types
open Suave.Files
open Suave.Filters
open Suave.Operators

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

let route = GET >=> path "/sitemap" >=> OK urlList
