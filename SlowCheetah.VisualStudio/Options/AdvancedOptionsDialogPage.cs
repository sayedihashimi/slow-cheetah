﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.Win32;

    /// <summary>
    /// Advanced Options Page for SlowCheetah
    /// </summary>
    internal class AdvancedOptionsDialogPage : DialogPage
    {
        private const string RegOptionsKey = "ConfigTransform";
        private const string RegPreviewCmdLine = "PreviewCmdLine";
        private const string RegPreviewExe = "PreviewExe";

        /// <summary>
        /// Gets or sets the exe path for the diff tool used to preview transformations
        /// </summary>
        public string PreviewToolExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the required command on execution of the preview tool
        /// </summary>
        public string PreviewToolCommandLine { get; set; }

        /// <inheritdoc/>
        protected override IWin32Window Window
        {
            get
            {
                var optionControl = new AdvancedOptionUserControl();
                optionControl.Initialize(this);
                return optionControl;
            }
        }

        /// <inheritdoc/>
        public override void SaveSettingsToXml(IVsSettingsWriter writer)
        {
            try
            {
                base.SaveSettingsToXml(writer);

                // Write settings to XML
                writer.WriteSettingString(RegPreviewExe, this.PreviewToolExecutablePath);
                writer.WriteSettingString(RegPreviewCmdLine, this.PreviewToolCommandLine);
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Error exporting Slow Cheetah settings: " + e.Message);
            }
        }

        /// <inheritdoc/>
        public override void LoadSettingsFromXml(IVsSettingsReader reader)
        {
            try
            {
                this.InitializeDefaults();
                if (ErrorHandler.Succeeded(reader.ReadSettingString(RegPreviewExe, out string exePath)) && !string.IsNullOrEmpty(exePath))
                {
                    this.PreviewToolExecutablePath = exePath;
                }

                if (ErrorHandler.Succeeded(reader.ReadSettingString(RegPreviewCmdLine, out string exeCmdLine)) && !string.IsNullOrEmpty(exeCmdLine))
                {
                    this.PreviewToolCommandLine = exeCmdLine;
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Error importing Slow Cheetah settings: " + e.Message);
            }
        }

        /// <summary>
        /// Load Tools-->Options settings from registry. Defaults are set in constructor so these
        /// will just override those values.
        /// </summary>
        public override void LoadSettingsFromStorage()
        {
            try
            {
                this.InitializeDefaults();
                using (RegistryKey userRootKey = SlowCheetahPackage.OurPackage.UserRegistryRoot)
                {
                    using (RegistryKey cheetahKey = userRootKey.OpenSubKey(RegOptionsKey))
                    {
                        if (cheetahKey != null)
                        {
                            object previewTool = cheetahKey.GetValue(RegPreviewExe);
                            if (previewTool != null && (previewTool is string) && !string.IsNullOrEmpty((string)previewTool))
                            {
                                this.PreviewToolExecutablePath = (string)previewTool;
                            }

                            object previewCmdLine = cheetahKey.GetValue(RegPreviewCmdLine);
                            if (previewCmdLine != null && (previewCmdLine is string) && !string.IsNullOrEmpty((string)previewCmdLine))
                            {
                                this.PreviewToolCommandLine = (string)previewCmdLine;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Error reading Slow Cheetah settings from the registry: " + e.Message);
            }
        }

        /// <summary>
        /// Save Tools-->Options settings to registry
        /// </summary>
        public override void SaveSettingsToStorage()
        {
            try
            {
                base.SaveSettingsToStorage();
                using (RegistryKey userRootKey = SlowCheetahPackage.OurPackage.UserRegistryRoot)
                {
                    using (RegistryKey cheetahKey = userRootKey.CreateSubKey(RegOptionsKey))
                    {
                        cheetahKey.SetValue(RegPreviewExe, this.PreviewToolExecutablePath);
                        cheetahKey.SetValue(RegPreviewCmdLine, this.PreviewToolCommandLine);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Error saving Slow Cheetah settings to the registry:" + e.Message);
            }
        }

        /// <summary>
        /// Sets up our default values.
        /// </summary>
        private void InitializeDefaults()
        {
            string diffToolPath = SlowCheetahPackage.OurPackage.GetVsInstallDirectory();
            if (diffToolPath != null)
            {
                diffToolPath = Path.Combine(diffToolPath, "diffmerge.exe");
                this.PreviewToolExecutablePath = diffToolPath;
            }

            this.PreviewToolCommandLine = "{0} {1}";
        }
    }
}
