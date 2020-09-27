// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Tests.Fakes
{
    using System;
    using System.Linq;

    /// <summary>
    ///     Overrides <see cref="Mock"/> for testing purposes.
    /// </summary>
    internal class MockStub : Mock
    {
        /// <summary>
        ///     The collection of messages the mock can return.
        /// </summary>
        private readonly string[] messages = new[]
        {
            "Sssmokin'!!!",
            "Sssomebody stop me!!!",
            "It's party time!",
        };

        /// <summary>
        ///     Occurs when <see cref="ShowMessage()"/> is called.
        /// </summary>
        public event EventHandler ShowingMessage = delegate { };

        /// <summary>
        ///     Occurs when <see cref="ShowMessage(ushort)"/> is called.
        /// </summary>
        public event EventHandler ShowingMessageWithId = delegate { };

        /// <summary>
        ///     Gets the default message the mock will return.
        /// </summary>
        public string DefaultMessage
        {
            get
            {
                return this.RunCustomBehaviorOr(
                    () => this[0], () => this.DefaultMessage);
            }
        }

        /// <summary>
        ///     Gets a message given an ID.
        /// </summary>
        /// <param name="id">
        ///     The ID of the message to return.
        /// </param>
        public string this[ushort id]
        {
            get
            {
                return this.RunCustomBehaviorOr(
                    () =>
                        id >= this.messages.Length
                        ? this.messages.Last()
                        : this.messages[id],
                    () => this[default]);
            }
        }

        /// <summary>
        ///     Shows a message.
        /// </summary>
        public void ShowMessage()
        {
            this.RunCustomBehaviorOr(
                () =>
                {
                    this.ShowingMessage(this, EventArgs.Empty);
                    Console.WriteLine(this.GetMessage());
                },
                () => this.ShowMessage());
        }

        /// <summary>
        ///     Shows a message.
        /// </summary>
        /// <param name="id">
        ///     The message to show: 0 for the default, any other number for a
        ///     new message.
        /// </param>
        public void ShowMessage(ushort id)
        {
            this.RunCustomBehaviorOr(
                () =>
                {
                    this.ShowingMessageWithId(this, EventArgs.Empty);
                    Console.WriteLine(this.GetMessage(id));
                },
                () => this.ShowMessage(0),
                id);
        }

        /// <summary>
        ///     Shows a message with additional custom text.
        /// </summary>
        /// <param name="id">
        ///     The message to show: 0 for the default, any other number for a
        ///     new message.
        /// </param>
        /// <param name="second">
        ///     A second piece of text to append to the message.
        /// </param>
        /// <param name="third">
        ///     A third piece of text to append to the message.
        /// </param>
        /// <param name="fourth">
        ///     A fourth piece of text to append to the message.
        /// </param>
        public void ShowMessage(
            ushort id, string second, string third, string fourth)
        {
            this.RunCustomBehaviorOr(
                () =>
                {
                    this.ShowingMessageWithId(this, EventArgs.Empty);
                    Console.WriteLine(
                        this.GetMessage(id) + second + third + fourth);
                },
                () => this.ShowMessage(0, default, default, default),
                id,
                second,
                third,
                fourth);
        }

        /// <summary>
        ///     Gets a message to show.
        /// </summary>
        /// <returns>
        ///     A message string.
        /// </returns>
        public string GetMessage()
        {
            return
                this.RunCustomBehaviorOr(
                    () => this.GetMessage(0),
                    () => this.GetMessage());
        }

        /// <summary>
        ///     Gets a message to show.
        /// </summary>
        /// <param name="id">
        ///     The message to get: 0 for the default, any other number for a
        ///     new message.
        /// </param>
        /// <returns>
        ///     A message string.
        /// </returns>
        public string GetMessage(ushort id)
        {
            return
                this.RunCustomBehaviorOr(
                    () => this[id],
                    () => this.GetMessage(0),
                    id);
        }

        /// <summary>
        ///     Gets a message to show. This overload passes the *wrong count* of
        ///     arguments to custom behavior on purpose, so custom behavior won't
        ///     work.
        /// </summary>
        /// <param name="id">
        ///     The message to get: 0 for the default, any other number for a
        ///     new message.
        /// </param>
        /// <returns>
        ///     A message string.
        /// </returns>
        public string GetMessageWithInvalidArgCountPassThru(ushort id)
        {
            return
                this.RunCustomBehaviorOr(
                    () => this.GetMessage(id),
                    () => this.GetMessageWithInvalidArgCountPassThru(0));

            // 'id' should go here ^ at the end -- not passed on purpose.
        }

        /// <summary>
        ///     Gets a message to show. This overload passes an additional,
        ///     unnecessary argument to custom behavior on purpose, which
        ///     should still allow custom behavior to work.
        /// </summary>
        /// <param name="id">
        ///     The message to get: 0 for the default, any other number for a
        ///     new message.
        /// </param>
        /// <returns>
        ///     A message string.
        /// </returns>
        public string GetMessageWithExcessArgsPassedThru(ushort id)
        {
            return
                this.RunCustomBehaviorOr(
                    () => this.GetMessage(id),
                    () => this.GetMessageWithExcessArgsPassedThru(0),
                    id,
                    "This is unnecessary but harmless...");
        }

        /// <summary>
        ///     Gets a message to show. This overload passes the *wrong type* of
        ///     argument to custom behavior on purpose, so custom behavior won't
        ///     work.
        /// </summary>
        /// <param name="id">
        ///     The message to get: 0 for the default, any other number for a
        ///     new message.
        /// </param>
        /// <returns>
        ///     A message string.
        /// </returns>
        public string GetMessageWithInvalidArgPassThru(ushort id)
        {
            return
                this.RunCustomBehaviorOr(
                    () => this.GetMessage(id),
                    () => this.GetMessageWithInvalidArgPassThru(0),
                    "INVALID");
        }
    }
}