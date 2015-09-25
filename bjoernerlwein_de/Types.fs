module Types


type Cfg =
    | Production
    | Debug
type Content = {title : string; content : string; id : string; date : string}
type ListType =
    | StaticPages
    | Posts
