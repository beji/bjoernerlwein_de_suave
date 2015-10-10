module Templates

open Types
open ListML.Core
open ListML.HTML

let page mode =

    let stylesheets =
      match mode with
      | Debug -> ["normalize.css"; "style.css"]
      | Production -> ["style.min.css"]
      |> List.fold (fun acc elem ->
          acc + link [href ("/css/" + elem);  "rel", "stylesheet"] []
        ) ""

    let scripts =
      match mode with
      | Debug -> ["angular.js";"angular.route.js";"angular.viewhead.js";"script.js"];
      | Production -> ["script.min.js"]
      |> List.fold (fun acc elem ->
          acc + script ["src", ("/js/" + elem); "async", "true"] [""]
        ) ""

    doctype +
    html ["lang","de"; "ng-app","bjoernerlweinde"] [
      head [] [
        meta ["charset", "UTF-8"]
        title ["ng-bind-template", "Bjoernerlwein.de | {{viewTitle}}"] ["Bjoernerlwein.de"]
        stylesheets
        link [href "http://fonts.googleapis.com/css?family=Source+Sans+Pro:400,700,400italic,700italic";  "rel", "stylesheet";  "type", "text/css"] []
        scripts
        ]
      body [] [
        div [classAttr "sidebar"] [
          header [] [
            h1 [] [
              a ["ng-href", "/#/"; href "/"] ["Bjoernerlwein.de"]]
            small [classAttr "sub-header"] ["{{viewTitle}}"]]
          nav ["data-ng-controller", "staticPagesController";  "data-ng-init", "index()"; classAttr "navigation"] [
            h2 [] ["Pages"]
            ul [] [
              li ["ng-repeat", "page in pages"] [
                a [href "/#/staticpage/{{page.id}}"] ["{{page.title}}"]]]]
          footer [] [
            a [href "http://creativecommons.org/licenses/by-sa/4.0/";  "rel", "license"] [
              img [src "http://i.creativecommons.org/l/by-sa/4.0/88x31.png"; alt "Creative Commons License";  "style", "border-width: 0;"]]
            br []
            span [] ["bjoernerlwein.de"]
            " by "
            a [href "http://bjoernerlwein.de";  "rel", "cc:ibutionURL"] ["Björn Erlwein"]
            " is licensed under a "
            a [href "http://creativecommons.org/licenses/by-sa/4.0/";  "rel", "license"] ["Creative Commons Attribution-ShareAlike 4.0
              International License"]
            "."]]
        div [classAttr "content";  "ng-view", "ng-view"] [""]]]

let post =
  div ["data-ng-controller", "postsController";  "data-ng-init", "show()"] [
    article ["id", "{{post.id}}"] [
      h1 ["view-title", "view-title"] ["{{post.title}}"]
      small [] ["{{toDateString(post.date)}}"]
      div ["ng-bind-html", "to_trusted(post.content)"] [""]]]

let posts =
  div ["data-ng-controller", "postsController";  "data-ng-init", "index()"] [
    node "view-title" [] ["Posts"]
    article ["id", "{{post.id}}";  "ng-repeat", "post in posts"] [
      h1 [] [
        a [href "#/post/{{post.id}}"] ["{{post.title}}"]]
      small [] ["{{toDateString(post.date)}}"]
      div ["ng-bind-html", "to_trusted(post.content)"] [""]]]

let staticpage =
  div ["data-ng-controller", "staticPagesController";  "data-ng-init", "show()"] [
    article ["id", "{{page.id}}"] [
      h1 ["view-title", "view-title"] ["{{page.title}}"]
      div ["ng-bind-html", "to_trusted(page.content)"] [""]]]
