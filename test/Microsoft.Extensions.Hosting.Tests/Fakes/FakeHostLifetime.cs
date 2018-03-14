// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.Tests.Fakes
{
    public class FakeHostLifetime : IHostLifetime
    {
        public int StartCount { get; internal set; }
        public int StopCount { get; internal set; }

        public Func<Func<object, Task>, object, Task> StartAction { get; set; }
        public Action StopAction { get; set; }
        
        public Task RegisterDelayStartCallbackAsync(Func<object, Task> callback, object state)
        {
            StartCount++;
            return StartAction?.Invoke(callback, state) ?? callback(state);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopCount++;
            StopAction?.Invoke();
            return Task.CompletedTask;
        }
    }
}
