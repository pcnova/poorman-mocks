// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using PoorMan.Mocks.Properties;

    /// <summary>
    ///     Contains methods that extend the <see cref="Expression"/> type for
    ///     the purposes of the <see cref="Mock"/> class.
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        ///     Gets metadata for the member identified by the given expression,
        ///     if the member can be mocked.
        /// </summary>
        /// <param name="memberCall">
        ///     The lambda expression that calls (or accesses) the member whose
        ///     info will be obtained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> does
        ///     not identify a valid member.
        /// </exception>
        /// <returns>
        ///     An object containing information about the member specified in
        ///     <paramref name="memberCall"/>.
        /// </returns>
        public static MemberInfo GetMockable(
            this Expression<Action> memberCall)
        {
            return ParseMockable(memberCall);
        }

        /// <summary>
        ///     Gets metadata for the member identified by the given expression,
        ///     if the member can be mocked. Use this for expressions that take
        ///     a parameter.
        /// </summary>
        /// <typeparam name="TParam">
        ///     The type of parameter that the specified member accepts.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The lambda expression that calls (or accesses) the member whose
        ///     info will be obtained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> does
        ///     not identify a valid member.
        /// </exception>
        /// <returns>
        ///     An object containing information about the member specified in
        ///     <paramref name="memberCall"/>.
        /// </returns>
        public static MemberInfo GetMockable<TParam>(
            this Expression<Action<TParam>> memberCall)
        {
            return ParseMockable(memberCall);
        }

        /// <summary>
        ///     Gets metadata for the member identified by the given expression,
        ///     if the member can be mocked. Use this for expressions that return
        ///     a value.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of result returned by the member to work on. This does
        ///     not need to be specified as it will be obtained through
        ///     type inference.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The lambda expression that calls (or accesses) the member whose
        ///     information will be obtained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> does
        ///     not identify a valid member.
        /// </exception>
        /// <returns>
        ///     An object containing information about the member specified in
        ///     <paramref name="memberCall"/>.
        /// </returns>
        public static MemberInfo GetMockable<TResult>(
            this Expression<Func<TResult>> memberCall)
        {
            return ParseMockable(memberCall);
        }

        /// <summary>
        ///     Gets metadata for the member identified by the given expression,
        ///     if the member can be mocked. Use this for expressions that take
        ///     a parameter and return a value.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of parameter used by the member call. This does not need
        ///     to be specified as it will be obtained through type inference.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of result returned by the member to work on. This does
        ///     not need to be specified as it will be obtained through
        ///     type inference.
        /// </typeparam>
        /// <param name="memberCall">
        ///     The lambda expression that calls (or accesses) the member whose
        ///     information will be obtained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Thrown (by helper method) if <paramref name="memberCall"/> does
        ///     not identify a valid member.
        /// </exception>
        /// <returns>
        ///     An object containing information about the member specified in
        ///     <paramref name="memberCall"/>.
        /// </returns>
        public static MemberInfo GetMockable<T, TResult>(
            this Expression<Func<T, TResult>> memberCall)
        {
            return ParseMockable(memberCall);
        }

        /// <summary>
        ///     Parses the given lambda expression looking for a member
        ///     identifier that can be mocked.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="memberCall"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Thrown if <paramref name="memberCall"/> does not identify a
        ///     valid member.
        /// </exception>
        /// <param name="memberCall">
        ///     The expression that supposedly identifies a member.
        /// </param>
        /// <returns>
        ///     An object containing metadata about the member.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     For our purposes, a "mockable" member is a member that offers
        ///     behavior that is useful to modify, i.e.: a method, property, or
        ///     indexer. This should cover most members you'll want to mock on
        ///     a regular basis.
        ///     </para>
        ///     <para>
        ///     This method is not exhaustive in verifying what is mockable. For
        ///     example, passing a field as the member to modify will work, but
        ///     that's obviously not mockable.
        ///     </para>
        ///     <para>
        ///     There might also be cases where we *could* be more liberal in
        ///     what we take, such as a nested lambda or some unary operators
        ///     (we take cast simply because it looks like the most common of
        ///     these edge cases, e.g., explicit interface implementations).
        ///     </para>
        ///     <para>
        ///     There's no shortage of cases we could handle, but this is just
        ///     intended to cover the basics for now.
        ///     </para>
        ///     <para>
        ///     This is based on a post by Marc Gravell at
        ///     <see href="http://groups.google.com/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/f44dc57a9dc5168d/4805324df6b30218?pli=1"/>.
        ///     </para>
        /// </remarks>
        private static MemberInfo ParseMockable(LambdaExpression memberCall)
        {
            // The info object to return.
            MemberInfo info;

            if (memberCall == null)
            {
                throw new ArgumentNullException(
                    nameof(memberCall), Resources.NoMemberExpression);
            }

            // Get the actual content of the expression (ignoring casts).
            var body =
                memberCall.Body.NodeType == ExpressionType.Convert
                ? ((UnaryExpression)memberCall.Body).Operand
                : memberCall.Body;

            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    {
                        info = ((MemberExpression)body).Member;
                        break;
                    }

                case ExpressionType.Call:
                    {
                        info = ((MethodCallExpression)body).Method;
                        break;
                    }

                default:
                    {
                        throw new NotSupportedException(
                            string.Format(
                                Resources.InvalidMemberExpression, body.NodeType));
                    }
            }

            return info;
        }
    }
}
