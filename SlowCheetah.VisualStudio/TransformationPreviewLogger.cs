// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using SlowCheetah;

    /// <summary>
    /// Logger for XDT transformation on Preview Transform
    /// </summary>
    public class TransformationPreviewLogger : ITransformationLogger
    {
        private IVsOutputWindowPane outputWindow;
        private ErrorListProvider errorListProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationPreviewLogger"/> class.
        /// </summary>
        /// <param name="package">The VS Package</param>
        public TransformationPreviewLogger(IServiceProvider package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.outputWindow = (IVsOutputWindowPane)package.GetService(typeof(SVsGeneralOutputWindowPane));
            this.errorListProvider = new ErrorListProvider(package);
        }

        /// <inheritdoc/>
        public void LogError(string message, params object[] messageArgs)
        {
            var newError = new ErrorTask()
            {
                ErrorCategory = TaskErrorCategory.Error,
                Category = TaskCategory.Misc,
                Text = string.Format(message, messageArgs)
            };

            this.errorListProvider.Tasks.Add(newError);
            this.errorListProvider.Show();
        }

        /// <inheritdoc/>
        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            var newError = new ErrorTask()
            {
                ErrorCategory = TaskErrorCategory.Error,
                Category = TaskCategory.Misc,
                Text = string.Format(message, messageArgs),
                Document = file,
                Line = lineNumber,
                Column = linePosition
            };

            this.errorListProvider.Tasks.Add(newError);
            this.errorListProvider.Show();
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex)
        {
            this.errorListProvider.Tasks.Add(new ErrorTask(ex));
            this.errorListProvider.Show();
        }

        /// <inheritdoc/>
        public void LogMessage(string message, params object[] messageArgs)
        {
            var newError = new ErrorTask()
            {
                ErrorCategory = TaskErrorCategory.Message,
                Category = TaskCategory.Misc,
                Text = string.Format(message, messageArgs)
            };

            this.errorListProvider.Tasks.Add(newError);
            this.errorListProvider.Show();
        }

        /// <inheritdoc/>
        public void LogWarning(string message, params object[] messageArgs)
        {
            var newError = new ErrorTask()
            {
                ErrorCategory = TaskErrorCategory.Warning,
                Category = TaskCategory.Misc,
                Text = string.Format(message, messageArgs)
            };

            this.errorListProvider.Tasks.Add(newError);
            this.errorListProvider.Show();
        }

        /// <inheritdoc/>
        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            var newError = new ErrorTask()
            {
                ErrorCategory = TaskErrorCategory.Warning,
                Category = TaskCategory.Misc,
                Text = string.Format(message, messageArgs),
                Document = file,
                Line = lineNumber,
                Column = linePosition
            };

            this.errorListProvider.Tasks.Add(newError);
            this.errorListProvider.Show();
        }
    }
}
