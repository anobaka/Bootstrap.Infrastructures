using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class HostingEnvironmentExntensions
    {
        public static bool IsTesting(this IHostingEnvironment env)
        {
            return env.IsEnvironment("Testing");
        }
    }
}
