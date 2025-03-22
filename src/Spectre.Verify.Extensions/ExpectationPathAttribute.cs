using System;

namespace VerifyTests;

/// <summary>
/// Attribute used to decorate test classes to specify a part of a path.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ExpectationPathAttribute : Attribute
{
    /// <summary>
    /// Gets the folder name.
    /// </summary>
    public string? Folder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectationPathAttribute"/> class.
    /// </summary>
    public ExpectationPathAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectationPathAttribute"/> class.
    /// </summary>
    /// <param name="folder">The folder name.</param>
    public ExpectationPathAttribute(string folder)
    {
        Folder = folder;
    }
}