// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using PoorMan.Mocks.Tests.Fakes;
    using PoorMan.Mocks.Tests.Tools;

    /// <summary>
    ///     Contains tests for the <see cref="Mock"/> type.
    /// </summary>
    [TestClass]
    public class MockTests
    {
        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs the default behavior
        ///     when no custom behavior is added to a method.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsDefaultBehaviorWhenNoCustomBehaviorAdded()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            mock.ShowMessage();

            Assert.AreEqual(1, defaultCalls);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     in addition to the default behavior for a member.
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
            mock.AddBehavior(() => mock.ShowMessage(), () => calls++);

            mock.ShowMessage();

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.AreEqual(1, calls, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a property.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorForProperty()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(() => mock.DefaultMessage, () => calls++);

            Console.WriteLine(mock.DefaultMessage);

            Assert.AreEqual(1, calls, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to an indexer.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorForIndexer()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(() => mock[default], () => calls++);

            Console.WriteLine(mock[2]);

            Assert.AreEqual(1, calls, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that, by default, a <see cref="Mock"/> object runs custom
        ///     behavior before running the default behavior for a member.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorBeforeDefaultBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to track default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            bool? actionRanBeforeDefault = null;
            mock.AddBehavior(
                () => mock.ShowMessage(),
                () => actionRanBeforeDefault = defaultCalls == 0);

            mock.ShowMessage();

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.AreEqual(
                true,
                actionRanBeforeDefault,
                "Custom behavior did not run before default.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     after running a member's default behavior, if so specified.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorAfterDefaultBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to track default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            bool? actionRanAfterDefault = null;
            mock.AddBehavior(
                () => mock.ShowMessage(),
                () => actionRanAfterDefault = defaultCalls > 0)
                .With(o => o.RunAfter = true);

            mock.ShowMessage();

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.AreEqual(
                true, actionRanAfterDefault, "Custom behavior did not run after.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> allows added behavior to
        ///     throw exceptions, letting them bubble up normally.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockDoesNotHandleAddedBehaviorException()
        {
            // Set up.
            var mock = new MockStub();
            var error = new Exception("Look Ma, I'm roadkill!");

            // Add custom behavior that fails.
            mock.AddBehavior(() => mock.ShowMessage(), () => throw error);

            Assert.AreSame(
                error,
                Assert.That.Throws<Exception>(() => mock.ShowMessage()));
        }

        /// <summary>
        ///     Ensures that by default, the <see cref="Mock"/> keeps executing
        ///     custom behavior any time the modified member is called.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockKeepsRunningAddedBehaviorByDefault()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => calls++);

            mock.ShowMessage();
            mock.ShowMessage();
            mock.ShowMessage();

            Assert.AreEqual(3, calls);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> executes custom behavior
        ///     only once, if so specified.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorOnce()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => calls++)
                .With(o => o.RunOnce = true);

            mock.ShowMessage();
            mock.ShowMessage();
            mock.ShowMessage();

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs an added behavior
        ///     for the correct member overload.
        /// </summary>
        /// <remarks>
        ///     This is basically ensuring that the full name of the member is
        ///     used by the mock to store and lookup custom behavior, so that
        ///     overloaded members are treated separately.
        /// </remarks>
        [TestMethod]
        [TestCategory("Add")]
        public void MockRunsAddedBehaviorForCorrectOverload()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to two overloads, to see if they execute
            // separately.
            int defaultCalls = 0, overloadCalls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => defaultCalls++);
            mock.AddBehavior(() => mock.ShowMessage(default), () => overloadCalls++);

            mock.ShowMessage();
            mock.ShowMessage(1);

            Assert.AreEqual(
                1, defaultCalls, "Expected behavior for default method to run once.");
            Assert.AreEqual(
                1, overloadCalls, "Expected behavior for overload to run once.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> allows adding behavior more
        ///     than once for the same member, running the behavior that was
        ///     added last in that case.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockAllowsOverridingAddedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior twice (this should not throw).
            int firstBehaviorCalls = 0, secondBehaviorCalls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => firstBehaviorCalls++);
            mock.AddBehavior(() => mock.ShowMessage(), () => secondBehaviorCalls++);

            mock.ShowMessage();

            Assert.AreEqual(
                0, firstBehaviorCalls, "Did not expect first behavior to run.");
            Assert.AreEqual(
                1, secondBehaviorCalls, "Expected second behavior to run once.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> throws a descriptive
        ///     exception when the member-identifying expression is <c>null</c>.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockThrowsWhenAddingBehaviorWithNullIdentifier()
        {
            // Set up.
            var mock = new MockStub();
            Action add = () => mock.AddBehavior(null, () => { });

            var error =
                Assert.That.Throws<ArgumentNullException>(
                    add, "expression must be provided");
            Assert.AreEqual("memberCall", error.ParamName);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> throws a descriptive
        ///     exception when the given expression does not identify a valid
        ///     member to modify.
        /// </summary>
        /// <param name="badIdentifier">
        ///     An expression that does *not* identify a member to modify.
        /// </param>
        /// <param name="expectedType">
        ///     The value indicating the type of expression
        ///     <paramref name="badIdentifier"/> is.
        /// </param>
        [TestMethod]
        [TestCategory("Add")]
        [DynamicData("GetInvalidIdentifyingExpressions", DynamicDataSourceType.Method)]
        public void MockThrowsWhenAddingBehaviorForNonMember(
            Expression<Func<object>> badIdentifier,
            ExpressionType expectedType)
        {
            // Set up.
            var mock = new MockStub();

            Assert.That.Throws<NotSupportedException>(
                () => mock.AddBehavior(badIdentifier, () => { }),
                expectedType,
                "identify a mockable member");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> does not throw an exception
        ///     when a given expression identifies a valid member but it's
        ///     preppended with a cast operator.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        public void MockDoesNotThrowWhenAddingBehaviorWithCast()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.AddBehavior(() => (object)mock.GetMessage(), () => calls++);

            mock.GetMessage();

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a member, when an argument is passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockRunsAddedBehaviorWithArg()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<ushort>();
            mock.AddBehavior(
                () => mock.ShowMessage(default),
                (ushort id) => calls.Add(id));

            mock.ShowMessage(testId);

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.That.SingleCall(calls, testId);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a member, when two arguments are passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockRunsAddedBehaviorWithTwoArgs()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();
            var customText = " Ha!";

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<IList>();
            mock.AddBehavior(
                () => mock.ShowMessage(default, null, null, null),
                (ushort id, string txt) => calls.Add(new ArrayList { id, txt }));

            mock.ShowMessage(testId, customText, null, null);

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.That.SingleCall(calls, testId, customText);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a member, when three arguments are passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockRunsAddedBehaviorWithThreeArgs()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();
            string customText = " Ha!", moreText = " Haa!!";

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<IList>();
            mock.AddBehavior(
                () => mock.ShowMessage(default, null, null, null),
                (ushort id, string txt, string txt2) => calls.Add(
                    new ArrayList { id, txt, txt2 }));

            mock.ShowMessage(testId, customText, moreText, null);

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.That.SingleCall(calls, testId, customText, moreText);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     added to a member, when four arguments are passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockRunsAddedBehaviorWithFourArgs()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();
            string customText = " Ha!", moreText = " Haa!!", andMore = " Haaa!!!";

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<IList>();
            mock.AddBehavior(
                () => mock.ShowMessage(default, null, null, null),
                (ushort id, string txt, string txt2, string txt3) => calls.Add(
                    new ArrayList { id, txt, txt2, txt3 }));

            mock.ShowMessage(testId, customText, moreText, andMore);

            Assert.AreEqual(1, defaultCalls, "Expected default behavior to run.");
            Assert.That.SingleCall(calls, testId, customText, moreText, andMore);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the number of
        ///     arguments passed on the call to <see cref="Mock.
        ///     RunCustomBehaviorOr(Action, Expression{Action}, object[])"/>,
        ///     throwing a descriptive message otherwise.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockValidatesArgCountForAddedBehaviorAtRunSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to a method we know does not pass the right
            // number of arguments on the RunCustomBehaviorOr() call.
            mock.AddBehavior(
                () => mock.GetMessageWithInvalidArgCountPassThru(default),
                (ushort id) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessageWithInvalidArgCountPassThru(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessageWithInvalidArgCountPassThru),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/>'s argument validation allows
        ///     the call to <see cref="Mock.
        ///     RunCustomBehaviorOr(Action, Expression{Action}, object[])"/> to
        ///     pass more arguments than required (since they're harmless).
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockAllowsMoreArgsForAddedBehaviorAtRunSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to a method we know passes more args than
            // necessary to the RunCustomBehaviorOr() call. This should be OK.
            var calls = 0;
            mock.AddBehavior(
                () => mock.GetMessageWithExcessArgsPassedThru(default),
                (ushort id) => calls++);

            mock.GetMessageWithExcessArgsPassedThru(0);

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the type of
        ///     arguments passed on the call to <see cref="Mock.
        ///     RunCustomBehaviorOr(Action, Expression{Action}, object[])"/>,
        ///     throwing a descriptive message otherwise.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockValidatesArgTypeForAddedBehaviorAtRunSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to a method that does not pass the right type
            // of arguments on the RunCustomBehaviorOr() call.
            mock.AddBehavior(
                () => mock.GetMessageWithInvalidArgPassThru(default),
                (ushort id) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessageWithInvalidArgPassThru(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessageWithInvalidArgPassThru),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the count of
        ///     arguments expected by added behavior, throwing a descriptive
        ///     message if invalid.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockValidatesArgCountForAddedBehaviorAtConfigSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior that expects an additional string arg, which
            // the actual method does not have.
            mock.AddBehavior(
                () => mock.GetMessage(default),
                (ushort id, string invalidArg) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessage),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/>'s argument validation allows
        ///     custom behavior to expect less arguments than the actual count
        ///     required by the member (since they may not all be needed).
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockAllowsLessArgsForAddedBehaviorAtConfigSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior that expects no args, even though the actual
            // method requires one. This should be OK.
            var calls = 0;
            mock.AddBehavior(() => mock.GetMessage(default), () => calls++);

            mock.GetMessage(1);

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the type of
        ///     arguments expected by added behavior, throwing a descriptive
        ///     message if invalid.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockValidatesArgTypeForAddedBehaviorAtConfigSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior that expects a single string arg, which the
            // actual method does not have.
            mock.AddBehavior(
                () => mock.GetMessage(default),
                (string invalidArg) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessage),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that when an added behavior is a private, non-anonymous
        ///     method, and argument validation fails, the <see cref="Mock"/>
        ///     finds and points out the class where the behavior resides.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockFindsSiteForMisconfiguredPrivateAddedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add an invalid custom behavior.
            mock.AddBehavior(
                () => mock.GetMessage(default),
                new Action<string>(InvalidBehavior));

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that when an added behavior is a public method and
        ///     argument validation fails, the <see cref="Mock"/> finds and
        ///     points out the class where the behavior is defined.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockFindsSiteForMisconfiguredPublicAddedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add an invalid custom behavior.
            mock.AddBehavior(
                () => mock.GetMessage(default),
                new Action<bool>(Assert.IsTrue));

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "look for usages of",
                typeof(Assert) + "." + nameof(Assert.IsTrue));
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> handles exceptions thrown by
        ///     added behavior that takes arguments in order to throw the original
        ///     exception, since they're wrapped in another exception due to the
        ///     way they're called.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockThrowsOriginalExceptionFromAddedBehaviorWithArgs()
        {
            // Set up.
            var mock = new MockStub();
            var error = new Exception("Look Ma, I'm roadkill!");

            // Add custom behavior that fails.
            mock.AddBehavior(
                () => mock.ShowMessage(0), (ushort id) => throw error);

            Assert.AreSame(
                error,
                Assert.That.Throws<Exception>(() => mock.ShowMessage(0)));
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs an added behavior
        ///     for the correct member overload.
        /// </summary>
        /// <remarks>
        ///     This is basically ensuring that the full name of the member is
        ///     used by the mock to store and lookup custom behavior, so that
        ///     overloaded members are treated separately.
        /// </remarks>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockRunsAddedBehaviorWithArgsForCorrectOverload()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to two overloads, to see if they execute
            // separately.
            int defaultCalls = 0, overloadCalls = 0;
            mock.AddBehavior(
                () => mock.ShowMessage(), () => defaultCalls++);
            mock.AddBehavior(
                () => mock.ShowMessage(default), (ushort id) => overloadCalls++);

            mock.ShowMessage();
            mock.ShowMessage(1);

            Assert.AreEqual(
                1, defaultCalls, "Expected behavior for default method to run once.");
            Assert.AreEqual(
                1, overloadCalls, "Expected behavior for overload to run once.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> allows adding behavior more
        ///     than once for the same member, running the behavior that was
        ///     added last in that case.
        /// </summary>
        [TestMethod]
        [TestCategory("Add With Args")]
        public void MockAllowsOverridingAddedBehaviorWithArgs()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior twice (this should not throw).
            int firstBehaviorCalls = 0, secondBehaviorCalls = 0;
            mock.AddBehavior(
                () => mock.ShowMessage(0), () => firstBehaviorCalls++);
            mock.AddBehavior(
                () => mock.ShowMessage(0), (ushort id) => secondBehaviorCalls++);

            mock.ShowMessage(1);

            Assert.AreEqual(
                0, firstBehaviorCalls, "Did not expect first behavior to run.");
            Assert.AreEqual(
                1, secondBehaviorCalls, "Expected second behavior to run once.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     that replaced an existing method.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to know if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => calls++);

            mock.ShowMessage();

            Assert.AreEqual(
                0, defaultCalls, "Did not expect default behavior to run.");
            Assert.AreEqual(1, calls, "Expected the custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a property.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorForProperty()
        {
            // Set up.
            var mock = new MockStub();
            const string Question = "Do I feel lucky?";

            // Add custom behavior to see if it executes.
            mock.SetBehavior(() => mock.DefaultMessage, () => Question);

            Assert.AreEqual(
                Question, mock.DefaultMessage, "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into an indexer.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorForIndexer()
        {
            // Set up.
            var mock = new MockStub();
            const string Question = "Well do ya?";

            // Add custom behavior to see if it executes.
            mock.SetBehavior(() => mock[default], () => Question);

            Assert.AreEqual(Question, mock[0], "Expected custom behavior to run.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object returns whatever value
        ///     is returned by custom behavior.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorAndReturnsValue()
        {
            // Set up.
            const string CustomMsg = @"Bring around... the ""loaner"".";
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            mock.SetBehavior(() => mock.GetMessage(), () => CustomMsg);

            Assert.AreEqual(CustomMsg, mock.GetMessage());
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object returns whatever value
        ///     is returned by custom behavior, even if it's <c>null</c>.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorAndReturnsNullValue()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            mock.SetBehavior(() => mock.GetMessage(), () => null);

            Assert.IsNull(mock.GetMessage());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> allows replaced behavior to
        ///     throw exceptions, letting them bubble up normally.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockDoesNotHandleReplacedBehaviorException()
        {
            // Set up.
            var mock = new MockStub();
            var error = new Exception("Look Ma, I'm roadkill!");

            // Add custom behavior that fails.
            mock.SetBehavior(() => mock.ShowMessage(), () => throw error);

            Assert.AreSame(
                error,
                Assert.That.Throws<Exception>(() => mock.ShowMessage()));
        }

        /// <summary>
        ///     Ensures that by default, the <see cref="Mock"/> keeps executing
        ///     custom behavior any time the modified member is called.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockKeepsRunningReplacedBehaviorByDefault()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => calls++);

            mock.ShowMessage();
            mock.ShowMessage();
            mock.ShowMessage();

            Assert.AreEqual(3, calls);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> executes custom behavior
        ///     only once, if so specified.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorOnce()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to see if it executes.
            var calls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => calls++)
                .With(o => o.RunOnce = true);

            mock.ShowMessage();
            mock.ShowMessage();
            mock.ShowMessage();

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs a replaced behavior
        ///     for the correct member overload.
        /// </summary>
        /// <remarks>
        ///     This is basically ensuring that the full name of the member is
        ///     used by the mock to store and lookup custom behavior, so that
        ///     overloaded members are treated separately.
        /// </remarks>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockRunsReplacedBehaviorForCorrectOverload()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to two overloads, to see if they execute
            // separately.
            int defaultCalls = 0, overloadCalls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => defaultCalls++);
            mock.SetBehavior(() => mock.ShowMessage(default), () => overloadCalls++);

            mock.ShowMessage();
            mock.ShowMessage(1);

            Assert.AreEqual(
                1, defaultCalls, "Expected behavior for default method to run once.");
            Assert.AreEqual(
                1, overloadCalls, "Expected behavior for overload to run once.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> allows replacing behavior
        ///     more than once for the same member, running the behavior that
        ///     was set last in that case.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockAllowsOverridingReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior twice (this should not throw).
            int firstBehaviorCalls = 0, secondBehaviorCalls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => firstBehaviorCalls++);
            mock.SetBehavior(() => mock.ShowMessage(), () => secondBehaviorCalls++);

            mock.ShowMessage();

            Assert.AreEqual(
                0, firstBehaviorCalls, "Did not expect first behavior to run.");
            Assert.AreEqual(
                1, secondBehaviorCalls, "Expected second behavior to run once.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> throws a descriptive
        ///     exception when the member-identifying expression is <c>null</c>.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockThrowsWhenReplacingBehaviorWithNullIdentifier()
        {
            // Set up.
            var mock = new MockStub();
            Action set = () => mock.SetBehavior(null, () => { });

            var error =
                Assert.That.Throws<ArgumentNullException>(
                    set, "expression must be provided");
            Assert.AreEqual("memberCall", error.ParamName);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> throws a descriptive
        ///     exception when the given expression does not identify a valid
        ///     member to modify.
        /// </summary>
        /// <param name="badIdentifier">
        ///     An expression that does *not* identify a member to modify.
        /// </param>
        /// <param name="expectedType">
        ///     The value indicating the type of expression
        ///     <paramref name="badIdentifier"/> is.
        /// </param>
        [TestMethod]
        [TestCategory("Replace")]
        [DynamicData("GetInvalidIdentifyingExpressions", DynamicDataSourceType.Method)]
        public void MockThrowsWhenReplacingBehaviorForNonMember(
            Expression<Func<object>> badIdentifier,
            ExpressionType expectedType)
        {
            // Set up.
            var mock = new MockStub();

            Assert.That.Throws<NotSupportedException>(
                () => mock.SetBehavior(badIdentifier, () => "?"),
                expectedType,
                "identify a mockable member");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> does not throw an exception
        ///     when a given expression identifies a valid member but it's
        ///     preppended with a cast operator.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace")]
        public void MockDoesNotThrowWhenReplacingBehaviorWithCast()
        {
            // Set up.
            var mock = new MockStub();
            const string Question = "Do I feel lucky?";

            // Add custom behavior to see if it executes.
            mock.SetBehavior(() => (object)mock.GetMessage(), () => Question);

            Assert.AreEqual(Question, mock.GetMessage());
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a member, when an argument is passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockRunsReplacedBehaviorWithArg()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<ushort>();
            mock.SetBehavior(
                () => mock.ShowMessage(default),
                (ushort id) => calls.Add(id));

            mock.ShowMessage(testId);

            Assert.AreEqual(0, defaultCalls, "Did not expect default behavior.");
            Assert.That.SingleCall(calls, testId);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a member, when two arguments are passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockRunsReplacedBehaviorWithTwoArgs()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();
            var customText = " Ha!";

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<IList>();
            mock.SetBehavior(
                () => mock.ShowMessage(default, null, null, null),
                (ushort id, string txt) => calls.Add(new ArrayList { id, txt }));

            mock.ShowMessage(testId, customText, null, null);

            Assert.AreEqual(0, defaultCalls, "Did not expect default behavior.");
            Assert.That.SingleCall(calls, testId, customText);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a member, when three arguments are passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockRunsReplacedBehaviorWithThreeArgs()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();
            string customText = " Ha!", moreText = " Haa!!";

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<IList>();
            mock.SetBehavior(
                () => mock.ShowMessage(default, null, null, null),
                (ushort id, string txt, string txt2) => calls.Add(
                    new ArrayList { id, txt, txt2 }));

            mock.ShowMessage(testId, customText, moreText, null);

            Assert.AreEqual(0, defaultCalls, "Did not expect default behavior.");
            Assert.That.SingleCall(calls, testId, customText, moreText);
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs custom behavior
        ///     set into a member, when four arguments are passed.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockRunsReplacedBehaviorWithFourArgs()
        {
            // Set up.
            var mock = new MockStub();
            var testId = (ushort)this.GetHashCode();
            string customText = " Ha!", moreText = " Haa!!", andMore = " Haaa!!!";

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessageWithId += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var calls = new List<IList>();
            mock.SetBehavior(
                () => mock.ShowMessage(default, null, null, null),
                (ushort id, string txt, string txt2, string txt3) => calls.Add(
                    new ArrayList { id, txt, txt2, txt3 }));

            mock.ShowMessage(testId, customText, moreText, andMore);

            Assert.AreEqual(0, defaultCalls, "Did not expect default behavior.");
            Assert.That.SingleCall(calls, testId, customText, moreText, andMore);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the number of
        ///     arguments passed on the call to <see cref="Mock.
        ///     RunCustomBehaviorOr(Action, Expression{Action}, object[])"/>,
        ///     throwing a descriptive message otherwise.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockValidatesArgCountForReplacedBehaviorAtRunSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to a method we know does not pass the right
            // number of arguments on the RunCustomBehaviorOr() call.
            mock.SetBehavior(
                () => mock.GetMessageWithInvalidArgCountPassThru(default),
                (ushort id) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessageWithInvalidArgCountPassThru(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessageWithInvalidArgCountPassThru),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/>'s argument validation allows
        ///     the call to <see cref="Mock.
        ///     RunCustomBehaviorOr(Action, Expression{Action}, object[])"/> to
        ///     pass more arguments than required (since they're harmless).
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockAllowsMoreArgsForReplacedBehaviorAtRunSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to a method we know passes more args than
            // necessary to the RunCustomBehaviorOr() call. This should be OK.
            var calls = 0;
            mock.SetBehavior(
                () => mock.GetMessageWithExcessArgsPassedThru(default),
                (ushort id) => calls++);

            mock.GetMessageWithExcessArgsPassedThru(0);

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the type of
        ///     arguments passed on the call to <see cref="Mock.
        ///     RunCustomBehaviorOr(Action, Expression{Action}, object[])"/>,
        ///     throwing a descriptive message otherwise.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockValidatesArgTypeForReplacedBehaviorAtRunSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to a method that does not pass the right type
            // of arguments on the RunCustomBehaviorOr() call.
            mock.SetBehavior(
                () => mock.GetMessageWithInvalidArgPassThru(default),
                (ushort id) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessageWithInvalidArgPassThru(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessageWithInvalidArgPassThru),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the count of
        ///     arguments expected by replaced behavior, throwing a descriptive
        ///     message if invalid.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockValidatesArgCountForReplacedBehaviorAtConfigSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior that expects an additional string arg, which
            // the actual method does not have.
            mock.SetBehavior(
                () => mock.GetMessage(default),
                (ushort id, string invalidArg) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessage),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/>'s argument validation allows
        ///     custom behavior to expect less arguments than the actual count
        ///     required by the member (since they may not all be needed).
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockAllowsLessArgsForReplacedBehaviorAtConfigSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior that expects no args, even though the actual
            // method requires one. This should be OK.
            var calls = 0;
            mock.SetBehavior(() => mock.GetMessage(default), () => calls++);

            mock.GetMessage(1);

            Assert.AreEqual(1, calls);
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> validates the type of
        ///     arguments expected by replaced behavior, throwing a descriptive
        ///     message if invalid.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockValidatesArgTypeForReplacedBehaviorAtConfigSite()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior that expects a single string arg, which the
            // actual method does not have.
            mock.SetBehavior(
                () => mock.GetMessage(default),
                (string invalidArg) => { });

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "wrong number or type",
                nameof(mock.GetMessage),
                nameof(MockStub),
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that when a replaced behavior is a private, non-anonymous
        ///     method, and argument validation fails, the <see cref="Mock"/>
        ///     finds and points out the class where the behavior resides.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockFindsSiteForMisconfiguredPrivateReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add an invalid custom behavior.
            mock.SetBehavior(
                () => mock.GetMessage(default),
                new Action<string>(InvalidBehavior));

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "call is in " + this.GetType());
        }

        /// <summary>
        ///     Ensures that when a replaced behavior is a public method and
        ///     argument validation fails, the <see cref="Mock"/> finds and
        ///     points out the class where the behavior is defined.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockFindsSiteForMisconfiguredPublicReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add an invalid custom behavior.
            mock.SetBehavior(
                () => mock.GetMessage(default),
                new Action<bool>(Assert.IsTrue));

            Assert.That.Throws<ArgumentException>(
                () => mock.GetMessage(0),
                "mock could not execute",
                "look for usages of",
                typeof(Assert) + "." + nameof(Assert.IsTrue));
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> handles exceptions thrown by
        ///     replaced behavior that takes arguments in order to throw the
        ///     original exception, since they're wrapped in another exception
        ///     due to the way they're called.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockThrowsOriginalExceptionFromReplacedBehaviorWithArgs()
        {
            // Set up.
            var mock = new MockStub();
            var error = new Exception("Look Ma, I'm roadkill!");

            // Add custom behavior that fails.
            mock.SetBehavior(
                () => mock.ShowMessage(0), (ushort id) => throw error);

            Assert.AreSame(
                error,
                Assert.That.Throws<Exception>(() => mock.ShowMessage(0)));
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs replaced behavior
        ///     for the correct member overload.
        /// </summary>
        /// <remarks>
        ///     This is basically ensuring that the full name of the member is
        ///     used by the mock to store and lookup custom behavior, so that
        ///     overloaded members are treated separately.
        /// </remarks>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockRunsReplacedBehaviorWithArgsForCorrectOverload()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior to two overloads, to see if they execute
            // separately.
            int defaultCalls = 0, overloadCalls = 0;
            mock.SetBehavior(
                () => mock.ShowMessage(), () => defaultCalls++);
            mock.SetBehavior(
                () => mock.ShowMessage(default), (ushort id) => overloadCalls++);

            mock.ShowMessage();
            mock.ShowMessage(1);

            Assert.AreEqual(
                1, defaultCalls, "Expected behavior for default method to run once.");
            Assert.AreEqual(
                1, overloadCalls, "Expected behavior for overload to run once.");
        }

        /// <summary>
        ///     Ensures that the <see cref="Mock"/> allows replacing behavior
        ///     more than once for the same member, running the behavior that
        ///     was added last in that case.
        /// </summary>
        [TestMethod]
        [TestCategory("Replace With Args")]
        public void MockAllowsOverridingReplacedBehaviorWithArgs()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior twice (this should not throw).
            int firstBehaviorCalls = 0, secondBehaviorCalls = 0;
            mock.SetBehavior(
                () => mock.ShowMessage(0), () => firstBehaviorCalls++);
            mock.SetBehavior(
                () => mock.ShowMessage(0), (ushort id) => secondBehaviorCalls++);

            mock.ShowMessage(1);

            Assert.AreEqual(
                0, firstBehaviorCalls, "Did not expect first behavior to run.");
            Assert.AreEqual(
                1, secondBehaviorCalls, "Expected second behavior to run once.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs both custom
        ///     behavior that replaces a member's default behavior, and custom
        ///     behavior that augments default behavior.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        [TestCategory("Replace")]
        public void MockRunsAddedAndReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Add custom behavior to see if it executes.
            var addCalls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => addCalls++);

            // Set replacement behavior to see if it executes.
            var replaceCalls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => replaceCalls++);

            mock.ShowMessage();

            Assert.AreEqual(0, defaultCalls, "Did not expect default behavior.");
            Assert.AreEqual(1, addCalls, "Expected added behavior.");
            Assert.AreEqual(1, replaceCalls, "Expected replacement behavior.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs both replacement
        ///     and additive behavior, and that this happens regardless of the
        ///     order in which behavior is set.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        [TestCategory("Replace")]
        public void MockRunsAddedAndReplacedBehaviorSetInReverseOrder()
        {
            // Set up.
            var mock = new MockStub();

            // Hook this up to see if the default behavior runs.
            var defaultCalls = 0;
            mock.ShowingMessage += delegate { defaultCalls++; };

            // Set replacement behavior *first*, to see if it executes.
            var replaceCalls = 0;
            mock.SetBehavior(() => mock.ShowMessage(), () => replaceCalls++);

            // Add custom behavior to see if it executes.
            var addCalls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => addCalls++);

            mock.ShowMessage();

            Assert.AreEqual(0, defaultCalls, "Did not expect default behavior.");
            Assert.AreEqual(1, addCalls, "Expected added behavior.");
            Assert.AreEqual(1, replaceCalls, "Expected replacement behavior.");
        }

        /// <summary>
        ///     Ensures that a <see cref="Mock"/> object runs both replacement
        ///     and additive behavior, but that the latter can be run after the
        ///     former, when so specified.
        /// </summary>
        [TestMethod]
        [TestCategory("Add")]
        [TestCategory("Replace")]
        public void MockRunsAddedBehaviorAfterReplacedBehavior()
        {
            // Set up.
            var mock = new MockStub();

            // Add custom behavior specifying execution after main behavior.
            var addCalls = 0;
            mock.AddBehavior(() => mock.ShowMessage(), () => addCalls++)
                .With(o => o.RunAfter = true);

            // Set replacement behavior to see if/when things execute.
            bool? addedBehaviorAfter = null;
            mock.SetBehavior(
                () => mock.ShowMessage(),
                () => addedBehaviorAfter = addCalls == 0);

            mock.ShowMessage();

            Assert.AreEqual(1, addCalls, "Expected added behavior.");
            Assert.AreEqual(
                true, addedBehaviorAfter, "Expected added behavior to run after.");
        }

        /// <summary>
        ///     Gets a series of expressions that are invalid for the purposes
        ///     of identifying a member to mock.
        /// </summary>
        /// <returns>
        ///     An enumerable of arrays where each array has an expression and
        ///     a corresponding <see cref="ExpressionType"/>.
        /// </returns>
        /// <remarks>
        ///     This is intended to be used as the source of data for tests
        ///     having the <see cref="DynamicDataAttribute"/> set.
        /// </remarks>
        private static IEnumerable<object[]> GetInvalidIdentifyingExpressions()
        {
            yield return new object[]
            {
                (Expression<Func<object>>)(() => "?"),
                ExpressionType.Constant,
            };
            yield return new object[]
            {
                (Expression<Func<object>>)(() => new object()),
                ExpressionType.New,
            };
            yield return new object[]
            {
                (Expression<Func<object>>)(
                    () => new object().GetHashCode() == 0 ? new object() : null),
                ExpressionType.Conditional,
            };
            yield return new object[]
            {
                (Expression<Func<object>>)(
                    () => new Func<object>(() => new object())),
                ExpressionType.Lambda,
            };
        }

        /// <summary>
        ///     A method used as an invalid custom behavior for
        ///     <see cref="MockStub"/> methods (because the argument does not
        ///     match).
        /// </summary>
        /// <param name="arg">
        ///     A dummy argument.
        /// </param>
        private static void InvalidBehavior(string arg)
        {
        }
    }
}
