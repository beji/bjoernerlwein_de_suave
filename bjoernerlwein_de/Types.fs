module Types
    type Cfg =
        | Production
        | Debug
    type PageVars = { Stylesheets : string []; Scripts : string[]; timestamp: string}
    type Content = {title : string; show : bool; content : string; id : string}
    type PagesCollection = {pages: Content list}
    type PostsCollection = {posts: Content list}