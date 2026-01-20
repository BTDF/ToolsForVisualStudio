// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    internal class Util
    {
        internal static string GetMsBuildPath()
        {
            const string MSBUILDTOOLSVERSIONSKEY = @"SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0";

            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(MSBUILDTOOLSVERSIONSKEY, false))
            {
                return string.Format("{0}MSBuild.exe", (string)rk.GetValue("MSBuildToolsPath"));
            }
        }

        internal static string GetGacUtilPath()
        {
            string btdfInstallPath = GetDeploymentFrameworkInstallPath();

            if (string.IsNullOrEmpty(btdfInstallPath))
            {
                return null;
            }

            return string.Format("{0}Framework\\DeployTools\\GacUtil.exe", btdfInstallPath);
        }

        internal static string GetDeploymentFrameworkInstallPath()
        {
            string btdfInstallDir =
                (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\DeploymentFrameworkForBizTalk", "InstallPath", null);

            if (string.IsNullOrEmpty(btdfInstallDir))
            {
                MessageBox.Show(
                    "Cannot find required registry key. Please install the Deployment Framework for BizTalk MSI.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return btdfInstallDir;
        }

        internal static string GetDeploymentProjectPath(string solutionPath)
        {
            //string solutionPath = _applicationObject.Solution.FileName;
            string solutionFilenameNoExt = Path.GetFileNameWithoutExtension(solutionPath);

            // First look for <solutionNameNoExtension>.Deployment\<solutionNameNoExtension>.Deployment.btdfproj
            string projectPath = Path.Combine(Path.GetDirectoryName(solutionPath), solutionFilenameNoExt + ".Deployment");
            projectPath = Path.Combine(projectPath, solutionFilenameNoExt + ".Deployment.btdfproj");

            if (!File.Exists(projectPath))
            {
                // Next look for <solutionNameNoExtension>.Deployment\Deployment.btdfproj
                projectPath = Path.Combine(Path.GetDirectoryName(solutionPath), solutionFilenameNoExt + ".Deployment");
                projectPath = Path.Combine(projectPath, "Deployment.btdfproj");

                if (!File.Exists(projectPath))
                {
                    // Next look for Deployment\<solutionNameNoExtension>.Deployment.btdfproj
                    projectPath = Path.Combine(Path.GetDirectoryName(solutionPath), "Deployment");
                    projectPath = Path.Combine(projectPath, solutionFilenameNoExt + ".Deployment.btdfproj");

                    if (!File.Exists(projectPath))
                    {
                        // Next look for Deployment\Deployment.btdfproj
                        projectPath = Path.Combine(Path.GetDirectoryName(solutionPath), "Deployment");
                        projectPath = Path.Combine(projectPath, "Deployment.btdfproj");

                        if (!File.Exists(projectPath))
                        {
                            MessageBox.Show(
                                "Could not find a .btdfproj file for this solution. Valid locations relative to the solution root are: <solutionNameNoExtension>.Deployment\\<solutionNameNoExtension>.Deployment.btdfproj, <solutionNameNoExtension>.Deployment\\Deployment.btdfproj, Deployment\\<solutionNameNoExtension>.Deployment.btdfproj or Deployment\\Deployment.btdfproj.",
                                "Deployment Framework for BizTalk",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }

            return projectPath;
        }
    }
}
