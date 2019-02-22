using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bootstrap.Infrastructures.Extensions.Swagger
{
    /// <summary>
    /// Describe the names and values of an enum type.
    /// todo: don't know the new logic in swagger gen v5.0, this is a temporary fix.
    /// </summary>
    public class EnumSchemaFilter : IDocumentFilter, ISchemaFilter, IOperationFilter
    {
        private void _applyEnumSchemeRecursively(OpenApiSchema model, Type type)
        {
            if (type.IsEnum)
            {
                var additionalDesc = _buildDescription(type);
                if (model.Description?.Contains(additionalDesc) != true)
                {
                    model.Description += $"{additionalDesc}";
                }
            }

            if (model.Properties?.Any() == true)
            {
                foreach (var p in model.Properties)
                {
                    var propertyType =
                        type.GetProperties()
                            .FirstOrDefault(t => t.Name.Equals(p.Key, StringComparison.OrdinalIgnoreCase))
                            ?.PropertyType ?? type.GetFields()
                            .FirstOrDefault(t => t.Name.Equals(p.Key, StringComparison.OrdinalIgnoreCase))?.FieldType;
                    if (propertyType != null)
                    {
                        _applyEnumSchemeRecursively(p.Value, propertyType);
                    }
                }
            }
        }

        private string _buildDescription(Type type)
        {
            string a = null;
            foreach (var e in Enum.GetValues(type))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    a += ", ";
                }

                a += $"{(int)e}: {e}";
            }

            a = $"[{a}]";

            return a;
        }

        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            _applyEnumSchemeRecursively(model, context.SystemType);
        }


        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters?.Any() == true)
            {
                foreach (var p in operation.Parameters)
                {
                    if (p.Schema.Enum.Count > 0)
                    {
                        var enumType = context.ApiDescription.ParameterDescriptions.FirstOrDefault(t =>
                            t.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
                        if (enumType != null)
                        {
                            var type = enumType.Type;
                            if (type.IsGenericType)
                            {
                                type = type.GenericTypeArguments.FirstOrDefault();
                            }

                            if (type?.IsEnum == true)
                            {
                                var additionalDesc = _buildDescription(type);
                                if (p.Schema.Description?.Contains(additionalDesc) != true)
                                {
                                    p.Schema.Description += $"{additionalDesc}";
                                }
                            }
                        }

                        p.Description += p.Schema.Description;
                    }
                }
            }
        }
    }
}