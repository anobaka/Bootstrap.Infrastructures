using System;
using System.Collections.Generic;
using System.Text;
//#if NETCOREAPP3_0
using Microsoft.AspNetCore.Hosting;
//#endif
//#if NETSTANDARD2_0
//using Microsoft.Extensions.Hosting;
//#endif

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
