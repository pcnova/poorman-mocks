Poor Man's Mocks
================

Intro
-----

This is a small .NET library that makes it easier to implement mock objects for
testing.

As the name implies, this is not intended to be a full-fledged mocking
framework. It's for cases where, for one reason or another, you can't or don't
want to use a more complete framework.

In those cases, the easiest path to mocking is to create your own mock classes.
You could do that and not have to use third party code - not even this library!

However, that really only works for simple test scenarios. The more scenarios
you have, the more complicated and time consuming things become: you have to
keep modifying your mock's logic to account for different scenarios, or, keep
implementing additional mocks for each scenario.

This library gives you the minimal functionality you need to create your own
mocks while still being productive. It allows you to:

- create a single mock for a given interface, and,
- make the mock behave any way you want - right from the unit test.

This means the mock is reusable, which significantly reduces the amount of work
you have to do. At the same time, you get almost complete visibility into and
control of the mock, since the mocking layer is very thin, which also means the
learning curve is minimal.

If these sound like good things for your particular situation, then this library
might be useful to you.



Usage
-----

Assume there is an `IMessenger` interface that your application depends on. You
want to test how the `MessageConsumer` class uses the `Hello()` method on that
interface. There are essentially two steps to doing this:

1. Implement the mock by deriving from the `Mock` class:

       internal class MockMessenger : Mock, IMessenger
       {
           public string Hello()
           {
               return this.RunCustomBehaviorOr(() => "Howdy!", () => this.Hello());
           }
       }

2. Use the class in a unit test:

- This uses the mock as implemented:

      [TestMethod]
      public void TestGetGreeting()
      {
          // Set up.
          var mock = new MockMessenger();
          var consumer = new MessageConsumer(mock);
      
          // This should call IMessenger for a greeting.
          var greeting = consumer.GetGreeting();
        
          Assert.AreEqual("Howdy!", greeting);
      }

- This uses the mock to do something besides its normal behavior:

      [TestMethod]
      public void TestHelloCalledOnce()
      {
          // Set up.
          var mock = new MockMessenger();
          var consumer = new MessageConsumer(mock);
      
          // Modify the mock to let us know how many times it's called.
          var calls = 0;
          mock.AddBehavior(() => mock.Hello(), () => calls++);
      
          // This should call IMessenger once.
          consumer.GetGreeting();
      
          Assert.AreEqual(1, calls);
      }

- This uses the mock to completely override its normal behavior:

      [TestMethod]
      public void TestHelloWithDifferentGreeting()
      {
          // Set up.
          var mock = new MockMessenger();
          var consumer = new MessageConsumer(mock);
      
          // Modify the mock to return a different greeting.
          mock.SetBehavior(() => mock.Hello(), () => "Hey there!");
      
          // Get a greeting.
          var greeting = consumer.GetGreeting();
      
          Assert.AreEqual("Hey there!", greeting);
      }

- An interesting variation is a method that takes arguments. Assume there's a
  new `IMessenger.Hello(string name)` overload, and a corresponding method in
  `MessageConsumer` that takes a name as well.

  You would implement the method in `MockMessenger` like this:

      internal class MockMessenger : Mock, IMessenger
      {
          // [...]
        
          public string Hello(string name)
          {
              return this.RunCustomBehaviorOr(
                  () => "Howdy!",
                  () => this.Hello(default),
                  name);
          }
      }

  And then you'd be able to use the argument passed to `Hello(string)` when
  overriding the mock's behavior:

      [TestMethod]
      public void TestHelloWithName()
      {
          // Set up.
          var mock = new MockMessenger();
          var consumer = new MessageConsumer(mock);
    
          // Modify the mock to return a greeting with a passed-in name.
          mock.SetBehavior(
              () => mock.Hello(default),
              (string nme) => "Hello " + nme + "!");
    
          // Get a greeting.
          var greeting = consumer.GetGreeting("World");
    
          Assert.AreEqual("Hello World!", greeting);
      }



Details
-------

The `Mock` class is, clearly, the core of the library. It provides the
facilities to specify what the derived mock class should do by default, and to
modify it with custom behavior during tests.

