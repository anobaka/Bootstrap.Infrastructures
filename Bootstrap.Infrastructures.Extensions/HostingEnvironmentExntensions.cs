using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;

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
