// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     Represents a custom behavior that replaces existing behavior, and
    ///     contains the options for running it.
    /// </summary>
    public class ReplacementBehavior : BehaviorOptions<Func<object[], object>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReplacementBehavior"/>
        ///     class.
        /// </summary>
        /// <param name="behavior">
        ///     The behavior to run.
        /// </param>
        /// <param name="fullMemberName">
        ///     The name of the member being overridden. This should be the
        ///     full name so it can be used to uniquely identify the member.
        /// </param>
        public ReplacementBehavior(
            Func<object[], object> behavior, string fullMemberName)
            : base(behavior, fullMemberName)
        {
        }
    }
}
