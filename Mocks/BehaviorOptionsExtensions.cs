// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks
{
    using System;

    /// <summary>
    ///     Contains methods that extend the <see cref="BehaviorOptions"/> type.
    /// </summary>
    public static class BehaviorOptionsExtensions
    {
        /// <summary>
        ///     Allows setting multiple options for a mock's behavior.
        /// </summary>
        /// <typeparam name="TOptions">
        ///     The type of options object to operate on. This will be
        ///     automatically inferred.
        /// </typeparam>
        /// <param name="options">
        ///     The options object to operate on. This does not need to be
        ///     specified when using the method as an extension.
        /// </param>
        /// <param name="optionSetter">
        ///     The delegate that will set options for the behavior.
        /// </param>
        /// <returns>
        ///     The same instance that was passed in, to allow call chaining.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     This is a convenience method that allows setting additional
        ///     options for a behavior in a fluent interface-like fashion. That
        ///     is, a call to this method can be chained after a call to a mock's
        ///     <see cref="Mock.SetBehavior"/> or <see cref="Mock.AddBehavior"/>
        ///     family of methods.
        ///     </para>
        /// </remarks>
        public static TOptions With<TOptions>(
            this TOptions options, Action<TOptions> optionSetter)
            where TOptions : BehaviorOptions
        {
            optionSetter(options);
            return options;
        }
    }
}
