// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Tests
{
    /// <summary>
    ///     Overrides <see cref="BehaviorOptions{T}"/> for testing purposes.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the target behavior.
    /// </typeparam>
    public class BehaviorOptionsStub<T> : BehaviorOptions<T>
        where T : class
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="BehaviorOptionsStub{T}"/> class.
        /// </summary>
        /// <param name="behavior">
        ///     The behavior to set.
        /// </param>
        /// <param name="memberName">
        ///     The name of the member to override.
        /// </param>
        public BehaviorOptionsStub(T behavior, string memberName)
            : base(behavior, memberName)
        {
        }
    }
}