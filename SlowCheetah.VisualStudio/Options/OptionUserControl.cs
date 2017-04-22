// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The UI for the options page
    /// </summary>
    public partial class OptionUserControl : UserControl
    {
        private OptionsDialogPage optionsPage = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUserControl"/> class.
        /// </summary>
        public OptionUserControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialized the Options Page Control
        /// </summary>
        /// <param name="optionsPage">The options page that corresponds to this control</param>
        internal void Initialize(OptionsDialogPage optionsPage)
        {
            this.optionsPage = optionsPage ?? throw new ArgumentNullException(nameof(optionsPage));
            this.EnablePreviewCheckbox.Checked = optionsPage.EnablePreview;
            this.AddDepentUponCheckbox.Checked = optionsPage.AddDependentUpon;
        }

        private void EnablePreviewCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.optionsPage != null)
            {
                this.optionsPage.EnablePreview = this.EnablePreviewCheckbox.Checked;
            }
        }

        private void AddDepentUponCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.optionsPage != null)
            {
                this.optionsPage.AddDependentUpon = this.AddDepentUponCheckbox.Checked;
            }
        }
    }
}
