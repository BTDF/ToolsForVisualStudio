// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    internal class MenuCommandRegistrar
    {
        private OleMenuCommandService _mcs;
        private CommandManager _manager;

        internal MenuCommandRegistrar(OleMenuCommandService mcs, CommandManager manager)
        {
            _mcs = mcs;
            _manager = manager;
        }

        internal void RegisterMenuCommands()
        {
            // Create the command for the menu item.
            CommandID menuCommandID;
            OleMenuCommand menuItem;

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidDeploy);
            menuItem = new OleMenuCommand(_manager.OnDeployCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidUndeploy);
            menuItem = new OleMenuCommand(_manager.OnUndeployCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidQuickDeploy);
            menuItem = new OleMenuCommand(_manager.OnQuickDeployCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidBuildMsi);
            menuItem = new OleMenuCommand(_manager.OnBuildMsiCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidDeployRules);
            menuItem = new OleMenuCommand(_manager.OnDeployRulesCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidUndeployRules);
            menuItem = new OleMenuCommand(_manager.OnUndeployRulesCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidBounce);
            menuItem = new OleMenuCommand(_manager.OnBounceCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidTerminateInstances);
            menuItem = new OleMenuCommand(_manager.OnTerminateInstancesCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidExportSettings);
            menuItem = new OleMenuCommand(_manager.OnExportSettingsCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidGacProjectOutput);
            menuItem = new OleMenuCommand(_manager.OnGacProjectOutputCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidImportBindings);
            menuItem = new OleMenuCommand(_manager.OnImportBindingsCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidPreprocessBindings);
            menuItem = new OleMenuCommand(_manager.OnPreprocessBindingsCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);

            menuCommandID = new CommandID(GuidList.guidAddinCmdSet, (int)PkgCmdIDList.cmdidUpdateSso);
            menuItem = new OleMenuCommand(_manager.OnUpdateSsoCommand, menuCommandID);
            _mcs.AddCommand(menuItem);
            menuItem.BeforeQueryStatus += new EventHandler(_manager.OnBeforeQueryStatus);
        }
    }
}
