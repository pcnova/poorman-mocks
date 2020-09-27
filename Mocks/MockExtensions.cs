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
    ///     override.
    ///     </para>
    ///     <para>
    ///     This is useful when you don't have a variable referencing the mock
    ///     object that you're operating on, for example, when you've obtained
    ///     the mock from a factory method and you want to chain a call to set
    ///     behavior on it without saving it to a variable first.
    ///     </para>
    ///     <para>
    ///     Another useful application is when you do have a variable to the mock,
    ///     but the variable name is relatively long. By using the argument passed
    ///     by these methods, expressions can shorten the argument name as desired,
    ///     helping make statements more succinct.
    ///     </para>
    ///     <para>
    ///     See remarks on the <see cref="Mock"/> class for more info on what
    ///     a mock class needs to do to support the extension methods in this
    ///     class. Also see
    ///     <see cref="Mock.AddBehaviorOf{TMock}(Expression{Action{TMock}},Action,bool)"/>
    ///     and <see cref="Mock.SetBehaviorOf{TMock}(Expression{Action{TMock}},Action)"/>
    ///     for more on the reasoning behind these extensions.
    ///     </para>
    /// </remarks>
    public static class MockExtensions
    {
        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it (see remarks). Use this when you don't have a variable
        ///     referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action behavior,
                      bool runAfter = false) where TMock : Mock
        {
            return target.AddBehaviorOf(callToMember, behavior, runAfter);
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing it,
        ///     allowing one call argument to be used by the behavior (see remarks).
        ///     Use this when you don't have a variable referencing the mock.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg> behavior,
                      bool runAfter = false)
                where TMock : Mock
        {
            return target.AddBehaviorOf(callToMember, behavior, runAfter, typeof(TArg));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing it,
        ///     allowing two call arguments to be used by the behavior (see remarks).
        ///     Use this when you don't have a variable referencing the mock.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg1, TArg2> behavior,
                      bool runAfter = false)
                where TMock : Mock
        {
            return target.AddBehaviorOf(callToMember, behavior, runAfter, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing it,
        ///     allowing three call arguments to be used by the behavior (see remarks).
        ///     Use this when you don't have a variable referencing the mock.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg1, TArg2, TArg3> behavior,
                      bool runAfter = false)
                where TMock : Mock
        {
            return target.AddBehaviorOf(
                callToMember, behavior, runAfter, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing it,
        ///     allowing four call arguments to be used by the behavior (see
        ///     remarks). Use this when you don't have a variable referencing the
        ///     mock.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are
        ///     required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3, TArg4>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg1, TArg2, TArg3, TArg4> behavior,
                      bool runAfter = false)
                where TMock : Mock
        {
            return target.AddBehaviorOf(
                callToMember,
                behavior,
                runAfter,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it (see remarks). Use this when you don't have a variable
        ///     referencing the mock, and for members that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member.
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TData>(
                      this TMock target,
                      Expression<Func<TMock, TData>> callToMember,
                      Action behavior,
                      bool runAfter = false) where TMock : Mock
        {
            return target.AddBehaviorOf(callToMember, behavior, runAfter);
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it, allowing one call argument to be used by the behavior (see
        ///     remarks). Use this when you don't have a variable referencing the
        ///     mock, and for members that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
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
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg, TData>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg, TData> behavior,
                      bool runAfter = false) where TMock : Mock
        {
            return target.AddBehaviorOf(callToMember, behavior, runAfter, typeof(TArg));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it, allowing two call arguments to be used by the behavior (see
        ///     remarks). Use this when you don't have a variable referencing the
        ///     mock, and for members that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
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
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TData>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg1, TArg2, TData> behavior,
                      bool runAfter = false) where TMock : Mock
        {
            return target.AddBehaviorOf(callToMember, behavior, runAfter, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it, allowing three call arguments to be used by the behavior (see
        ///     remarks). Use this when you don't have a variable referencing the
        ///     mock, and for members that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
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
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3, TData>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg1, TArg2, TArg3, TData> behavior,
                      bool runAfter = false) where TMock : Mock
        {
            return target.AddBehaviorOf(
                callToMember, behavior, runAfter, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Adds custom behavior to the specified member without replacing
        ///     it, allowing four call arguments to be used by the behavior (see
        ///     remarks). Use this when you don't have a variable referencing the
        ///     mock, and for members that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
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
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are
        ///     required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <param name="runAfter">
        ///     <c>True</c> to run the behavior after the specified member,
        ///     <c>false</c> to run it before.
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was added and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static AddedBehavior AddBehavior<TMock, TArg1, TArg2, TArg3, TArg4, TData>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg1, TArg2, TArg3, TArg4, TData> behavior,
                      bool runAfter = false) where TMock : Mock
        {
            return target.AddBehaviorOf(
                callToMember,
                behavior,
                runAfter,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Replaces the specified member's behavior (see remarks). Use this
        ///     when you don't have a variable referencing the mock.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
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
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior);
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing one call argument
        ///     to be used by the behavior (see remarks). Use this when you don't
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior, typeof(TArg));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing two call arguments
        ///     to be used by the behavior (see remarks). Use this when you don't
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg1, TArg2> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing three call arguments
        ///     to be used by the behavior (see remarks). Use this when you don't
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg1, TArg2, TArg3> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing four call
        ///     arguments to be used by the behavior (see remarks). Use this when
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are
        ///     required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3, TArg4>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Action<TArg1, TArg2, TArg3, TArg4> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                callToMember,
                behavior,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }

        /// <summary>
        ///     Replaces the specified member's behavior (see remarks). Use this
        ///     when you don't have a variable referencing the mock, and for
        ///     members that return a value.
        /// </summary>
        /// <typeparam name="TMock">
        ///     The type of object to operate on. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <typeparam name="TData">
        ///     The type of data returned by the member. This does not have to be
        ///     specified as it will be obtained via type inference.
        /// </typeparam>
        /// <param name="target">
        ///     The object that is the target of the operation.
        /// </param>
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
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
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TData>(
                      this TMock target,
                      Expression<Func<TMock, TData>> callToMember,
                      Func<TData> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior);
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing one call argument
        ///     to be used by the behavior (see remarks). Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior, typeof(TArg));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing two call arguments
        ///     to be used by the behavior (see remarks). Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg1, TArg2, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior, typeof(TArg1), typeof(TArg2));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing three call arguments
        ///     to be used by the behavior (see remarks). Use this when you don't
        ///     have a variable referencing the mock, and for members that return
        ///     a value.
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg1, TArg2, TArg3, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(callToMember, behavior, typeof(TArg1), typeof(TArg2), typeof(TArg3));
        }

        /// <summary>
        ///     Replaces the specified member's behavior, allowing four call
        ///     arguments to be used by the behavior (see remarks). Use this when
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
        /// <param name="callToMember">
        ///     The expression that specifies the member whose behavior will
        ///     be overridden. This should be a simple call to a method or
        ///     property. Note, however, that the member is not actually called;
        ///     so it doesn't matter what parameters are passed (if any are
        ///     required).
        /// </param>
        /// <param name="behavior">
        ///     The delegate that implements the desired behavior for the
        ///     specified member. This will be passed the original call
        ///     arguments, so the signature must match the overridden member (see
        ///     remarks for details).
        /// </param>
        /// <returns>
        ///     The object representing the behavior that was set and its options.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     See the homologous method in the <see cref="Mock"/> class, and
        ///     class-level remarks for more info.
        ///     </para>
        /// </remarks>
        public static ReplacementBehavior SetBehavior<TMock, TArg1, TArg2, TArg3, TArg4, TResult>(
                      this TMock target,
                      Expression<Action<TMock>> callToMember,
                      Func<TArg1, TArg2, TArg3, TArg4, TResult> behavior)
                where TMock : Mock
        {
            return target.SetBehaviorOf(
                callToMember,
                behavior,
                typeof(TArg1),
                typeof(TArg2),
                typeof(TArg3),
                typeof(TArg4));
        }
    }
}
