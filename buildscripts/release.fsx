#I "../packages/FAKE/tools/"
#r "../packages/FAKE/tools/FakeLib.dll"
#load "./util.fsx"

open Fake

"Clean"
==> "Bower"
==> "CopyStaticfiles"
==> "CopyContent"
==> "CssMin"
==> "JsMin"
==> "BuildRelease"
==> "RunRelease"

RunTargetOrDefault "RunRelease"
