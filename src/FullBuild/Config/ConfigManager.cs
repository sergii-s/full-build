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

using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using FullBuild.Helpers;

namespace FullBuild.Config
{
    internal static class ConfigManager
    {
        public static FullBuildConfig LoadConfig(DirectoryInfo adminDir)
        {
            var bootstrapConfig = (BoostrapConfig) ConfigurationManager.GetSection("FullBuildConfig");
            var fbDir = adminDir.GetDirectory(".full-build");
            var adminConfig = LoadAdminConfig(fbDir);
            var config = new FullBuildConfig(bootstrapConfig, adminConfig);
            return config;
        }

        public static void SaveAdminConfig(AdminConfig config, DirectoryInfo adminDir)
        {
            var file = new FileInfo(Path.Combine(adminDir.FullName, "full-build.config"));
            var xmlSer = new XmlSerializer(typeof(AdminConfig));
            using (var writer = new StreamWriter(file.FullName))
            {
                xmlSer.Serialize(writer, config);
            }
        }

        public static AdminConfig LoadAdminConfig(DirectoryInfo adminDir)
        {
            var file = new FileInfo(Path.Combine(adminDir.FullName, "full-build.config"));
            if (file.Exists)
            {
                var xmlSer = new XmlSerializer(typeof(AdminConfig));
                using(var reader = new StreamReader(file.FullName))
                {
                    var bootstrapConfig = (AdminConfig) xmlSer.Deserialize(reader);
                    bootstrapConfig.SourceRepos = bootstrapConfig.SourceRepos ?? new RepoConfig[0];

                    return bootstrapConfig;
                }
            }

            return new AdminConfig {SourceRepos = new RepoConfig[0]};
        }
    }
}