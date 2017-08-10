// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

// Guids.cs
// MUST match guids.h
using System;

namespace DeploymentFrameworkForBizTalk.Addin
{
    static class GuidList
    {
        public const string guidAddinPkgString = "85a16b5e-be96-48ee-b32c-ef07df88501f";
        public const string guidAddinCmdSetString = "1e87aacf-06e4-496f-9e6a-f39f47bf43eb";

        public static readonly Guid guidAddinCmdSet = new Guid(guidAddinCmdSetString);
    };
}