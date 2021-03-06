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

module ProjectParsing

open System
open System.IO
open System.Linq
open System.Xml.Linq
open Anthology
open StringHelpers
open MsBuildHelpers
open Env
open Collections
open System.Text.RegularExpressions

type ProjectDescriptor = 
    { Packages : Package set
      Project : Project }

let extractOutput(xdoc : XDocument) = 
    let xoutput = xdoc.Descendants(NsMsBuild + "AssemblyName").Single()
    let soutput = !> xoutput : string
    soutput

let getProjectOutput (dir : DirectoryInfo) (relFile : string) = 
    let file = dir |> IoHelpers.GetFile relFile
    let xdoc = XDocument.Load(file.FullName)
    extractOutput xdoc

let getProjectReferences (prjDir : DirectoryInfo) (xdoc : XDocument) = 
    // VS project references
    let prjRefs = xdoc.Descendants(NsMsBuild + "ProjectReference")
                  |> Seq.map (fun x -> !> x.Attribute(XNamespace.None + "Include") : string)
                  |> Seq.map (fun x -> getProjectOutput prjDir x |> ProjectId.from)
                  |> Set
    
    // full-build project references (once converted)
    let fbRefs = xdoc.Descendants(NsMsBuild + "Import")
                 |> Seq.map (fun x -> !> x.Attribute(XNamespace.None + "Project") : string)
                 |> Seq.filter (fun x -> x.StartsWith(MSBUILD_PROJECT_FOLDER) || x.StartsWith(MSBUILD_PROJECT_FOLDER2))
                 |> Seq.map (fun x -> Path.GetFileNameWithoutExtension x |> ProjectId.from)
                 |> Set
    
    prjRefs |> Set.union fbRefs

let getAssemblies(xdoc : XDocument) : AssemblyId set = 
    let res = seq { 
        for binRef in xdoc.Descendants(NsMsBuild + "Reference") do
            let inc = !> binRef.Attribute(XNamespace.None + "Include") : string
            let assName = inc.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries).[0]
            let assRef = AssemblyId.from (System.Reflection.AssemblyName(assName))
            yield assRef
    }
    res |> Set

let parseNuGetPackage (pkgRef : XElement) : Package =
    let pkgId : string = !> pkgRef.Attribute(XNamespace.None + "id")
    let pkgVer = !> pkgRef.Attribute(XNamespace.None + "version") : string
    { Id = PackageId.from pkgId
      Version = PackageVersion pkgVer }

let parseFullBuildPackage (fileName : string) : Package =
    let fi = FileInfo (fileName)
    let fo = fi.Directory.Name

    { Id = PackageId.from fo
      Version = Unspecified }

let getNuGetPackages (nugetDoc : XDocument) =
    let nugetPkgs = nugetDoc.Descendants(XNamespace.None + "package") |> Seq.map parseNuGetPackage 
                                                                      |> Set
    nugetPkgs

let isPaketReference (xel : XElement) =
    let hasPaket = xel.Descendants(NsMsBuild + "Paket").Any() 
    let hasHintPath = xel.Descendants(NsMsBuild + "HintPath").Any() 
    hasPaket && hasHintPath

let (|MatchPackage|_|) hintpath =
    let m = Regex.Match (hintpath, @".*\\packages\\(?<Package>[^\\]*).*")
    if m.Success then Some (m.Groups.["Package"].Value)
    else None

let getPackageFromPaketReference (xel : XElement) =
    let xhintPath = xel.Descendants(NsMsBuild + "HintPath") |> Seq.head
    let hintPath = !> xhintPath : string
    match hintPath with
    | MatchPackage pkg -> { Id = PackageId.from pkg
                            Version = Unspecified }
    | _ -> failwith "Failed to find package"

let getFullBuildPackages (prjDoc : XDocument)  =
    let fbPkgs = prjDoc.Descendants(NsMsBuild + "Import")
                 |> Seq.map (fun x -> !> x.Attribute(XNamespace.None + "Project") : string)
                 |> Seq.filter (fun x -> x.StartsWith(MSBUILD_PACKAGE_FOLDER) || x.StartsWith(MSBUILD_PACKAGE_FOLDER2))
                 |> Seq.map parseFullBuildPackage
                 |> Set
    fbPkgs

let getPaketPackages (prjDoc : XDocument)  =
    let paketPkgs = prjDoc.Descendants(NsMsBuild + "Reference")
                    |> Seq.filter isPaketReference
                    |> Seq.map getPackageFromPaketReference
                    |> Set
    paketPkgs

let parseProjectContent (xdocLoader : FileInfo -> XDocument option) (repoDir : DirectoryInfo) (repoRef : RepositoryId) (file : FileInfo) =
    let relativeProjectFile = IoHelpers.ComputeRelativePath repoDir file
    let xprj = match xdocLoader file with
               | Some x -> x
               | _ -> failwithf "Failed to load project %A" file.FullName
    let xguid = !> xprj.Descendants(NsMsBuild + "ProjectGuid").Single() : string
    let guid = ParseGuid xguid
    let assemblyName = !> xprj.Descendants(NsMsBuild + "AssemblyName").Single() : string
    let assemblyRef = AssemblyId.from assemblyName
    let projectRef = ProjectId.from assemblyName
    
    let extension =  match !> xprj.Descendants(NsMsBuild + "OutputType").Single() : string with
                     | "Library" -> OutputType.Dll
                     | _ -> OutputType.Exe
    
    let sfxTarget = !> xprj.Descendants(NsMsBuild + "TargetFrameworkVersion").SingleOrDefault() : string
    let fxTarget = if sfxTarget <> null then sfxTarget
                   else "v4.5"

    let prjRefs = getProjectReferences file.Directory xprj
    
    let assemblies = getAssemblies xprj
    let pkgFile = file.Directory |> IoHelpers.GetFile "packages.config"
    let nugetPackages = match xdocLoader pkgFile with
                        | Some xnuget -> getNuGetPackages xnuget
                        | _ -> Set.empty
    let fbPackages = getFullBuildPackages xprj
    let paketPackages = getPaketPackages xprj
    let packages = nugetPackages |> Set.union fbPackages 
                                 |> Set.union paketPackages
    let pkgRefs = packages |> Set.map (fun x -> x.Id)

    { Packages = packages
      Project = { Repository = repoRef
                  RelativeProjectFile = ProjectRelativeFile relativeProjectFile
                  UniqueProjectId = ProjectUniqueId.from guid
                  ProjectId = projectRef
                  Output = assemblyRef
                  OutputType = extension
                  FxTarget = FrameworkVersion fxTarget
                  AssemblyReferences = assemblies
                  PackageReferences = pkgRefs
                  ProjectReferences = prjRefs } }

let ParseProject (repoDir : DirectoryInfo) (repoRef : RepositoryId) (file : FileInfo) : ProjectDescriptor = 
    try
        parseProjectContent IoHelpers.XDocLoader repoDir repoRef file
    with 
        e -> exn(sprintf "Failed to parse project %A" (file.FullName), e) |> raise
