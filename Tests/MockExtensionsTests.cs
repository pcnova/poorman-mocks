// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Tests
{
    using System;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using PoorMan.Mocks.Tests.Fakes;
    using PoorMan.Mocks.Tests.Tools;

    /// <summary>
    ///     Contains tests for the <see cref="MockExtensions"/> class.
    /// </summary>
    [TestClass]
    public class MockExtensionsTests
    {
        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a member when using the extension that provides the
        ///     mock as a parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(m => m.ShowMessage(), () => calls++);

            mock.ShowMessage();

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.AreEqual(1, calls, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a member when using the extension that provides the
        ///     mock as a parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorReturningData()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(m => m.GetMessage(), () => calls++);

            // Note this is not the method we modified, but it calls the one we
            // did. This allows us to verify that the default behavior runs.
            mock.ShowMessage();

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.AreEqual(1, calls, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> throws a descriptive
        ///     exception when the given expression does not identify a valid
        ///     member to modify.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockThrowsWhenAddingBehaviorForNonMember()
        {
            // Set up.
            var mock = new MockStub();

            Assert.That.Throws<NotSupportedException>(
                () => mock.AddBehavior(m => "?", () => { }),
                ExpressionType.Constant,
                "identify a mockable member");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a member when using the extension that provides the
        ///     mock as a parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.SetBehavior(m => m.ShowMessage(), () => calls++);

            mock.ShowMessage();

            Assert.AreEqual(1, calls, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a member when using the extension that provides the
        ///     mock as a parameter.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorReturningData()
        {
            // Set up.
            var mock = new MockStub();
            const string Question = "Well do ya?";

            // Add custom behavior to see if it executes.
            mock.SetBehavior(m => m.GetMessage(), () => Question);

            Assert.AreEqual(
                Question, mock.GetMessage(), "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> throws a descriptive
        ///     exception when the given expression does not identify a valid
        ///     member to modify.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockThrowsWhenReplacingBehaviorForNonMember()
        {
            // Set up.
            var mock = new MockStub();

            Assert.That.Throws<NotSupportedException>(
                () => mock.SetBehavior(m => "?", () => "?"),
                ExpressionType.Constant,
                "identify a mockable member");
        }
    }
}
