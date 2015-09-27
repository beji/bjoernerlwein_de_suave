module Logger

open Logary
open Logary.Configuration
open Logary.Targets
open Suave.Logging

let logary =
  withLogary' "BjoernerlweinDe" (
    // a new allow-all rule for 'console' with a 'console' target
    withRule (
      {Rule.empty with target = "console"; level = Logary.LogLevel.Debug }) //No real need for the verbose log
    >> withTarget (Console.create Console.empty "console")
  )
let logger = logary.GetLogger("BjoernerlweinDe.main")

let logMsg =
  Logger.debug logger "logMsg is deprecated, use logDbg instead"
  Logger.debug logger
let logDbg = Logger.debug logger
let logInfo = Logger.info logger

let adapter = SuaveAdapter logger
