#I "../packages/FAKE/tools/"
#r "../packages/FAKE/tools/FakeLib.dll"
#load "./util.fsx"

open Fake

"Clean"
==> "CopyStaticfiles"
==> "CopyContent"
==> "BuildDebug"
==> "RunDebug"

RunTargetOrDefault "RunDebug"
