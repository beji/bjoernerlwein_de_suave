module Logger

    open Logary
    open Logary.Configuration
    open Logary.Targets
    open Suave.Logging
    
    let logary =
      withLogary' "BjoernerlweinDe" (
        // a new allow-all rule for 'console' with a 'console' target
        withRule (Rule.createForTarget "console")
        >> withTarget (Console.create Console.empty "console")
      )
    let logger = logary.GetLogger("BjoernerlweinDe.main")

    let logMsg = Logger.debug logger

    let adapter = SuaveAdapter logger