// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DeploymentFramework.VisualStudioAddIn.ProjectWizard;

namespace VisualStudioAddIn.ProjectWizard.Test
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeploymentOptions options = new DeploymentOptions();
            options.IncludeBam = true;

            OptionsForm of = new OptionsForm(options);
            of.ShowDialog();

        }
    }
}
