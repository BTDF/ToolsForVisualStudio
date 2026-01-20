// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using DeploymentFrameworkForBizTalk.Addin.Implementation;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeploymentFrameworkForBizTalk.Addin
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// The minimum requirement for a class to be considered a package for Visual Studio is to implement IVsPackage and register with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF) to do it: it derives from the Package class that
    /// provides the implementation of the IVsPackage interface and uses the registration attributes defined in the framework to register itself
    /// and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is used to register the informations needed to show the this package in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "5.x", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(GuidList.guidAddinPkgString)]
    public sealed class AddinPackage : AsyncPackage
    {
        private CommandManager _cmdManager;

        /// <summary>
        /// Inside this method you can place any initialization code that does not require any Visual Studio service, because at this point
        /// the package object is created but not sited yet inside Visual Studio. The place to do all the other initialization is the Initialize method.
        /// </summary>
        public AddinPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is where to put initialization code that relies on services provided by Visual Studio
        /// </summary>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));

            // Switches to the UI thread in order to consume some services used in command initialization
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            DTE2 applicationObject = (DTE2) await GetServiceAsync(typeof(DTE));
            IVsOutputWindow outputWindow = (IVsOutputWindow) await GetServiceAsync(typeof(SVsOutputWindow));

            _cmdManager = new CommandManager(new CommandRunner(applicationObject, outputWindow), applicationObject, this);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                MenuCommandRegistrar registrar = new MenuCommandRegistrar(mcs, _cmdManager);
                registrar.RegisterMenuCommands();
            }
        }
    }
}
