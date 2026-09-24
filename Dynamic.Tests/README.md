# DynamicRuntimeTests

These files are intended to be copied into the existing `DynamicRuntimeTests`
xUnit v3 project.

## Assumptions

- The test project already references the runtime project containing:
  - `Person`
  - `ActiveClass<TSelf>`
  - `ClassRegistry<TSelf>`
  - `DynamicClass<TSelf>`
  - `RuntimeCompilationException`
- The test project has xUnit v3 configured.
- Tests run through the public `ClassEval` / `Eval` APIs rather than testing
  `SelfMemberRewriter` directly.
- `RuntimeTestBase` resets `ClassRegistry<Person>` by reflection because the
  registry is static and its `Replace` method is internal.
- Parallel test execution is disabled because class definitions are shared
  process-wide through `ClassRegistry<Person>`.

## Test philosophy

The suite has two important types of assertions:

1. **Successful runtime behavior**
   Runtime C# compiles and the resulting dynamic behavior is asserted.

2. **Compiler-preservation behavior**
   Invalid C# must stay invalid after rewriting. These tests assert Roslyn
   diagnostics such as `CS0165` and `CS0103`.

That second category is especially important: the rewriter decides what an
identifier means, while Roslyn remains responsible for language rules such as
definite assignment.
