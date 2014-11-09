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
using System.IO;
using LibGit2Sharp;

namespace FullBuild.SourceControl
{
    internal class Git : ISourceControl
    {
        private class Progress
        {
            private static readonly char[] _indicator = {'-', '\\', '|', '/'};

            private int _index = 0;

            public bool OnTransferProgress(TransferProgress progress)
            {
                Console.Write("\r{0}", _indicator[_index]);
                _index = (_index + 1) % _indicator.Length;
                return true;
            }
        }

        public void Clone(DirectoryInfo rootDir, string name, string url)
        {
            CloneOptions options = new CloneOptions();
            var progress = new Progress();
            options.OnTransferProgress += progress.OnTransferProgress;
            Repository.Clone(url, rootDir.FullName, options);
            Console.Write("\r");
        }

        public string Tip(DirectoryInfo rootDir)
        {
            using(var repo = new Repository(rootDir.FullName))
            {
                return repo.Head.Tip.Sha;
            }
        }
    }
}