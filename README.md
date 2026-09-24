# Dynamic

An experimental C# runtime exploring how far a statically typed language can be pushed toward the runtime dynamism found in languages such as Ruby and Smalltalk.

Dynamic grew out of a broader experiment in modelling objects whose state and capabilities can evolve during their lifetime, particularly in the context of decision-making systems informed by traditional logic, philosophical psychology and metaphysics.

The original question was simple:

> **How dynamic can C# actually be?**

The answer turns out to be: quite dynamic.

## What does it do?

Dynamic explores runtime capabilities including:

- evaluating C# code at runtime;
- evaluating code in the context of an individual object;
- defining methods and properties dynamically;
- adding behaviour to an object's runtime class;
- dynamic message dispatch;
- `MethodMissing`-style fallback behaviour;
- runtime argument binding and conversion.

For example:

```csharp
dynamic bob = new Person("Bob");

bob.ClassEval("""
    public string DisplayName()
    {
        return FirstName;
    }

    public string Greeting()
    {
        return "Hello " + DisplayName();
    }
    """);

Console.WriteLine(bob.Greeting());
```

Messages can also be dispatched by name:

```csharp
bob.Send("Greeting");
```

The runtime also provides `Eval` facilities for evaluating C# directly and in the context of an object.

## How does it work?

Underneath the API is a combination of several C# and .NET runtime facilities, including:

- `DynamicObject`;
- the Dynamic Language Runtime (DLR);
- Roslyn runtime compilation;
- reflection;
- runtime binders;
- syntax rewriting;
- dynamically defined methods and properties.

The project is deliberately experimental. Its purpose has been to investigate what is possible when some of the dynamism normally associated with languages such as Ruby is introduced into C#.

## Where next?

The experiment works, but it has also led me to reconsider the original approach.

I've come to think that I was putting the dynamism in the wrong place.

Rather than changing an object's runtime class, I'm now exploring a model in which its CLR type remains stable while its state and capabilities evolve through composition over its lifetime.

In other words, the more interesting question may no longer be:

> **How can I change what this type can do at runtime?**

but:

> **How should we represent what this particular object can do now?**

There is also a simpler possibility.

After implementing runtime compilation, DLR dispatch, `Eval`, `ClassEval`, `Send`, dynamically defined members and `MethodMissing` in pursuit of some of Ruby's flexibility...

perhaps I should just use Ruby. 😄

## Status

Dynamic is a work in progress and an experimental codebase rather than a production framework.

The current implementation and test suite preserve the results of the original runtime-dynamism experiment while I explore the next direction.

The project remains useful both as an investigation of C# runtime capabilities and as a record of the design path that led to a different question.

## The real objective

All of this is ultimately in pursuit of something rather less abstract:

**an intelligent goblin for a game I have in mind.**

The runtime machinery was never the destination. It was one attempt at finding the right computational model for objects that can possess capabilities, change over time, perceive a world, and eventually participate in richer forms of decision-making.

That investigation continues.
