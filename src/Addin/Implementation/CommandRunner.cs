// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    internal class CommandRunner
    {
        internal const string ToolWindowName = "Deployment Framework for BizTalk";

        internal int IsBusy = 0;

        private DTE2 _applicationObject;
        private IVsOutputWindow _vsOutputWindow;

        private delegate void RunHandler(string exePath, string arguments);

        internal CommandRunner(DTE2 applicationObject, IVsOutputWindow outputWindow)
        {
            this._applicationObject = applicationObject;
            this._vsOutputWindow = outputWindow;
        }

        internal void ExecuteBuild(string exePath, string arguments)
        {
            if (SetBusy() == 1)
            {
                return;
            }

            OutputWindow ow = _applicationObject.ToolWindows.OutputWindow;
            OutputWindowPane owP = GetOutputWindowPane();

            owP.Clear();
            owP.Activate();
            ow.Parent.Activate();

            RunHandler rh = new RunHandler(Run);
            AsyncCallback callback = new AsyncCallback(RunCallback);
            rh.BeginInvoke(exePath, arguments, callback, rh);
        }

        internal void OnOpenSolution()
        {
            GetOutputWindowPane();
        }

        internal void OnCloseSolution()
        {
            RemoveOutputWindowPane();
        }

        private int SetBusy()
        {
            return Interlocked.CompareExchange(ref IsBusy, 1, 0);
        }

        private void SetFree()
        {
            Interlocked.CompareExchange(ref IsBusy, 0, 1);
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
                SetFree();
            }
        }

        private void proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            WriteToOutputWindow(e.Data);
        }

        private void WriteToOutputWindow(string message)
        {
            OutputWindowPane owP = GetOutputWindowPane();
            owP.OutputString(message);
            owP.OutputString("\r\n");
        }

        private void Run(string exePath, string arguments)
        {
            RunProcess(exePath, arguments);
        }

        private void RunProcess(string exePath, string arguments)
        {
            WriteToOutputWindow("Starting build...");
            WriteToOutputWindow(exePath + " " + arguments);
            WriteToOutputWindow(string.Empty);

            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo.FileName = exePath;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;

                proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(proc_OutputDataReceived);
                proc.Start();

                proc.BeginOutputReadLine();
                proc.WaitForExit();
            }
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

        private void RemoveOutputWindowPane()
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
    }
}