The class leverages lambda expressions extensively, particularly for specifying
member names. This does add some noise to calls, but it has many benefits.

That is, the alternative would've been to specify member names as strings, which
would've made tests very brittle, would've made it difficult to differentiate
between overloads, and would've also required much more traditional, and
complicated, reflection.

By taking expressions, we avoid these downsides, while taking advantage of
compiler and tooling support (e.g., type safety, safe refactoring, Intellisense,
etc).

The class can be thought of as a specialized container of behaviors: it stores
the behaviors you assign, with a mapping to the interface member they modify,
and simply selects the behavior(s) to run when the interface member is called.

Accordingly, there are three types of behaviors the `Mock` deals with, which
basically map to the methods you need to know to use it:

- Default:
    This is the behavior that will run when nothing else is specified, and it's
    set by calling `RunCustomBehaviorOr()` from within members of a derived
    mock class.
    
    Looking at the examples in the previous section, the default behavior is
    what returns the string "Howdy!".
    
    You set this once when implementing the mock, and after this, there's mostly
    no need to worry about this again.

- Added:
    An added behavior runs along with the default behavior, and is set by
    calling `AddBehavior()` from a unit test.
    
    For the second test method in the previous section, this is what counts
    calls, but note that it does not prevent `Hello()` from returning "Howdy!".
    
    This is probably the most-used kind of behavior. It allows you to augment
    what the mock does without disturbing or having to re-implement the default
    behavior.
    
    You can use this to keep track of things, or to simulate an additional
    condition, before or after the call (more on this below).

- Replaced:
    A replacement behavior runs instead of the default behavior, and is set by
    calling `SetBehavior()` from a unit test.
    
    For the third test method in the previous section, this is what returns the
    custom string "Hey there!", replacing the default, "Howdy!".
    
    This is most useful when you need to completely override default behavior
    to test some condition, but you can also use it when you simply don't care
    for the default behavior.

When deciding between adding or replacing behavior, simply ask whether you want
the default behavior or not. In some cases, there is no real difference between
the two, e.g., when the behavior throws an exception. It's also possible to run
both kinds of behaviors on the same member, which can be occasionally useful.

There are quite a few overloads of `AddBehavior()` and `SetBehavior()`; they
exist to support members that return something versus ones that don't, and to
support members that take arguments.

There are also extensions that show up as overloads of these methods, and which
are no more than a convenience for doing this:

    messengerMock.SetBehavior(m => m.Hello(), () => "Hey there!");

instead of this:

    messengerMock.SetBehavior(() => messengerMock.Hello(), () => "Hey there!");

Besides a more succinct call, they are also useful when you resolve a mock from
a factory or dependency container, but you won't use the mock for anything else,
so you don't want to assign it to a variable:

    container.Resolve<MessengerMock>().SetBehavior(m => m.Hello(), () => "Hey!");

Finally, there are a couple options for running custom behavior, which are
specified on the object returned by `AddBehavior()` and `SetBehavior()`.

One of the options applies to both added and replaced behavior, and allows you
to run the behavior only once, as opposed to every time the member is called.
The second option applies only to added behavior, and allows you to specify
whether it should run before or after the default behavior.

For more details, see the in-code XML documentation, or, for additional examples
and use cases, take a look at the unit tests included with the library.



Constraints
-----------

- This library only works for mocking proper .NET interfaces.

  If your dependency is a class, then this can't be used because your mock would
  then need to inherit from two different classes, which is not allowed (and
  you're not about to derive all your dependencies from `Mock`... right?).

- This works for methods, properties, and indexers - the most common members
  you'll need to modify.
  
  This basically means events, or newer interface member types allowed in C# 8,
  may or may not work. It _may_ be possible to add support for some of these
  things, but it's not something I have needed in day-to-day use, so it's hard
  to see the benefit.

- This currently only works for interface members with up to 4 arguments.

  Again, this has been enough in my experience (more than that and things start
  to smell, i.e., the member should probably take an argument container object
  instead), but there is no technical reason why more arguments couldn't be
  supported in the future.