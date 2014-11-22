﻿// Copyright (c) 2014, Pierre Chalamet
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

using System;
using System.Linq;
using System.Text.RegularExpressions;
using FullBuild.Config;
using FullBuild.Helpers;
using FullBuild.Model;
using FullBuild.SourceControl;

namespace FullBuild.Commands
{
    internal partial class Workspace
    {
        public void ListRepos()
        {
            var admDir = WellKnownFolders.GetAdminDirectory();
            var config = ConfigManager.LoadAdminConfig(admDir);

            config.SourceRepos.ForEach(x => Console.WriteLine(x.Name));
        }

        public void AddRepo(string name, VersionControlType type, string url)
        {
            var admDir = WellKnownFolders.GetAdminDirectory();
            var config = ConfigManager.LoadAdminConfig(admDir);

            var newRepo = new RepoConfig
                          {
                              Name = name,
                              Vcs = type,
                              Url = url
                          };

            config.SourceRepos = config.SourceRepos.Concat(new[] {newRepo}).ToArray();

            ConfigManager.SaveAdminConfig(admDir, config);
        }

        public void CloneRepo(string[] repos)
        {
            var wsDir = WellKnownFolders.GetWorkspaceDirectory();
            var config = ConfigManager.LoadConfig(wsDir);

            // validate first that repos are valid and clone them
            foreach(var repo in repos)
            {
                var match = "^" + repo + "$";
                var regex = new Regex(match, RegexOptions.IgnoreCase);
                var repoConfigs = config.SourceRepos.Where(x => regex.IsMatch(x.Name));
                if (!repoConfigs.Any())
                {
                    Console.WriteLine("WARNING: no repository found");
                    return;
                }

                foreach(var repoConfig in repoConfigs)
                {
                    var repoDir = wsDir.GetDirectory(repoConfig.Name);
                    if (!repoDir.Exists)
                    {
                        var sourceControl = ServiceActivator<Factory>.Create<ISourceControl>(repoConfig.Vcs.ToString());
                        sourceControl.Clone(repoDir, repoConfig.Name, repoConfig.Url);
                    }
                }
            }
        }
    }
}