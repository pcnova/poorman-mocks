// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Tests.Tools
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     Contains methods that extend the <see cref="Assert"/> class for
    ///     easier testing.
    /// </summary>
    internal static class AssertExtensions
    {
        /// <summary>
        ///     Asserts that the specified member call throws an exception,
        ///     optionally verifying the exception message as well.
        /// </summary>
        /// <typeparam name="TEx">
        ///     The type of exception expected to be thrown.
        /// </typeparam>
        /// <param name="assert">
        ///     The assertion object to work against. This is simply the mechanism
        ///     through which we extend assertions - not really used.
        /// </param>
        /// <param name="memberCall">
        ///     The expression that calls some member that is expected to throw.
        /// </param>
        /// <param name="messageSnippets">
        ///     Zero or more snippets of text that are expected to be included
        ///     in the exception message.
        /// </param>
        /// <returns>
        ///     The exception that was thrown.
        /// </returns>
        public static TEx Throws<TEx>(
            this Assert assert, Action memberCall, params object[] messageSnippets)
            where TEx : Exception
        {
            // Ensure an exception is thrown and keep it for further assertions.
            var error = Assert.ThrowsException<TEx>(memberCall);

            foreach (var snip in messageSnippets.Select(snp => snp + string.Empty))
            {
                // Shorten the snippet for our assert failed message.
                var shortSnip =
                    snip.Length > 50 ? snip.Remove(50) + "[...]" : snip;

                Assert.IsTrue(
                    error.Message.Contains(snip),
                    $"Expected the following text in the message:\n\t\"{shortSnip}\"");
            }

            return error;
        }

        /// <summary>
        ///     Asserts that the given list contains a single entry representing
        ///     a member call and its one argument, and that the argument matches
        ///     an expected value.
        /// </summary>
        /// <param name="assert">
        ///     The assertion object to work against. This is simply the mechanism
        ///     through which we extend assertions - not really used.
        /// </param>
        /// <param name="actualCalls">
        ///     A set of generic objects. Each entry in the list is assumed to be
        ///     the result of calling a mock's custom behavior, and the entry's
        ///     item is the argument passed to the call.
        /// </param>
        /// <param name="expectedArg">
        ///     A value that should match the first <paramref name="actualCalls"/>
        ///     item, i.e., this is the expected argument for the call.
        /// </param>
        public static void SingleCall(
            this Assert assert,
            IEnumerable actualCalls,
            object expectedArg)
        {
            assert.SingleCall(
                actualCalls.OfType<object>().Select(obj => new[] { obj }).ToArray(),
                expectedArg);
        }

        /// <summary>
        ///     Asserts that the given list contains a single entry representing
        ///     a member call and its arguments, and that the arguments match
        ///     expected values.
        /// </summary>
        /// <param name="assert">
        ///     The assertion object to work against. This is simply the mechanism
        ///     through which we extend assertions - not really used.
        /// </param>
        /// <param name="actualCalls">
        ///     A list containing lists of generic objects. Each inner list is
        ///     assumed to be the result of calling a mock's custom behavior, and
        ///     the items in the list are the arguments passed to the call.
        /// </param>
        /// <param name="expectedArgs">
        ///     Zero or more values that should match the objects in the first
        ///     <paramref name="actualCalls"/> list, i.e., these are the expected
        ///     arguments for the call.
        /// </param>
        public static void SingleCall(
            this Assert assert,
            IReadOnlyList<IList> actualCalls,
            params object[] expectedArgs)
        {
            Assert.AreEqual(
                1, actualCalls.Count, "Expected custom behavior to run once.");

            Assert.AreEqual(
                expectedArgs.Length,
                actualCalls[0].Count,
                "Unexpected number of arguments passed to custom behavior.");

            for (int i = 0; i < expectedArgs.Length; i++)
            {
                Assert.AreEqual(
                    expectedArgs[i],
                    actualCalls[0][i],
                    $"Unexpected value passed to custom behavior for arg {i + 1}.");
            }
        }
    }
}
