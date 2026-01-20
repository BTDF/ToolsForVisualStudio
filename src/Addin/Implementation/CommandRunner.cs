// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    internal class CommandRunner
    {
        internal const string ToolWindowName = "Deployment Framework for BizTalk";

        internal int IsBusy = 0;

        private DTE2 _applicationObject;
        private IVsOutputWindow _vsOutputWindow;
        private OutputWindowPane _owP;

        private delegate void RunHandler(string exePath, string arguments);

        internal CommandRunner(DTE2 applicationObject, IVsOutputWindow outputWindow)
        {
            this._applicationObject = applicationObject;
            this._vsOutputWindow = outputWindow;
        }

        internal void ExecuteBuild(string exePath, string arguments)
        {
            if (Interlocked.CompareExchange(ref IsBusy, 1, 0) == 1)
            {
                return;
            }

            RunHandler rh = new RunHandler(RunProcess);
            AsyncCallback callback = new AsyncCallback(RunCallback);
            rh.BeginInvoke(exePath, arguments, callback, rh);
        }

        internal void OnCloseSolution()
        {
            OutputWindow ow = _applicationObject.ToolWindows.OutputWindow;

            foreach (OutputWindowPane owp in ow.OutputWindowPanes)
            {
                if (string.Compare(owp.Name, ToolWindowName, true) == 0)
                {
                    _vsOutputWindow.DeletePane(Guid.Parse(owp.Guid));
                    return;
                }
            }
        }

        private void RunCallback(IAsyncResult result)
        {
            RunHandler rh = result.AsyncState as RunHandler;

            try
            {
                rh.EndInvoke(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Deployment Framework for BizTalk: Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Interlocked.CompareExchange(ref IsBusy, 0, 1);
            }
        }

        private void OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _owP.OutputString(e.Data + "\r\n");
            });
        }

        private void RunProcess(string exePath, string arguments)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _owP = GetOutputWindowPane();
                _owP.Clear();
                _owP.Activate();
                _applicationObject.ToolWindows.OutputWindow.Parent.Activate();

                _owP.OutputString("Starting build...\r\n");
                _owP.OutputString(exePath + " " + arguments + "\r\n");
                _owP.OutputString(string.Empty);
            });

            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = exePath;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;

                proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputDataReceived);
                proc.Start();

                proc.BeginOutputReadLine();
                proc.WaitForExit();
            }

            _owP = null;
        }

        private OutputWindowPane GetOutputWindowPane()
        {
            OutputWindow ow = _applicationObject.ToolWindows.OutputWindow;

            foreach (OutputWindowPane owp in ow.OutputWindowPanes)
            {
                if (string.Compare(owp.Name, ToolWindowName, true) == 0)
                {
                    return owp;
                }
            }

            return ow.OutputWindowPanes.Add(ToolWindowName);
        }
    }
}
