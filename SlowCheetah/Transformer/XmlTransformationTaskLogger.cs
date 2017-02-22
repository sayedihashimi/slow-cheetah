// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.Web.XmlTransform;

    /// <summary>
    /// Shim for using MSBuild logger in <see cref="XmlTransformation"/>
    /// </summary>
    public class XmlTransformationTaskLogger : IXmlTransformationLogger
    {
        private readonly string indentStringPiece = "  ";

        private readonly TaskLoggingHelper loggingHelper;

        private int indentLevel = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformationTaskLogger"/> class.
        /// </summary>
        /// <param name="logger">The MSBuild logger</param>
        public XmlTransformationTaskLogger(TaskLoggingHelper logger)
        {
            this.loggingHelper = logger;
        }

        private string IndentString
        {
            get
            {
                if (this.indentLevel == 0)
                {
                    return string.Empty;
                }

                return string.Concat(Enumerable.Repeat(this.indentStringPiece, this.indentLevel));
            }
        }

        /// <inheritdoc/>
        public void LogError(string message, params object[] messageArgs)
        {
            this.loggingHelper.LogError(message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogError(string file, string message, params object[] messageArgs)
        {
            this.LogError(file, 0, 0, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.loggingHelper.LogError(null, null, null, file, lineNumber, linePosition, 0, 0, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex)
        {
            this.loggingHelper.LogErrorFromException(ex);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex, string file)
        {
            this.loggingHelper.LogErrorFromException(ex, false, false, file);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            this.LogError(file, lineNumber, linePosition, ex.Message);
        }

        /// <inheritdoc/>
        public void LogMessage(string message, params object[] messageArgs)
        {
            this.LogMessage(MessageType.Normal, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            MessageImportance importance;
            switch (type)
            {
                case MessageType.Normal:
                    importance = MessageImportance.Normal;
                    break;
                case MessageType.Verbose:
                    importance = MessageImportance.Low;
                    break;
                default:
                    Debug.Fail("Unknown MessageType");
                    importance = MessageImportance.Normal;
                    break;
            }

            this.loggingHelper.LogMessage(importance, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogWarning(string message, params object[] messageArgs)
        {
            this.loggingHelper.LogWarning(message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            this.LogWarning(file, 0, 0, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.loggingHelper.LogWarning(null, null, null, file, lineNumber, linePosition, 0, 0, message, messageArgs);
        }

        /// <inheritdoc/>
        public void StartSection(string message, params object[] messageArgs)
        {
            this.StartSection(MessageType.Normal, message, messageArgs);
        }

        /// <inheritdoc/>
        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            this.LogMessage(type, message, messageArgs);
            this.indentLevel++;
        }

        /// <inheritdoc/>
        public void EndSection(string message, params object[] messageArgs)
        {
            this.EndSection(MessageType.Normal, message, messageArgs);
        }

        /// <inheritdoc/>
        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            Debug.Assert(this.indentLevel > 0, "There must be at least one section started");
            if (this.indentLevel > 0)
            {
                this.indentLevel--;
            }

            this.LogMessage(type, message, messageArgs);
        }
    }
}
