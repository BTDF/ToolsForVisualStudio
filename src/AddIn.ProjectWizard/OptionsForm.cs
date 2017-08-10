// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DeploymentFramework.VisualStudioAddIn.ProjectWizard
{
    public partial class OptionsForm : Form
    {
        internal DeploymentOptions Options { get; set; }
        internal bool WritePropertiesOnlyWhenNonDefault
        {
            get { return writeOnlyWhenNonDefault.Checked; }
        }

        public OptionsForm(DeploymentOptions options)
        {
            this.Options = options;
            InitializeComponent();
            this.propertyGrid1.SelectedObject = this.Options;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
