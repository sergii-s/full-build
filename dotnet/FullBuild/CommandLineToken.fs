﻿//   Copyright 2014-2016 Pierre Chalamet
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

module CommandLineToken


type TokenOption =
    | Debug
    | All
    | Bin
    | Src
    | Exclude
    | Multithread
    | Shallow
    | Default
    | Branch
    | Version
    | Sticky
    | Rebase
    | Optimize
    | Unknown

let (|TokenOption|) (token : string) =
    match token with
    | "--debug" -> TokenOption.Debug
    | "--all" -> TokenOption.All
    | "--bin" -> TokenOption.Bin
    | "--src" -> TokenOption.Src
    | "--exclude" -> TokenOption.Exclude
    | "--mt" -> TokenOption.Multithread
    | "--shallow" -> TokenOption.Shallow
    | "--default" -> TokenOption.Default
    | "--branch" -> TokenOption.Branch
    | "--version" -> TokenOption.Version
    | "--sticky" -> TokenOption.Sticky
    | "--rebase" -> TokenOption.Rebase
    | _ -> Unknown


type Token = 
    | Version
    | Workspace
    | Help
    | Upgrade
    | Setup
    | Init
    | Clone
    | Update
    | Build
    | Rebuild
    | Index
    | Convert
    | Push
    | Graph
    | Install
    | Simplify
    | Outdated
    | Publish
    | Pull
    | Checkout
    | Exec
    | Test
    | Alter
    | Open
    | Bind
    | History

    | Add
    | Drop
    | List
    | Describe

    | View
    | Repo
    | Package
    | NuGet
    | App

    | Clean
    | UpdateGuids
    | Migrate
    | Unknown


let (|Token|) (token : string) = 
    match token with
    | "version" -> Version
    | "workspace" -> Workspace

    | "help" -> Help
    | "upgrade" -> Upgrade
    | "setup" -> Setup
    | "init" -> Init
    | "clone" -> Clone
    | "update" -> Update
    | "build" -> Build
    | "rebuild" -> Rebuild
    | "index" -> Index
    | "convert" -> Convert
    | "push" -> Push
    | "graph" -> Graph
    | "install" -> Install
    | "outdated" -> Outdated
    | "publish" -> Publish
    | "pull" -> Pull
    | "checkout" -> Checkout
    | "exec" -> Exec
    | "clean" -> Clean
    | "test" -> Test
    | "alter" -> Alter
    | "open" -> Open
    | "bind" -> Bind
    | "history" -> History

    | "add" -> Add
    | "drop" -> Drop
    | "list" -> List
    | "describe" -> Describe

    | "view" -> View
    | "repo" -> Repo
    | "package" -> Package
    | "nuget" -> NuGet
    | "app" -> App

    | "update-guids" -> UpdateGuids
    | "migrate" -> Migrate
    | _ -> Unknown
