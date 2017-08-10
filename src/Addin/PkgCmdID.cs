// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace DeploymentFrameworkForBizTalk.Addin
{
    static class PkgCmdIDList
    {
        public const uint cmdidDeploy = 0x100;
        public const uint cmdidUndeploy = 0x0102;
        public const uint cmdidQuickDeploy = 0x0104;
        public const uint cmdidBuildMsi = 0x0106;
        public const uint cmdidDeployRules = 0x0108;
        public const uint cmdidUndeployRules = 0x0110;
        public const uint cmdidBounce = 0x0112;
        public const uint cmdidTerminateInstances = 0x0114;
        public const uint cmdidExportSettings = 0x0116;
        public const uint cmdidGacProjectOutput = 0x0118;
        public const uint cmdidImportBindings = 0x0120;
        public const uint cmdidPreprocessBindings = 0x0122;
        public const uint cmdidUpdateSso = 0x0124;
    };
}