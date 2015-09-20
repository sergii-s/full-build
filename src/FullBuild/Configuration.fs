﻿// Copyright (c) 2014-2015, Pierre Chalamet
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Pierre Chalamet nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL PIERRE CHALAMET BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
module Configuration

open System
open System.IO
open IoHelpers
open Anthology
open Env

let private WORKSPACE_CONFIG_FILE = ".full-build"

type WorkspaceConfiguration = 
    { Repositories : Repository list }

let DefaultGlobalIniFilename() = 
    let userProfileDir = DirectoryInfo (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile))
    let configFile = userProfileDir |> GetFile WORKSPACE_CONFIG_FILE
    configFile

let GlobalConfig () : GlobalConfiguration = 
    let filename = DefaultGlobalIniFilename ()
    if filename.Exists then ConfigurationSerializer.Load filename
    else 
        let (ToRepository repo) = ("git", ".full-build", String.Empty)
        {
            BinRepo = String.Empty
            Repository = repo
            NuGets = [RepositoryUrl.from "https://www.nuget.org/api/v2/"]
        }

let LoadAnthology() : Anthology = 
    let anthoFn = GetAnthologyFileName ()
    AnthologySerializer.Load anthoFn

let SaveAnthology(anthology : Anthology) = 
    let anthoFn = GetAnthologyFileName ()
    AnthologySerializer.Save anthoFn anthology

let LoadBaseline() : Baseline =
    let baselineFile = GetBaselineFileName ()
    BaselineSerializer.Load baselineFile

let SaveBaseline (baseline : Baseline) =
    let baselineFile = GetBaselineFileName ()
    BaselineSerializer.Save baselineFile baseline


let Migrate (file : string) =
    ()
