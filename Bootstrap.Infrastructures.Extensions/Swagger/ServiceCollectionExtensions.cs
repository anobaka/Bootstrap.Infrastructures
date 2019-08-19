using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Bootstrap.Infrastructures.Extensions.Swagger.ApiVisibility;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bootstrap.Infrastructures.Extensions.Swagger
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Common configuration.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="docs"></param>
        private static void Configure(this SwaggerGenOptions t, IEnumerable<KeyValuePair<string, OpenApiInfo>> docs)
        {
            if (docs != null)
            {
                foreach (var kv in docs)
                {
                    t.SwaggerDoc(kv.Key, kv.Value);
                }
            }

            t.DescribeAllParametersInCamelCase();
            t.DescribeStringEnumsInCamelCase();

            t.DocumentFilter<EnumSchemaFilter>();
            t.SchemaFilter<EnumSchemaFilter>();
            t.OperationFilter<EnumSchemaFilter>();

            t.EnableAnnotations();
        }

        public static IServiceCollection
            AddBootstrapSwaggerGen(this IServiceCollection services, string docName, string docTitle) =>
            services.AddBootstrapSwaggerGen(new Dictionary<string, OpenApiInfo>
                {{docName, new OpenApiInfo {Version = "v1", Title = docTitle}}});

        public static IServiceCollection AddBootstrapSwaggerGen(this IServiceCollection services,
            IEnumerable<KeyValuePair<string, OpenApiInfo>> docs)
        {
            return services.AddSwaggerGen(t => { Configure(t, docs); });
        }

        public static IServiceCollection AddBootstrapSwaggerGen<TApiVisibleAttribute, TApiVisibleRealm>(
            this IServiceCollection services,
            IEnumerable<KeyValuePair<string, OpenApiInfo>> docs) where TApiVisibleRealm : Enum
            where TApiVisibleAttribute : Attribute, IApiVisibilityAttribute<TApiVisibleRealm>
        {
            return services.AddSwaggerGen(t =>
            {
                Configure(t, docs);

                t.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

                    var apiVisibility = methodInfo.GetCustomAttribute<TApiVisibleAttribute>(true);

                    if (apiVisibility == null || apiVisibility.Realms?.Any() != true)
                    {
                        return true;
                    }

                    if (apiVisibility.Realms?.Any(r => Convert.ToInt32(r) == 0) == true)
                    {
                        return false;
                    }

                    var docNames = new List<string>();
                    var enumDocs = SpecificEnumUtils<TApiVisibleRealm>.Values
                        .ToDictionary(r => r,
                            r => r.GetAttributeOfType<SwaggerDocAttribute>()?.DocName)
                        .Where(r => !string.IsNullOrEmpty(r.Value)).ToDictionary(r => r.Key, r => r.Value);
                    foreach (var r in apiVisibility.Realms)
                    {
                        SpecificEnumUtils<TApiVisibleRealm>.Values.ForEach(sr =>
                        {
                            if (r.HasFlag(sr))
                            {
                                if (enumDocs.TryGetValue(sr, out var d))
                                {
                                    docNames.Add(d);
                                }
                            }
                        });
                    }

                    return docNames.Contains(docName);
                });
            });
        }
    }
}