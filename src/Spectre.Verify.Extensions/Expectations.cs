using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using VerifyTests;

namespace Spectre.Verify.Extensions
{
    /// <summary>
    /// Contains initialization logic for deriving path information.
    /// </summary>
    public static class Expectations
    {
        /// <summary>
        /// Initializes the custom <see cref="DerivePathInfo"/> strategy.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="projectDirectory">The directory of the project that the test was compile from.</param>
        /// <param name="type">The class the test method exists in.</param>
        /// <param name="method">The test method.</param>
        /// <returns>The path info.</returns>
        [SuppressMessage("Usage", "CA1801:Review unused parameters")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        [SuppressMessage("Redundancy", "RCS1163:Unused parameter.")]
        public static PathInfo Initialize(string sourceFile, string projectDirectory, Type type, MethodInfo method)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var root = Path.Combine(projectDirectory, "Expectations");

            var attribute = method.GetCustomAttribute<ExpectationAttribute>();
            if (attribute == null)
            {
                return GetFallbackPathInfo(root, type, method);
            }

            return GetPathInfo(root, type, method);
        }

        private static PathInfo GetFallbackPathInfo(string root, Type type, MethodInfo method)
        {
            return new PathInfo(
                directory: root,
                typeName: GetTypeName(type),
                methodName: method.Name);
        }

        private static PathInfo GetPathInfo(string root, Type type, MethodInfo method)
        {
            // Get the method name
            var nameAttribute = method.GetCustomAttribute<ExpectationAttribute>();
            if (nameAttribute == null)
            {
                throw new InvalidOperationException("Could not resolve expectation attribute.");
            }

            var methodName = string.IsNullOrWhiteSpace(nameAttribute.Name) ? method.Name : nameAttribute.Name;

            var pathStack = new List<string>();
            var current = type;
            while (current != null)
            {
                var pathAttribute = current.GetCustomAttribute<ExpectationPathAttribute>();
                if (pathAttribute != null)
                {
                    if (pathAttribute.Folder == null)
                    {
                        pathStack.Insert(0, current.Name);
                    }
                    else
                    {
                        pathStack.Insert(0, pathAttribute.Folder);
                    }
                }

                current = current.DeclaringType;
            }

            // Append the path stack to the root
            root = Path.Combine(root, string.Join("/", pathStack));

            // Get the suffix
            var suffix = nameAttribute.Suffix ?? "Output";

            return new PathInfo(
                directory: root,
                typeName: methodName,
                methodName: suffix);
        }

        private static string GetTypeName(Type type)
        {
            var typeName = type.FullName;
            if (typeName == null)
            {
                throw new InvalidOperationException("Could not resolve type name.");
            }

            var index = typeName.LastIndexOf('.');
            if (index != -1)
            {
                typeName = typeName.Substring(index + 1);
            }

            return typeName.Replace('+', '.');
        }
    }
}
