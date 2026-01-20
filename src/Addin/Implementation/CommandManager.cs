// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    internal class CommandManager
    {
        private CommandRunner _commandRunner;
        private DTE2 _applicationObject;
        private SolutionEvents _solutionEvents;
        private IServiceProvider _pkg;
        private string _msbuildPath;
        private string _gacUtilPath;

        internal CommandManager(CommandRunner runner, DTE2 application, IServiceProvider pkg)
        {
            _commandRunner = runner;
            _applicationObject = application;
            _pkg = pkg;

            _msbuildPath = Util.GetMsBuildPath();
            _gacUtilPath = Util.GetGacUtilPath();

            _solutionEvents = ((Events2)application.Events).SolutionEvents;
            _solutionEvents.AfterClosing += _solutionEvents_AfterClosing;
        }

        internal void OnDeployCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("Deploy");
        }

        internal void OnUndeployCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("Undeploy");
        }

        internal void OnQuickDeployCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("UpdateOrchestration");
        }

        internal void OnBuildMsiCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("Installer");
        }

        internal void OnBounceCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("BounceBizTalk");
        }

        internal void OnTerminateInstancesCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("TerminateServiceInstances");
        }

        internal void OnExportSettingsCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("ExportSettings");
        }

        internal void OnImportBindingsCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("ImportBindings");
        }

        internal void OnPreprocessBindingsCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("PreprocessBindings");
        }

        internal void OnUpdateSsoCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("DeploySSO");
        }

        internal void OnDeployRulesCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("DeployVocabAndRules", "ExplicitlyDeployRulePoliciesOnDeploy=true");
        }

        internal void OnUndeployRulesCommand(object sender, EventArgs e)
        {
            ExecuteMSBuildTarget("UndeployVocabAndRules", "RemoveRulePoliciesFromAppOnUndeploy=true");
        }

        internal void OnGacProjectOutputCommand(object sender, EventArgs e)
        {
            Array projects = (Array)_applicationObject.ActiveSolutionProjects;

            if (projects.Length == 0)
            {
                return;
            }

            if (projects.Length > 1)
            {
                ShowMessageBox("Please select only one project.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_WARNING);
                return;
            }

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Project proj = projects.GetValue(0) as Project;

                if (proj.ConfigurationManager == null)
                {
                    return;
                }

                OutputGroup primaryOutputGroup = proj.ConfigurationManager.ActiveConfiguration.OutputGroups.Item("Built");
                object[] primaryOutputs = primaryOutputGroup.FileURLs as object[];

                Uri path = new Uri(primaryOutputs[0].ToString());

                string arguments = string.Format("/i \"{0}\" /f", System.IO.Path.GetFullPath(path.LocalPath));
                _commandRunner.ExecuteBuild(_gacUtilPath, arguments);
            });
        }

        internal void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;

            if (command != null)
            {
                //System.Diagnostics.Trace.WriteLine("OnBeforeQueryStatus: " + command.ToString());
                command.Enabled = (_commandRunner.IsBusy != 1 && _applicationObject.Solution.IsOpen && !VsShellUtilities.IsSolutionBuilding(_pkg));
            }
        }

        private void ExecuteMSBuildTarget(string targetName)
        {
            ExecuteMSBuildTarget(targetName, null);
        }

        private void ExecuteMSBuildTarget(string targetName, string addlProperties)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string activeSolutionConfiguration = _applicationObject.Solution.SolutionBuild.ActiveConfiguration.Name;
                string solutionFilename = _applicationObject.Solution.FileName;
                string projectPath = Util.GetDeploymentProjectPath(solutionFilename);
                string arguments = string.Format("\"{0}\" /nologo /t:{1} /p:Configuration={2}", projectPath, targetName, activeSolutionConfiguration);

                if (!string.IsNullOrWhiteSpace(addlProperties))
                {
                    arguments += " /p:" + addlProperties;
                }

                _commandRunner.ExecuteBuild(_msbuildPath, arguments);
            });
        }

        private void ShowMessageBox(string msg, OLEMSGBUTTON buttonType, OLEMSGICON iconType)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Guid clsid = Guid.Empty;
                int result;

                IVsUIShell uiShell = (IVsUIShell)_pkg.GetService(typeof(SVsUIShell));

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                    uiShell.ShowMessageBox(
                        0,
                        ref clsid,
                        "Deployment Framework for BizTalk",
                        msg,
                        string.Empty,
                        0,
                        buttonType,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                        iconType,
                        0,        // false
                        out result));
            });
        }

        private void _solutionEvents_AfterClosing()
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _commandRunner.OnCloseSolution();
            });
        }
    }
}
