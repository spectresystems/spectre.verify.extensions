using System;

namespace VerifyTests;

/// <summary>
/// Attribute used to decorate test methods to indicate what
/// an expectation file is called.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class ExpectationAttribute : Attribute
{
    /// <summary>
    /// Gets the expectation name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets the expectation suffix.
    /// </summary>
    public string? Suffix { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectationAttribute"/> class.
    /// </summary>
    public ExpectationAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectationAttribute"/> class.
    /// </summary>
    /// <param name="name">The expectation name.</param>
    public ExpectationAttribute(string name)
        : this(name, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectationAttribute"/> class.
    /// </summary>
    /// <param name="name">The expectation name.</param>
    /// <param name="suffix">The expectation suffix.</param>
    public ExpectationAttribute(string name, string? suffix)
    {
        Name = name;
        Suffix = suffix;
    }
}