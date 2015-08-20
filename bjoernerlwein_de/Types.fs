module Types
    type Cfg =
        | Production
        | Debug
    type PageVars = { Stylesheets : string []; Scripts : string[]; timestamp: string}
    type Content = {title : string; content : string; id : string}