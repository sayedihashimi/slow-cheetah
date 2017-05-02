﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.Win32;

    /// <summary>
    /// Options page for SlowCheetah
    /// </summary>
    [System.Runtime.InteropServices.Guid("01B6BAC2-0BD6-4ead-95AE-6D6DE30A6286")]
    internal class OptionsDialogPage : DialogPage
    {
        private const string RegOptionsKey = "ConfigTransform";
        private const string RegPreviewEnable = "EnablePreview";
        private const string RegDependentUpon = "EnableDependentUpon";

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialogPage"/> class.
        /// Constructor called by VSIP just in time when user wants to view this tools, options page
        /// </summary>
        public OptionsDialogPage()
        {
            this.InitializeDefaults();
        }

        /// <summary>
        /// Gets or sets a value indicating whether preview is enabled or not
        /// </summary>
        public bool EnablePreview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to add DependentUpon metadata
        /// </summary>
        public bool AddDependentUpon { get; set; }

        /// <inheritdoc/>
        protected override IWin32Window Window
        {
            get
            {
                var optionControl = new OptionUserControl();
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
                writer.WriteSettingBoolean(RegPreviewEnable, this.EnablePreview ? 1 : 0);
                writer.WriteSettingBoolean(RegDependentUpon, this.AddDependentUpon ? 1 : 0);
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

                if (ErrorHandler.Succeeded(reader.ReadSettingBoolean(RegPreviewEnable, out int enablePreview)))
                {
                    this.EnablePreview = enablePreview == 1;
                }

                if (ErrorHandler.Succeeded(reader.ReadSettingBoolean(RegDependentUpon, out int addDependentUpon)))
                {
                    this.AddDependentUpon = addDependentUpon == 1;
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
                            object enablePreview = cheetahKey.GetValue(RegPreviewEnable);
                            if (enablePreview != null && (enablePreview is int))
                            {
                                this.EnablePreview = ((int)enablePreview) == 1;
                            }

                            object addDependentUpon = cheetahKey.GetValue(RegDependentUpon);
                            if (addDependentUpon != null && (addDependentUpon is int))
                            {
                                this.AddDependentUpon = ((int)addDependentUpon) == 1;
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
                        cheetahKey.SetValue(RegPreviewEnable, this.EnablePreview ? 1 : 0);
                        cheetahKey.SetValue(RegDependentUpon, this.AddDependentUpon ? 1 : 0);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Error saving Slow Cheetah settings to the registry:" + e.Message);
            }
        }

        /// <summary>
        /// This event is raised when VS wants to deactivate this page.
        /// The page is deactivated unless the event is cancelled.
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        protected override void OnDeactivate(CancelEventArgs e)
        {
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
            }

            this.EnablePreview = true;
            this.AddDependentUpon = true;
        }

        /// <summary>
        /// Attribute class allows us to loc the displayname for our properties. Property resource
        /// is expected to be named PropName_[propertyname]
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        internal sealed class LocDisplayNameAttribute : DisplayNameAttribute
        {
            private string displayName;

            /// <summary>
            /// Initializes a new instance of the <see cref="LocDisplayNameAttribute"/> class.
            /// </summary>
            /// <param name="name">Attribute name</param>
            public LocDisplayNameAttribute(string name)
            {
                this.displayName = name;
            }

            /// <summary>
            /// Gets the display name of the attribute
            /// </summary>
            public override string DisplayName
            {
                get
                {
                    string result = Resources.Resources.ResourceManager.GetString("PropName_" + this.displayName);
                    if (result == null)
                    {
                        // Just return non-loc'd value
                        Debug.Assert(false, "String resource '" + this.displayName + "' is missing");
                        result = this.displayName;
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Attribute class allows us to loc the description for our properties. Property resource
        /// is expected to be named PropDesc_[propertyname]
        /// </summary>
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        internal sealed class LocDescriptionAttribute : DescriptionAttribute
        {
            private string descName;

            /// <summary>
            /// Initializes a new instance of the <see cref="LocDescriptionAttribute"/> class.
            /// </summary>
            /// <param name="name">Attribute name</param>
            public LocDescriptionAttribute(string name)
            {
                this.descName = name;
            }

            /// <summary>
            /// Gets the description for the attribute
            /// </summary>
            public override string Description
            {
                get
                {
                    string result = Resources.Resources.ResourceManager.GetString("PropDesc_" + this.descName);

                    if (result == null)
                    {
                        // Just return non-loc'd value
                        Debug.Assert(false, "String resource '" + this.descName + "' is missing");
                        result = this.descName;
                    }

                    return result;
                }
            }
        }
    }
}
