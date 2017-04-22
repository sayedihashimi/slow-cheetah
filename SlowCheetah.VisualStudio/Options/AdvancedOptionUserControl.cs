// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System.Windows.Forms;

    /// <summary>
    /// The UI for the advanced section of the options page
    /// </summary>
    public partial class AdvancedOptionUserControl : UserControl
    {
        private AdvancedOptionsDialogPage advancedOptionsPage = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedOptionUserControl"/> class.
        /// </summary>
        public AdvancedOptionUserControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialized the Advanced Options Page Control
        /// </summary>
        /// <param name="advancedOptionsPage">The options page that corresponds to this control</param>
        internal void Initialize(AdvancedOptionsDialogPage advancedOptionsPage)
        {
            this.advancedOptionsPage = advancedOptionsPage;
            this.PreviewToolPathTextbox.Text = advancedOptionsPage.PreviewToolExecutablePath;
            this.PreviewToolCommandLineTextbox.Text = advancedOptionsPage.PreviewToolCommandLine;
        }

        private void PreviewToolPathTextbox_Leave(object sender, System.EventArgs e)
        {
            if (this.advancedOptionsPage != null)
            {
                this.advancedOptionsPage.PreviewToolExecutablePath = this.PreviewToolPathTextbox.Text;
            }
        }

        private void PreviewToolCommandLineTextbox_Leave(object sender, System.EventArgs e)
        {
            if (this.advancedOptionsPage != null)
            {
                this.advancedOptionsPage.PreviewToolCommandLine = this.PreviewToolCommandLineTextbox.Text;
            }
        }
    }
}
