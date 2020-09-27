// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using PoorMan.Mocks.Properties;

    /// <summary>
    ///     Contains the options for running a custom mock behavior.
    /// </summary>
    public class BehaviorOptions
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the behavior will run
        ///     only once, as opposed to every time the modified member is called.
        /// </summary>
        /// <value>
        ///     <c>True</c> if the behavior should run once; <c>false</c> otherwise.
        /// </value>
        public bool RunOnce { get; set; }
    }

    /// <summary>
    ///     Contains a custom mock behavior and the options for running it.
    /// </summary>
    /// <typeparam name="TBehavior">
    ///     The type of behavior to run, that is, a concrete type of delegate,
    ///     such as <see cref="Action"/> or <see cref="Func{TResult}"/>. If this
    ///     is not a delegate type, an exception will be thrown on construction.
    /// </typeparam>
    /// <remarks>
    ///     <para>
    ///     This class is intended for use by the <see cref="Mock"/> class only,
    ///     which is why its members are marked internal. Not doing so would
    ///     expose members that consumers don't actually need.
    ///     </para>
    /// </remarks>
    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are practically the same type.")]
    public class BehaviorOptions<TBehavior> : BehaviorOptions
           where TBehavior : class
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="BehaviorOptions{TBehavior}"/> class.
        /// </summary>
        /// <param name="behavior">
        ///     The behavior to be run.
        /// </param>
        /// <param name="fullMemberName">
        ///     The name of the member being modified. This should be the
        ///     full name so it can be used to uniquely identify the member.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="behavior"/> is null or not a delegate,
        ///     or if <paramref name="fullMemberName"/> is blank.
        /// </exception>
        protected BehaviorOptions(TBehavior behavior, string fullMemberName)
        {
            if (!(behavior is Delegate))
            {
                throw new ArgumentException(Resources.InvalidBehaviorType);
            }

            if (string.IsNullOrEmpty(fullMemberName))
            {
                throw new ArgumentException(
                    Resources.NullOrEmptyArgument, nameof(fullMemberName));
            }

            this.Behavior = behavior;
            this.MemberName = fullMemberName;
        }

        /// <summary>
        ///     Gets the behavior to run.
        /// </summary>
        /// <value>
        ///     The delegate given during construction that represents the
        ///     behavior to run.
        /// </value>
        /// <remarks>
        ///     <para>
        ///     This property is only of interest to the <see cref="Mock"/> class,
        ///     and maybe to derived classes, which is why it is marked protected
        ///     internal.
        ///     </para>
        /// </remarks>
        protected internal TBehavior Behavior { get; private set; }

        /// <summary>
        ///     Gets the full name of the member being modified by the behavior.
        /// </summary>
        /// <value>
        ///     The string representing the full name of the member.
        /// </value>
        /// <remarks>
        ///     <para>
        ///     This property is only of interest to the <see cref="Mock"/> class,
        ///     and maybe to derived classes, which is why it is marked protected
        ///     internal.
        ///     </para>
        /// </remarks>
        protected internal string MemberName { get; private set; }
    }
}
