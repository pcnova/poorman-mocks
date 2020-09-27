// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using PoorMan.Mocks.Extensions;
    using PoorMan.Mocks.Properties;

    /// <summary>
    ///     Represents an object that allows modifying its behavior at runtime
    ///     for testing purposes, and implements basic functionality to build
    ///     such objects.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     A *very* poor man's idea of mocking an interface could be to simply
    ///     derive from it, implementing default behavior that makes sense for
    ///     the tests at hand.
    ///     </para>
    ///     <para>
    ///     If there was a need to test a separate case or feature, that man
    ///     could create another derived class, or keep modifying the mock to
    ///     account for all cases to be tested. This could quickly become
    ///     untenable either way.
    ///     </para>
    ///     <para>
    ///     This class was designed to improve on this situation. That is, this
    ///     is intended to allow implementing a mock class once (for a given
    ///     interface), and to allow quick, explicit, and temporary modification
    ///     of that mock's behavior on the fly, right from a unit test (as
    ///     opposed to making permanent, design-time modifications to one or
    ///     many mock classes).
    ///     </para>
    ///     <para>
    ///     To support this, a mock class will have to be manually modified as
    ///     follows (but hopefully just once, and in a less onerous way than
    ///     what would be needed otherwise):
    ///     </para>
    ///     <list type="number">
    ///     <item>
    ///         <description>
    ///             Derive from this class.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Call a special method from every member that should support
    ///             custom behavior: <see cref="RunCustomBehaviorOr"/> (or its
    ///             overloads).
    ///         </description>
    ///     </item>
    ///     </list>
    ///     <para>
    ///     After this, a consumer of the derived mock class can modify it with
    ///     custom behavior by calling <see cref="AddBehavior"/> or
    ///     <see cref="SetBehavior"/>, which would augment or replace the mock's
    ///     default behavior, respectively.
    ///     </para>
    ///     <para>
    ///     Note also the <see cref="MockExtensions"/>, for cases where you don't
    ///     have (or don't want) a variable referencing the mock, and the
    ///     <see cref="BehaviorOptionsExtensions.With"/> extension, which allows
    ///     configuring a couple options for how behavior should run.
    ///     </para>
    /// </remarks>
    public class Mock
    {
        /// <summary>
        ///     The dictionary that maps member names to their custom behaviors.
        /// </summary>
        private readonly Dictionary<string, ReplacementBehavior> behaviors =
                     new Dictionary<string, ReplacementBehavior>();

        /// <summary>
        ///     The dictionary that maps member names to custom behaviors that
        ///     add to, instead of replace, their regular behavior.
        /// </summary>
        private readonly Dictionary<string, AddedBehavior> addedBehaviors =
                     new Dictionary<string, AddedBehavior>();

        /// <summary>
        ///     Replaces the specified member's behavior.
        /// </summary>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     Calling this method, or its overloads, has no effect if the mock
        ///     has not been modified to support custom behavior; see class-level
        ///     remarks.
        ///     </para>
        ///     <para>
        ///     The <paramref name="memberCall"/> should be a simple call to a
        ///     method or property. Note, however, that the member is not
        ///     actually called; so it doesn't matter what parameters are passed
        ///     (if any).
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior(
            Expression<Action> memberCall, Action behavior)
        {
            // Wrap the given behavior in a function that returns null, this
            // allows us to use a single dictionary of behaviors.
            var customBehavior = new ReplacementBehavior(
                args =>
                    {
                        behavior();
                        return null;
                    },
                this.GetMockable(memberCall).ToString());

            this.behaviors.AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use one call argument.
        /// </summary>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     Calling this method, or its overloads, has no effect if the mock
        ///     has not been modified to support custom behavior; see class-level
        ///     remarks.
        ///     </para>
        ///     <para>
        ///     The <paramref name="memberCall"/> should be a simple call to a
        ///     method or property. Note, however, that the member is not
        ///     actually called; so it doesn't matter what parameters are passed
        ///     (if any).
        ///     </para>
        ///     <para>
        ///     The <paramref name="behavior"/> signature must match the
        ///     overridden member in order to be passed argument(s). However,
        ///     note that it's OK if the overridden member actually requires
        ///     more arguments. See <see cref="SetBehaviorWithArgs"/> for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg>(
            Expression<Action> memberCall,
            Action<TArg> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use two call arguments.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg1, TArg2>(
            Expression<Action> memberCall,
            Action<TArg1, TArg2> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg1),
                typeof(TArg2));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use three call arguments.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg1, TArg2, TArg3>(
            Expression<Action> memberCall,
            Action<TArg1, TArg2, TArg3> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use four call arguments.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg4">
        ///     The type of the fourth parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg1, TArg2, TArg3, TArg4>(
            Expression<Action> memberCall,
            Action<TArg1, TArg2, TArg3, TArg4> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Replaces the specified member's behavior. Use this for members
        ///     that return a value.
        /// </summary>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior(Expression{Action}, Action)"/> for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TData>(
            Expression<Func<TData>> memberCall,
            Func<TData> behavior)
        {
            // Wrap the given behavior in a function that returns a generic
            // object, this allows us to use a single dictionary of behaviors.
            var customBehavior = new ReplacementBehavior(
                args => behavior.Invoke(),
                this.GetMockable(memberCall).ToString());

            this.behaviors.AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use one call argument. Use this for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg, TResult>(
            Expression<Func<TResult>> memberCall,
            Func<TArg, TResult> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use two call arguments. Use this for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg1, TArg2, TResult>(
            Expression<Func<TResult>> memberCall,
            Func<TArg1, TArg2, TResult> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg1),
                typeof(TArg2));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use three call arguments. Use this for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg1, TArg2, TArg3, TResult>(
            Expression<Func<TResult>> memberCall,
            Func<TArg1, TArg2, TArg3, TResult> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, letting the custom
        ///     behavior use four call arguments (see remarks). Use this for
        ///     members that return a value.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg4">
        ///     The type of the fourth parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="SetBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public ReplacementBehavior SetBehavior<TArg1, TArg2, TArg3, TArg4, TResult>(
            Expression<Func<TResult>> memberCall,
            Func<TArg1, TArg2, TArg3, TArg4, TResult> behavior)
        {
            return this.SetBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it.
        /// </summary>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     Calling this method, or its overloads, has no effect if the mock
        ///     has not been modified to support custom behavior; see class-level
        ///     remarks.
        ///     </para>
        ///     <para>
        ///     The <paramref name="memberCall"/> should be a simple call to a
        ///     method or property. Note, however, that the member is not
        ///     actually called; so it doesn't matter what parameters are passed
        ///     (if any).
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior(
            Expression<Action> memberCall, Action behavior, bool runAfter = false)
        {
            // The object representing the added behavior.
            var customBehavior = new AddedBehavior(
                args => behavior(),
                this.GetMockable(memberCall).ToString(),
                runAfter);

            this.addedBehaviors
                .AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use one call argument.
        /// </summary>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     Calling this method, or its overloads, has no effect if the mock
        ///     has not been modified to support custom behavior; see class-level
        ///     remarks.
        ///     </para>
        ///     <para>
        ///     The <paramref name="memberCall"/> should be a simple call to a
        ///     method or property. Note, however, that the member is not
        ///     actually called; so it doesn't matter what parameters are passed
        ///     (if any).
        ///     </para>
        ///     <para>
        ///     The <paramref name="behavior"/> signature must match the
        ///     overridden member in order to be passed argument(s). However,
        ///     note that it's OK if the overridden member actually requires
        ///     more arguments. See <see cref="AddBehaviorWithArgs"/> for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg>(
            Expression<Action> memberCall,
            Action<TArg> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use two call arguments.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg1, TArg2>(
            Expression<Action> memberCall,
            Action<TArg1, TArg2> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg1),
                typeof(TArg2));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use three call arguments.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg1, TArg2, TArg3>(
            Expression<Action> memberCall,
            Action<TArg1, TArg2, TArg3> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use four call arguments.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg4">
        ///     The type of the fourth parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg1, TArg2, TArg3, TArg4>(
            Expression<Action> memberCall,
            Action<TArg1, TArg2, TArg3, TArg4> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member. Use this for
        ///     members that return a value.
        /// </summary>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior(Expression{Action}, Action)"/> for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TData>(
            Expression<Func<TData>> memberCall,
            Action behavior,
            bool runAfter = false)
        {
            // The object representing the added behavior.
            var customBehavior = new AddedBehavior(
                args => behavior(),
                this.GetMockable(memberCall).ToString(),
                runAfter);

            this.addedBehaviors
                .AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use one call argument. Use this for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg, TData>(
            Expression<Func<TData>> memberCall,
            Action<TArg> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use two call arguments. Use this for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg1, TArg2, TData>(
            Expression<Func<TData>> memberCall,
            Action<TArg1, TArg2> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg1),
                typeof(TArg2));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use three call arguments. Use this for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg1, TArg2, TArg3, TData>(
            Expression<Func<TData>> memberCall,
            Action<TArg1, TArg2, TArg3> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, letting the
        ///     behavior use four call arguments. Use this for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg4">
        ///     The type of the fourth parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehavior{TArg}(Expression{Action}, Action{TArg})"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        public AddedBehavior AddBehavior<TArg1, TArg2, TArg3, TArg4, TData>(
            Expression<Func<TData>> memberCall,
            Action<TArg1, TArg2, TArg3, TArg4> behavior,
            bool runAfter = false)
        {
            return this.AddBehaviorWithArgs(
                behavior,
                this.GetMockable(memberCall),
                runAfter,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member of the given mock
        ///     type. This is a support method for the <see cref="MockExtensions"/>.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     By requiring the <typeparamref name="TMock"/> parameter, this
        ///     method, and similar overloads, allow callers access to the mock
        ///     instance when specifying the member to modify. This means that a
        ///     separate variable referencing the mock is not needed, which can
        ///     be useful in some cases.
        ///     </para>
        ///     <para>
        ///     However, since this is a non-generic class, using this method
        ///     as-is would be cumbersome, because it forces the user to specify
        ///     the type of the derived mock object when calling it.
        ///     </para>
        ///     <para>
        ///     To avoid this, the method was made internal so that it's not
        ///     visible to outside users, which should instead use the
        ///     <see cref="MockExtensions.AddBehavior{TMock}"/> extension (and
        ///     its overloads). The extension allows type inference to work, and
        ///     so is much more convenient and natural to use.
        ///     </para>
        ///     <para>
        ///     See remarks on <see cref="AddBehavior(Expression{Action}, Action)"/>
        ///     for general information on adding behavior.
        ///     </para>
        /// </remarks>
        internal AddedBehavior AddBehaviorOf<TMock>(
            Expression<Action<TMock>> memberCall,
            Action behavior,
            bool runAfter)
            where TMock : Mock
        {
            // The object representing the added behavior.
            var customBehavior = new AddedBehavior(
                args => behavior(),
                default(TMock).GetMockable(memberCall).ToString(),
                runAfter);

            this.addedBehaviors
                .AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Adds custom behavior to the specified member of the given mock
        ///     type, letting the behavior return data. This is a support method
        ///     for the <see cref="MockExtensions"/>.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        internal AddedBehavior AddBehaviorOf<TMock, TData>(
            Expression<Func<TMock, TData>> memberCall,
            Action behavior,
            bool runAfter)
            where TMock : Mock
        {
            // The object representing the added behavior.
            var customBehavior = new AddedBehavior(
                args => behavior(),
                default(TMock).GetMockable(memberCall).ToString(),
                runAfter);

            this.addedBehaviors
                .AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Adds custom behavior to the specified member of the given mock
        ///     type, letting the behavior use call arguments. This is a support
        ///     method for the <see cref="MockExtensions"/>.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <param name="orderedArgTypes">
        ///     The <see cref="Type"/> objects representing each argument required
        ///     by the <paramref name="behavior"/>, in the exact order in which
        ///     they're expected.
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     This method accepts a generic delegate to avoid having to define
        ///     overloads for different argument counts. This works because this
        ///     is for internal use by the <see cref="MockExtensions"/>, which
        ///     have the onus of ensuring that the <paramref name="orderedArgTypes"/>
        ///     are correct for the delegate.
        ///     </para>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
        ///     for more on adding behavior and the <see cref="MockExtensions"/>.
        ///     </para>
        /// </remarks>
        internal AddedBehavior AddBehaviorOf<TMock>(
            Expression<Action<TMock>> memberCall,
            Delegate behavior,
            bool runAfter,
            params Type[] orderedArgTypes)
            where TMock : Mock
        {
            return this.AddBehaviorWithArgs(
                behavior,
                default(TMock).GetMockable(memberCall),
                runAfter,
                orderedArgTypes);
        }

        /// <summary>
        ///     Replaces the behavior of the specified member of the given mock
        ///     type. This is a support method for the <see cref="MockExtensions"/>.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
        ///     for more on custom behavior and the <see cref="MockExtensions"/>.
        ///     </para>
        /// </remarks>
        internal ReplacementBehavior SetBehaviorOf<TMock>(
                 Expression<Action<TMock>> memberCall,
                 Action behavior)
           where TMock : Mock
        {
            // Wrap the given behavior in a function that returns null, this
            // allows us to use a single dictionary of behaviors.
            var customBehavior = new ReplacementBehavior(
                args =>
                    {
                        behavior();
                        return null;
                    },
                default(TMock).GetMockable(memberCall).ToString());

            this.behaviors.AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Replaces the behavior of the specified member of the given mock
        ///     type, letting the behavior return data. This is a support method
        ///     for the <see cref="MockExtensions"/>.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
        ///     for more on custom behavior and the <see cref="MockExtensions"/>.
        ///     </para>
        /// </remarks>
        internal ReplacementBehavior SetBehaviorOf<TMock, TData>(
                 Expression<Func<TMock, TData>> memberCall,
                 Func<TData> behavior)
           where TMock : Mock
        {
            // Wrap the given behavior in a function that returns a generic
            // object, this allows us to use a single dictionary of behaviors.
            var customBehavior = new ReplacementBehavior(
                args => behavior.Invoke(),
                default(TMock).GetMockable(memberCall).ToString());

            this.behaviors.AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Replaces the behavior of the specified member of the given mock
        ///     type, letting the behavior use call arguments. This is a support
        ///     method for the <see cref="MockExtensions"/>.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden (see remarks).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments (see remarks).
        /// </param>
        /// <param name="orderedArgTypes">
        ///     The <see cref="Type"/> objects representing each argument required
        ///     by the <paramref name="behavior"/>, in the exact order in which
        ///     they're expected.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See remarks on
        ///     <see cref="AddBehaviorOf{TMock}(Expression{Action{TMock}},Delegate,Type[])"/>
        ///     for more.
        ///     </para>
        /// </remarks>
        internal ReplacementBehavior SetBehaviorOf<TMock>(
                 Expression<Action<TMock>> memberCall,
                 Delegate behavior,
                 params Type[] orderedArgTypes)
           where TMock : Mock
        {
            return this.SetBehaviorWithArgs(
                behavior,
                default(TMock).GetMockable(memberCall),
                orderedArgTypes);
        }

        /// <summary>
        ///     Runs custom behavior set by a previous call to <see cref="AddBehavior"/>
        ///     and/or <see cref="SetBehavior"/>, if any, otherwise runs the
        ///     specified default behavior.
        /// </summary>
        /// <param name="behavior">
        ///     The default behavior to run if no custom behavior has been set
        ///     for the given member.
        /// </param>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden.
        /// </param>
        /// <param name="arguments">
        ///     The arguments, if any, that were passed to the member being
        ///     overridden, in the same order (see remarks for details). 
        /// </param>
        /// <remarks>
        ///     <para>
        ///     The <paramref name="arguments"/> are needed in order to pass
        ///     them through to the custom behavior set for the overridden member.
        ///     It's very important to pass the same arguments passed in the call
        ///     to the member, and in the same order, otherwise the call will
        ///     fail, or it will yield unexpected results.
        ///     </para>
        ///     <para>
        ///     Note that this means that, when the signature of the member
        ///     changes, the <see cref="arguments"/> passed to this method may
        ///     need to change as well. For most signature changes (add, remove,
        ///     or a change to the type of an argument), either the compiler or
        ///     a runtime error will provide an indication that the arguments
        ///     passed to this method need to change.
        ///     </para>
        ///     <para>
        ///     However, when the order is changed for arguments of the same type,
        ///     there will be no compiler or runtime error, but the call will
        ///     result in unexpected behavior. It is unfortunate that this can't
        ///     be detected, because it will probably be annoying and difficult
        ///     to debug, but it seems like something that should rarely happen,
        ///     and that well-designed tests should catch. So, at least for now,
        ///     it looks like the benefits (custom behavior being able to access
        ///     arguments) outweigh the risks.
        ///     </para>
        ///     <para>
        ///     See class remarks for more on the purpose and usage of this method.
        ///     </para>
        /// </remarks>
        protected void RunCustomBehaviorOr(Action behavior, Expression<Action> memberCall, params object[] arguments)
        {
            // Wrap the given behavior in a function that returns null, this
            // allows us to re-use the overload.
            this.RunCustomBehaviorOr<object>(
                args =>
                {
                    behavior();
                    return null;
                },
                this.GetMockable(memberCall).ToString(),
                arguments);
        }

        /// <summary>
        ///     Runs custom behavior set by a previous call to <see cref="AddBehavior"/>
        ///     and/or <see cref="SetBehavior"/>, if any, otherwise runs the
        ///     specified default behavior.
        /// </summary>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="behavior">
        ///     The default behavior to run if no custom behavior has been set
        ///     for the given member.
        /// </param>
        /// <param name="memberCall">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden.
        /// </param>
        /// <param name="arguments">
        ///     The arguments, if any, that were passed to the member being
        ///     overridden, in the same order (see remarks for details). 
        /// </param>
        /// <returns>
        ///     The data returned by the behavior.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     The <paramref name="arguments"/> are needed in order to pass
        ///     them through to the custom behavior set for the overridden member.
        ///     It's very important to pass the same arguments passed in the call
        ///     to the member, and in the same order, otherwise the call will
        ///     fail, or it will yield unexpected results.
        ///     </para>
        ///     <para>
        ///     Note that this means that, when the signature of the member
        ///     changes, the <see cref="arguments"/> passed to this method may
        ///     need to change as well. For most signature changes (add, remove,
        ///     or a change to the type of an argument), either the compiler or
        ///     a runtime error will provide an indication that the arguments
        ///     passed to this method need to change.
        ///     </para>
        ///     <para>
        ///     However, when the order is changed for arguments of the same type,
        ///     there will be no compiler or runtime error, but the call will
        ///     result in unexpected behavior. It is unfortunate that this can't
        ///     be detected, because it will probably be annoying and difficult
        ///     to debug, but it seems like something that should rarely happen,
        ///     and that well-designed tests should catch. So, at least for now,
        ///     it looks like the benefits (custom behavior being able to access
        ///     arguments) outweigh the risks.
        ///     </para>
        ///     <para>
        ///     See class remarks for more on the purpose and usage of this method.
        ///     </para>
        /// </remarks>
        protected TData RunCustomBehaviorOr<TData>(Func<TData> behavior, Expression<Func<TData>> memberCall, params object[] arguments)
        {
            return this.RunCustomBehaviorOr(args => behavior(), this.GetMockable(memberCall).ToString(), arguments);
        }

        /// <summary>
        ///     Validates that the given argument values match the given argument
        ///     information.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments to be validated against the <paramref name="orderedArgTypes"/>.
        /// </param>
        /// <param name="orderedArgTypes">
        ///     The <see cref="Type"/> objects representing each argument required
        ///     by some behavior or operation, in the order in which they're
        ///     expected.
        /// </param>
        /// <returns>
        ///     <c>True</c> if there are as many arguments as required argument
        ///     types, and if each argument matches one of the types, in the order
        ///     specified by the lists; <c>false</c> otherwise.
        /// </returns>
        private static bool ValidateBehaviorArguments(object[] arguments, params Type[] orderedArgTypes)
        {
            // Make sure we have enough arguments to pair up with the given types,
            // and then compare the types to ensure they match. Note that we ignore
            // any arguments in excess of the required types, since that means the
            // behavior has no use for them. Also, null is an allowed value for a
            // reference type, so we must skip checking the type on that (which
            // would fail too).
            return arguments.Length >= orderedArgTypes.Length &&
                   orderedArgTypes.Select((t, i) => new
                   {
                       Type = t,
                       Value = arguments[i]
                   })
                                  .All(arg => (arg.Value == null && !arg.Type.IsValueType) ||
                                               arg.Type.IsInstanceOfType(arg.Value));
        }

        /// <summary>
        ///     Gets a string describing the location where the specified behavior
        ///     was defined or set into the mock as custom behavior.
        /// </summary>
        /// <param name="behavior">
        ///     The delegate representing some custom behavior.
        /// </param>
        /// <returns>
        ///     A string describing the class where the specified behavior was
        ///     defined, or at least, a string describing how to find the places
        ///     where the behavior is used.
        /// </returns>
        private static string GetLocation(Delegate behavior)
        {
            string locationMsg;

            // Find the type where the behavior is declared.
            var type = behavior.Method.DeclaringType;

            // If the declaring type happens to be a nested class, we want to
            // provide information on the parent type (which is more visible and
            // so, easier to find), so look for a parent declaring type.
            while (type != null && type.DeclaringType != null)
            {
                type = type.DeclaringType;
            }

            if (type == null)
            {
                // This is weird, but could happen as R# points out. Basically,
                // the behavior would have to be a global method imported from
                // VB.NET or something...
                locationMsg =
                    "To find this call (or calls), look for usages of this method: " +
                    behavior.Method.Name +
                    " (this seems to be a global method in a separate module... what are you doing?).";
            }
            else if (behavior.Method.DeclaringType.IsGenerated() ||
                     behavior.Method.IsPrivate)
            {
                // This is an anonymous method or a private method in some class,
                // so let's simply point to the class where this is defined.
                locationMsg = "This call is in " + type.FullName + ".";
            }
            else
            {
                // This is a public method in some class, so we can't tell where
                // this is being called from... provide the full type name so
                // the caller can look for usages of the thing.
                locationMsg =
                    "To find this call (or calls), look for usages of this method: " +
                    (type.FullName + "." + behavior.Method.Name) + ".";
            }

            return locationMsg;
        }

        /// <summary>
        ///     Runs custom behavior set by a previous call to <see cref="AddBehavior"/>
        ///     and/or <see cref="SetBehavior"/>, if any, otherwise runs the
        ///     specified default behavior.
        /// </summary>
        /// <typeparam name="TData">
        ///     The type of data returned by the member.
        /// </typeparam>
        /// <param name="behavior">
        ///     The default behavior to run if no custom behavior has been set
        ///     for the given member.
        /// </param>
        /// <param name="member">
        ///     The member whose behavior will be overridden.
        /// </param>
        /// <param name="behaviorArgs">
        ///     The arguments to pass to the behavior to execute (that is, the
        ///     same arguments passed by the original caller).
        /// </param>
        /// <returns>
        ///     The data returned by the behavior.
        /// </returns>
        /// <remarks>
        ///     This member is private to discourage hard-coding of member names.
        ///     See class remarks for more on the purpose and usage of this method.
        /// </remarks>
        private TData RunCustomBehaviorOr<TData>(Func<object[], TData> behavior, string member, params object[] behaviorArgs)
        {
            // The behavior to execute instead of the default behavior (if any).
            ReplacementBehavior customBehavior;
            this.behaviors.TryGetValue(member, out customBehavior);

            // The behavior to execute along with any other behavior.
            AddedBehavior addedBehavior;
            this.addedBehaviors.TryGetValue(member, out addedBehavior);

            // Check whether we need to execute an added behavior.
            if (addedBehavior != null && !addedBehavior.RunAfter)
            {
                this.TryRun(addedBehavior, behaviorArgs);
            }

            // Execute the custom behavior if we have one, otherwise the given
            // default behavior.
            var result = customBehavior != null
                             ? this.TryRun<TData>(customBehavior, behaviorArgs)
                             : behavior(behaviorArgs);

            if (addedBehavior != null && addedBehavior.RunAfter)
            {
                this.TryRun(addedBehavior, behaviorArgs);
            }

            return result;
        }

        /// <summary>
        ///     Runs the specified behavior inside a try block, so that any
        ///     post-execution tasks are run regardless of whether the behavior
        ///     throws or not.
        /// </summary>
        /// <param name="behavior">
        ///     The object representing the behavior to run.
        /// </param>
        /// <param name="args">
        ///     The arguments to pass to the behavior.
        /// </param>
        /// <remarks>
        ///     <para>
        ///     This method does not handle exceptions thrown by the behavior
        ///     because a) it does not have a good way of handling them, and b)
        ///     the behavior's entire purpose may be to throw an exception.
        ///     </para>
        /// </remarks>
        private void TryRun(AddedBehavior behavior, object[] args)
        {
            try
            {
                behavior.Behavior(args);
            }
            finally
            {
                if (behavior.RunOnce)
                {
                    this.addedBehaviors.Remove(behavior.MemberName);
                }
            }
        }

        /// <summary>
        ///     Runs the specified behavior inside a try block, so that any
        ///     post-execution tasks are run regardless of whether the behavior
        ///     throws or not.
        /// </summary>
        /// <typeparam name="TData">
        ///     The type of data returned by the behavior.
        /// </typeparam>
        /// <param name="behavior">
        ///     The object representing the behavior to run.
        /// </param>
        /// <param name="args">
        ///     The arguments to pass to the behavior.
        /// </param>
        /// <returns>
        ///     The data returned by the behavior.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     This method does not handle exceptions thrown by the behavior
        ///     because a) it does not have a good way of handling them, and b)
        ///     the behavior's entire purpose may be to throw an exception.
        ///     </para>
        /// </remarks>
        private TData TryRun<TData>(ReplacementBehavior behavior, object[] args)
        {
            try
            {
                return (TData)behavior.Behavior(args);
            }
            finally
            {
                if (behavior.RunOnce)
                {
                    this.behaviors.Remove(behavior.MemberName);
                }
            }
        }

        /// <summary>
        ///     Replaces the behavior of the specified member, letting the
        ///     behavior use call arguments. This method supports the more
        ///     strongly-typed versions of it, see remarks.
        /// </summary>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="memberInfo">
        ///     The object identifying the member being overridden with custom
        ///     behavior.
        /// </param>
        /// <param name="orderedArgTypes">
        ///     The <see cref="Type"/> objects representing each argument required
        ///     by the <paramref name="behavior"/>, in the exact order in which
        ///     they're expected.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     This method is intended for use by the overloads of
        ///     <see cref="SetBehavior"/> and
        ///     <see cref="SetBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
        ///     that allow passing call arguments to custom behavior. This avoids
        ///     duplicating logic but also puts a strong onus on callers to ensure
        ///     that the given <paramref name="orderedArgTypes"/> are correct
        ///     for the given <paramref name="behavior"/> delegate, otherwise
        ///     custom behavior will fail to execute through no fault of the user.
        ///     </para>
        ///     <para>
        ///     Note that it's OK if the member being overridden requires more
        ///     arguments than the ones actually defined for the custom behavior.
        ///     That is, the custom behavior will execute successfully, with less
        ///     arguments, as long as the arguments match the order and type of
        ///     the arguments defined for the member (from left to right). This
        ///     allows defining simpler custom behavior methods that can ignore
        ///     arguments that they don't require. For more on this, see how
        ///     <see cref="Execute"/> works.
        ///     </para>
        ///     <para>
        ///     See class remarks for more on the general purpose and design
        ///     behind this method.
        ///     </para>
        /// </remarks>
        private ReplacementBehavior SetBehaviorWithArgs(Delegate behavior, MemberInfo memberInfo, params Type[] orderedArgTypes)
        {
            // The object representing the behavior to replace.
            var customBehavior =
                new ReplacementBehavior(
                    args => this.Execute(behavior, memberInfo, args, orderedArgTypes),
                    memberInfo.ToString());

            this.behaviors.AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Adds behavior to the specified member, letting the behavior use
        ///     call arguments. This method supports the more strongly-typed
        ///     versions of it, see remarks.
        /// </summary>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="memberInfo">
        ///     The object identifying the member being overridden with custom
        ///     behavior.
        /// </param>
        /// <param name="runAfter">
        ///     The value indicating whether to run the behavior before or after
        ///     the overridden member.
        /// </param>
        /// <param name="orderedArgTypes">
        ///     The <see cref="Type"/> objects representing each argument required
        ///     by the <paramref name="behavior"/>, in the exact order in which
        ///     they're expected.
        /// </param>
        /// <returns>
        ///     The object representing the added behavior and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     This method is intended for use by the overloads of
        ///     <see cref="AddBehavior"/> and
        ///     <see cref="AddBehaviorOf{TMock}(Expression{Action{TMock}},Action,bool)"/>
        ///     that allow passing call arguments to custom behavior. This avoids
        ///     duplicating logic but also puts a strong onus on callers to ensure
        ///     that the given <paramref name="orderedArgTypes"/> are correct
        ///     for the given <paramref name="behavior"/> delegate, otherwise
        ///     custom behavior will fail to execute through no fault of the user.
        ///     </para>
        ///     <para>
        ///     Note that it's OK if the member being overridden requires more
        ///     arguments than the ones actually defined for the custom behavior.
        ///     That is, the custom behavior will execute successfully, with less
        ///     arguments, as long as the arguments match the order and type of
        ///     the arguments defined for the member (from left to right). This
        ///     allows defining simpler custom behavior methods that can ignore
        ///     arguments that they don't require. For more on this, see how
        ///     <see cref="Execute"/> works.
        ///     </para>
        ///     <para>
        ///     See class remarks for more on the general purpose and design
        ///     behind this method.
        ///     </para>
        /// </remarks>
        private AddedBehavior AddBehaviorWithArgs(Delegate behavior, MemberInfo memberInfo, bool runAfter, params Type[] orderedArgTypes)
        {
            // The object representing the added behavior.
            var customBehavior = new AddedBehavior(
                args => this.Execute(behavior, memberInfo, args, orderedArgTypes),
                memberInfo.ToString(),
                runAfter);

            this.addedBehaviors.AddOrUpdate(customBehavior.MemberName, customBehavior);

            return customBehavior;
        }

        /// <summary>
        ///     Executes custom behavior for a member, ensuring that the
        ///     behavior's arguments are valid according to the given argument
        ///     types.
        /// </summary>
        /// <param name="behavior">
        ///     The custom behavior to execute.
        /// </param>
        /// <param name="memberInfo">
        ///     The information about the member that is being overridden by
        ///     the given <paramref name="behavior"/>.
        /// </param>
        /// <param name="args">
        ///     The arguments to be passed to the <paramref name="behavior"/>.
        ///     These will be validated against the <paramref name="orderedArgTypes"/>.
        /// </param>
        /// <param name="orderedArgTypes">
        ///     The <see cref="Type"/> objects representing each argument required
        ///     by the <paramref name="behavior"/>, in the exact order in which
        ///     they're expected.
        /// </param>
        /// <returns>
        ///     The result returned by the <paramref name="behavior"/>.
        /// </returns>
        private object Execute(Delegate behavior, MemberInfo memberInfo, object[] args, params Type[] orderedArgTypes)
        {
            if (!ValidateBehaviorArguments(args, orderedArgTypes))
            {
                this.ThrowBehaviorArgumentMismatchError(memberInfo.Name, behavior);
            }

            // Pass only the arguments required by the custom behavior; any extra
            // arguments harm nothing and can be ignored, but not taking them out
            // here would cause the dynamic invoke to fail.
            try
            {
                return behavior.DynamicInvoke(args.Take(orderedArgTypes.Length).ToArray());
            }
            catch (TargetInvocationException targetError)
            {
                // If we let the target invocation exception go, we expose the
                // caller to the mechanics of the mock, which they are not
                // interested in; they expect the original error to be thrown.
                // By throwing the inner exception, we preserve the caller's
                // expectations, and become transparent to them.
                if (targetError.InnerException != null)
                {
                    throw targetError.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        ///     Throws an exception specifying that there is a mismatch in the
        ///     arguments provided for a member's custom behavior, and provides
        ///     descriptive information about how to fix the error.
        /// </summary>
        /// <param name="memberName">
        ///     The name of the member being overridden with custom behavior.
        /// </param>
        /// <param name="behavior">
        ///     The custom behavior that failed to execute because of bad arguments.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown when this method is called, as specified in the summary.
        /// </exception>
        private void ThrowBehaviorArgumentMismatchError(string memberName, Delegate behavior)
        {
            var mockName = this.GetType().Name;

            throw new ArgumentException(
                string.Format(
                        Resources.MockBehaviorArgumentMismatchError,
                        memberName,
                        mockName + "." + memberName,
                        nameof(this.RunCustomBehaviorOr),
                        GetLocation(behavior)));
        }
    }
}
