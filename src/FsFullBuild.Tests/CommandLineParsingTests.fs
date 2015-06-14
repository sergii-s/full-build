﻿module CommandLineParsingTests

open NUnit.Framework
open FsUnit
open Types
open CommandLineParsing

[<Test>]
let CheckUsageInvoked () =
    let result = ParseCommandLine [ "workspace"; "create" ]
    let expected = Command.Usage
    result |> should equal expected

[<Test>]
let CheckWorkspaceCreate () =
    let result = ParseCommandLine [ "workspace"; "create"; @"c:\toto" ]
    let (ToWorkspacePath wsPath) = @"c:\toto"
    let expected = Command.InitWorkspace (wsPath)
    result |> should equal expected

[<Test>]
let CheckWorkspaceIndex () =
    let result = ParseCommandLine [ "workspace"; "index" ]
    let expected = Command.IndexWorkspace
    result |> should equal expected

    
[<Test>]
let CheckWorkspaceUpdate () =
    let result = ParseCommandLine [ "workspace"; "update" ]
    let expected = Command.RefreshWorkspace
    result |> should equal expected