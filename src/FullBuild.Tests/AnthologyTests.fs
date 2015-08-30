﻿module AnthologyTests

open NUnit.Framework
open FsUnit
open Anthology
open StringHelpers

[<Test>]
let CheckReferences () =
    AssemblyId.Bind "badaboum" |> should equal <| AssemblyId.Bind "BADABOUM"

    PackageId.Bind "badaboum" |> should equal <| PackageId.Bind "BADABOUM"

    RepositoryId.Bind "badaboum" |> should equal <| RepositoryId.Bind "BADABOUM"

[<Test>]
let CheckToRepository () =
    let (ToRepository repoGit) = ("git", "https://github.com/pchalamet/cassandra-sharp", "cassandra-sharp")
    repoGit |> should equal { Vcs = VcsType.Git
                              Name = RepositoryId.Bind "cassandra-sharp"
                              Url = RepositoryUrl "https://github.com/pchalamet/cassandra-sharp" } 

    let (ToRepository repoHg) = ("hg", "https://github.com/pchalamet/cassandra-sharp", "cassandra-sharp")
    repoHg |> should equal { Vcs = VcsType.Hg
                             Name = RepositoryId.Bind "cassandra-sharp"
                             Url = RepositoryUrl "https://github.com/pchalamet/cassandra-sharp" } 

    (fun () -> let (ToRepository repo) = ("pouet", "https://github.com/pchalamet/cassandra-sharp", "cassandra-sharp")
               ())
        |> should throw typeof<System.Exception>

[<Test>]
let CheckEqualityWithPermutation () =
    let antho1 = {
        Applications = Set.empty
        Bookmarks = [ { Name = BookmarkName "cassandra-sharp"; Version = BookmarkVersion "b62e33a6ba39f987c91fdde11472f42b2a4acd94" }; { Name = BookmarkName "cassandra-sharp-contrib"; Version = BookmarkVersion "e0089100b3c5ca520e831c5443ad9dc8ab176052" } ] |> set
        Repositories = [ { Vcs = VcsType.Git; Name = RepositoryId.Bind "cassandra-sharp"; Url = RepositoryUrl "https://github.com/pchalamet/cassandra-sharp" }
                         { Vcs = VcsType.Git; Name = RepositoryId.Bind "cassandra-sharp-contrib"; Url = RepositoryUrl "https://github.com/pchalamet/cassandra-sharp-contrib" } ] |> set
        Projects = [ { Output = AssemblyId.Bind "cqlplus"
                       OutputType = OutputType.Exe
                       ProjectGuid = ProjectId (ParseGuid "0a06398e-69be-487b-a011-4c0be6619b59")
                       RelativeProjectFile = ProjectRelativeFile "cqlplus/cqlplus-net45.csproj"
                       FxTarget = FrameworkVersion "v4.5"
                       ProjectReferences = [ ProjectId.Bind (ParseGuid "6f6eb447-9569-406a-a23b-c09b6dbdbe10"); ProjectId.Bind(ParseGuid "c1d252b7-d766-4c28-9c46-0696f896846c") ] |> set
                       AssemblyReferences = [ AssemblyId.Bind "System" ; AssemblyId.Bind "System.Data"; AssemblyId.Bind "System.Xml"] |> set
                       PackageReferences = Set.empty
                       Repository = RepositoryId.Bind "cassandra-sharp" } ] |> set }

    let antho2 = {
        Applications = Set.empty
        Bookmarks = [ { Name = BookmarkName "cassandra-sharp-contrib"; Version = BookmarkVersion "e0089100b3c5ca520e831c5443ad9dc8ab176052" }; { Name = BookmarkName "cassandra-sharp"; Version = BookmarkVersion "b62e33a6ba39f987c91fdde11472f42b2a4acd94" } ] |> set
        Repositories = [ { Vcs = VcsType.Git; Name = RepositoryId.Bind "cassandra-sharp-contrib"; Url = RepositoryUrl "https://github.com/pchalamet/cassandra-sharp-contrib" } 
                         { Vcs = VcsType.Git; Name = RepositoryId.Bind "cassandra-sharp"; Url = RepositoryUrl "https://github.com/pchalamet/cassandra-sharp" } ] |> set
        Projects = [ { Output = AssemblyId.Bind "cqlplus"
                       OutputType = OutputType.Exe
                       ProjectGuid = ProjectId (ParseGuid "0a06398e-69be-487b-a011-4c0be6619b59")
                       RelativeProjectFile = ProjectRelativeFile "cqlplus/cqlplus-net45.csproj"
                       FxTarget = FrameworkVersion "v4.5"
                       ProjectReferences = [ ProjectId.Bind(ParseGuid "c1d252b7-d766-4c28-9c46-0696f896846c"); ProjectId.Bind (ParseGuid "6f6eb447-9569-406a-a23b-c09b6dbdbe10") ] |> set
                       AssemblyReferences = [ AssemblyId.Bind "System" ; AssemblyId.Bind "System.Xml"; AssemblyId.Bind "System.Data" ] |> set
                       PackageReferences = Set.empty
                       Repository = RepositoryId.Bind "cassandra-sharp" } ] |> set }
        
    antho1 |> should equal antho2