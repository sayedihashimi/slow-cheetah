// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah
{
    using System.Diagnostics.Contracts;
    using System.IO;
    using Microsoft.Web.XmlTransform;
    using SlowCheetah.Exceptions;

    /// <summary>
    /// Transforms XML files utilizing Microsoft Web XmlTransform library
    /// </summary>
    public class XmlTransformer : ITransformer
    {
        private IXmlTransformationLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformer"/> class.
        /// </summary>
        public XmlTransformer()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformer"/> class with an external logger.
        /// </summary>
        /// <param name="logger">External logger. Passed into the transformation</param>
        public XmlTransformer(IXmlTransformationLogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public void Transform(string source, string transform, string destination)
        {
            // Parameter validation
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(!string.IsNullOrWhiteSpace(transform));
            Contract.Requires(!string.IsNullOrWhiteSpace(destination));

            // File validation
            if (!File.Exists(source))
            {
                throw new FileNotFoundException("File to transform not found", source);
            }

            if (!File.Exists(transform))
            {
                throw new FileNotFoundException("Transform file not found", transform);
            }

            using (XmlTransformableDocument document = new XmlTransformableDocument())
            using (XmlTransformation transformation = new XmlTransformation(transform, this.logger))
            {
                document.PreserveWhitespace = true;
                document.Load(source);

                var success = transformation.Apply(document);
                if (!success)
                {
                    string message = $"There was an unknown error trying while trying to apply the transform. Source file='{source}',Transform='{transform}', Destination='{destination}'";
                    throw new TransformFailedException(message);
                }

                document.Save(destination);
            }
        }
    }
}
