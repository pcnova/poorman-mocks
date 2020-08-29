// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     Contains methods that extend the <see cref="Mock"/> type.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     This class simply provides convenience methods that wrap calls to
    ///     equivalent <see cref="Mock"/> methods. The methods pass an argument
    ///     representing the mock to the expressions that define the member to
    ///     modify or override.
    ///     </para>
    ///     <para>
    ///     This is useful when you don't have a variable referencing the mock
    ///     object that you're operating on, for example, when you've obtained
    ///     the mock from a factory method and you want to chain a call to set
    ///     behavior on it without saving it to a variable first.
    ///     </para>
    ///     <para>
    ///     Another useful application is when you do have a variable to the
    ///     mock, but the variable name is relatively long. By using the
    ///     argument passed by these methods, expressions can shorten the
    ///     argument name as desired, helping make statements more succinct.
    ///     </para>
    ///     <para>
    ///     See remarks on the <see cref="Mock"/> class for more info on what
    ///     a mock class needs to do to support the extension methods in this
    ///     class. Also see
    ///     <see cref="Mock.AddBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
    ///     for more on the reasoning behind these extensions.
    ///     </para>
    /// </remarks>
    public static class MockExtensions
    {
        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it. Use this when you don't have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior"/> and class-level
        ///     remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action behavior)
            where TMock : Mock
        {
            return target.AddBehaviorOf(memberCall, behavior);
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing one call
        ///     argument to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg> behavior)
                where TMock : Mock
        {
            return target.AddBehaviorOf(memberCall, behavior, typeof(TArg));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing two call
        ///     arguments to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg1, TArg2> behavior)
                where TMock : Mock
        {
            return target.AddBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing three call
        ///     arguments to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg1, TArg2, TArg3> behavior)
                where TMock : Mock
        {
            return target.AddBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing four call
        ///     arguments to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
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
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3, TArg4>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg1, TArg2, TArg3, TArg4> behavior)
                where TMock : Mock
        {
            return target.AddBehaviorOf(
                memberCall,
                behavior,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member. Use this when you
        ///     don't have a variable referencing the mock, and for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TData>(
                      this TMock target,
                      Expression<Func<TMock, TData>> memberCall,
                      Action behavior)
            where TMock : Mock
        {
            return target.AddBehaviorOf(memberCall, behavior);
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing one call
        ///     argument to be used by the behavior. Use this when you don't have
        ///     a variable referencing the mock, and for members that return a
        ///     value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg, TData>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg, TData> behavior)
            where TMock : Mock
        {
            return target.AddBehaviorOf(memberCall, behavior, typeof(TArg));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing two call
        ///     arguments to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TData>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg1, TArg2, TData> behavior)
            where TMock : Mock
        {
            return target.AddBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing three call
        ///     arguments to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
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
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3, TData>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg1, TArg2, TArg3, TData> behavior)
            where TMock : Mock
        {
            return target.AddBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member, allowing four call
        ///     arguments to be used by the behavior. Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
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
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.AddBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3, TArg4, TData>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg1, TArg2, TArg3, TArg4, TData> behavior)
            where TMock : Mock
        {
            return target.AddBehaviorOf(
                memberCall,
                behavior,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Replaces the specified member's behavior. Use this when you don't
        ///     have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior"/> and class-level
        ///     remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(memberCall, behavior);
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing one call
        ///     argument to be used by the custom behavior. Use this when you
        ///     don't have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(memberCall, behavior, typeof(TArg));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing two call
        ///     arguments to be used by the custom behavior. Use this when you
        ///     don't have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg1, TArg2> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing three call
        ///     arguments to be used by the custom behavior. Use this when you
        ///     don't have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg3">
        ///     The type of the third parameter required by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg1, TArg2, TArg3> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing four call
        ///     arguments to be used by the custom behavior. Use this when
        ///     you don't have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
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
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3, TArg4>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Action<TArg1, TArg2, TArg3, TArg4> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                memberCall,
                behavior,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Replaces the specified member's behavior. Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The mock that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TData>(
                      this TMock target,
                      Expression<Func<TMock, TData>> memberCall,
                      Func<TData> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(memberCall, behavior);
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing one call
        ///     argument to be used by the custom behavior. Use this when you
        ///     don't have a variable referencing the mock, and for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg">
        ///     The type of parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(memberCall, behavior, typeof(TArg));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing two call
        ///     arguments to be used by the custom behavior. Use this when you
        ///     don't have a variable referencing the mock, and for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
        /// <typeparam name="TArg1">
        ///     The type of the first parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TArg2">
        ///     The type of the second parameter required by the custom behavior.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the custom behavior.
        /// </typeparam>
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg1, TArg2, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing three call
        ///     arguments to be used by the custom behavior. Use this when you
        ///     don't have a variable referencing the mock, and for members that
        ///     return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
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
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg1, TArg2, TArg3, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                memberCall, behavior, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing four call
        ///     arguments to be used by the custom behavior. Use this when
        ///     you don't have a variable referencing the mock, and for members
        ///     that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of mock object on which the method is operating.
        /// </typeparam>
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
        /// <param name="target">
        ///     The mock object that is the target of the operation.
        /// </param>
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
        ///     See remarks on <see cref="Mock.SetBehavior{T}(Expression{Action}, Action{T})"/>
        ///     and class-level remarks for usage and other details.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3, TArg4, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> memberCall,
                      Func<TArg1, TArg2, TArg3, TArg4, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                memberCall,
                behavior,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }
    }
}
