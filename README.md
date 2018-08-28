Prototest
=====================

_A lightweight testing framework for .NET_

Prototest is a lightweight testing framework for .NET.  It requires no IDE plugins or extensions, nor does it require any command-line runners.  Each test project is a standard .NET executable or application (depending on the target platform), and test execution is automatically parallelised for you.

Minimal Example
------------------

Here's an example of a simple test written in Prototest:

```csharp
using Prototest.Library.Version1;

namespace Prototest.Example
{
    public class ExampleTestA
    {
        private readonly IAssert _assert;

        public ExampleTestA(IAssert assert)
        {
            _assert = assert;
        }

        public void Test1()
        {
            _assert.True(true);
        }
    }
}
```

Getting Started
------------------

If you're installing Prototest into a test assembly itself, you should install the [Prototest](https://www.nuget.org/packages/Prototest/) package and target .NET Core (console application) for the test application.

If you just want to use Prototest's APIs inside another library (which will then be used in a test assembly), only install the [Prototest.Runtime](https://www.nuget.org/packages/Prototest.Runtime/).

Supported Platforms
----------------------

Prototest runs on test assemblies using .NET Core or .NET Standard 2.0 and higher.

Build Status
-------------

Prototest is built on AppVeyor.

|     | Status |
| --- | ----- |
| Prototest | [![Build status](https://ci.appveyor.com/api/projects/status/lmpg5vywarlwt2c0?svg=true)](https://ci.appveyor.com/project/hach-que/prototest) |

Features
------------

* Extremely lightweight, no external runtime dependencies.
* Supports both desktop and mobile devices.
* Forward and backward compatible API design, with namespaces used for interface versioning.
* Tests automatically run in parallel.
* Tests can be generated programmatically, by implementing `ITestSetProvider`.

How to Contribute
--------------------

To contribute to Prototest, submit a pull request to `RedpointGames/Prototest`.

The developer chat is hosted on [Gitter](https://gitter.im/RedpointGames/Prototest)

[![Gitter](https://badges.gitter.im/RedpointGames/Prototest.svg)](https://gitter.im/RedpointGames/Prototest?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Providing Feedback / Obtaining Support
-----------------------------------------

To provide feedback or get support about issues, please file a GitHub issue on this repository.

License Information
---------------------

Prototest is licensed under the MIT license.

```
Copyright (c) 2015 Redpoint Games Pty Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
```

Community Code of Conduct
------------------------------

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).
