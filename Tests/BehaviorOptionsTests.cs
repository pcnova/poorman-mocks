// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using PoorMan.Mocks.Tests.Tools;

    /// <summary>
    ///     Contains tests for the <see cref="BehaviorOptions"/> class.
    /// </summary>
    [TestClass]
    public class BehaviorOptionsTests
    {
        /// <summary>
        ///     An empty <see cref="Action"/> delegate.
        /// </summary>
        private static readonly Action NoOp = new Action(() => { });

        /// <summary>
        ///     Ensures that the constructor checks the behavior parameter and
        ///     throws a descriptive exception when it's invalid.
        /// </summary>
        [TestMethod]
        public void ConstructorThrowsWhenBehaviorNotADelegate()
        {
            Assert.That.Throws<ArgumentException>(
                () => new BehaviorOptionsStub<string>("Invalid", "Test"),
                "must be a delegate");
        }

        /// <summary>
        ///     Ensures that the constructor checks the member name parameter and
        ///     throws a descriptive exception when it's invalid.
        /// </summary>
        [TestMethod]
        public void ConstructorThrowsWhenMemberNameNull()
        {
            Assert.That.Throws<ArgumentException>(
                () => new BehaviorOptionsStub<Action>(NoOp, null),
                "must be provided");
        }

        /// <summary>
        ///     Ensures that the constructor checks the member name parameter and
        ///     throws a descriptive exception when it's invalid.
        /// </summary>
        [TestMethod]
        public void ConstructorThrowsWhenMemberNameEmpty()
        {
            Assert.That.Throws<ArgumentException>(
                () => new BehaviorOptionsStub<Action>(NoOp, string.Empty),
                "must not be empty");
        }

        /// <summary>
        ///     Ensures that the constructor checks the member name parameter and
        ///     throws a descriptive exception when it's invalid.
        /// </summary>
        [TestMethod]
        public void ConstructorThrowsWithParamNameWhenMemberNameInvalid()
        {
            // The constructor to test.
            Action ctor = () => new BehaviorOptionsStub<Action>(NoOp, null);

            Assert.AreEqual(
                "fullMemberName",
                Assert.That.Throws<ArgumentException>(ctor).ParamName);
        }
    }
}
