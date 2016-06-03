module TwilightImperium
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open Suave.Utils
open Logger
open Newtonsoft.Json

type result = {name: string; race: string}

let races = 
    [|"Federation of Sol"; "Sardakk N'Orr"; "The Barony of Letnev"; "The Emirates of Hacan";
        "The L1Z1X Mindnet"; "The Mentak Coalition"; "The Naalu Collective"; "The Xxcha Kingdom"; "The Yssaril Tribes";
        "Universities of Jol-Nar"; "Clan of Saar"; "Embers of Muaat"; "The Winnu"; "The Yin Brotherhood"; "The Arborec"; "The Ghosts of Creuss"; "The Lazax"; "The Nekro Virus"|]
    |> Array.sort

let KnuthShuffle (lst : array<'a>) =                   // '
    let Swap i j =                                                  // Standard swap
        let item = lst.[i]
        lst.[i] <- lst.[j]
        lst.[j] <- item
    
    let ln = lst.Length
    [0..(ln - 2)]                                                   // For all indices except the last
    |> Seq.iter (fun i -> Swap i (ThreadSafeRandom.next i ln))                 // swap th item at the index with a random one following it (or itself)
    lst                                                             // Return the list shuffled in place

let prepareRequest (reqString:string) : string list =
    reqString.Split([|'|'|])
    |> Array.toList
     

let testRequest = "Berlwein|Staumann|Krusator|Joe"
    
let shuffleAndCutHead races =
    let tmp = KnuthShuffle races
    Array.head tmp, Array.tail tmp

let getRacesForPlayers requestString races =

    let rec _internalLoop (names:string list) (races:string array) (acc: result list) =
        match names with
        | [] -> acc
        | name::n_tail ->
            let race, r_tail = shuffleAndCutHead races
            let pair = {name=name; race=race}
            _internalLoop n_tail r_tail (pair :: acc)
            
    
    let names = prepareRequest requestString
    
    _internalLoop names races []
    |> List.rev
    
let routes = 
    path "/ti" >=> Writers.setMimeType "application/json" >=> request (fun req ->
        let players = req.formData "players"
        match players with
        | Choice1Of2 players -> 
            let resp = 
                getRacesForPlayers players races
                |> JsonConvert.SerializeObject            
            OK resp
        | Choice2Of2 x ->
            logError x
            BAD_REQUEST x
        )