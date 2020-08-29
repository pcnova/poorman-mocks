// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     Represents a custom behavior that adds to existing behavior, and
    ///     contains the options for running it.
    /// </summary>
    public class AddedBehavior : BehaviorOptions<Action<object[]>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AddedBehavior"/> class.
        /// </summary>
        /// <param name="behavior">
        ///     The added behavior action.
        /// </param>
        /// <param name="fullMemberName">
        ///     The name of the member being modified. This should be the
        ///     full name so it can be used to uniquely identify the member.
        /// </param>
        public AddedBehavior(
            Action<object[]> behavior, string fullMemberName)
            : base(behavior, fullMemberName)
        {
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to run the added behavior
        ///     before or after the member's default behavior.
        /// </summary>
        /// <value>
        ///     <c>True</c> to run after the member's behavior, <c>false</c>
        ///     to run before.
        /// </value>
        public bool RunAfter { get; set; }
    }
}
