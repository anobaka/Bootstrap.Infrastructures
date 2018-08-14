using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bootstrap.Infrastructures.Extensions.Swagger
{
    public class EnumSchemaFilter : IDocumentFilter, ISchemaFilter, IOperationFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
        }

        private void _applyEnumSchemeRecursively(Schema model)
        {
            if (model.Enum?.Any() == true)
            {
                var additionalDesc = _buildDescription(model.Enum);
                if (model.Description?.Contains(additionalDesc) != true)
                {
                    model.Description += $"->{additionalDesc}";
                }
            }

            if (model.Properties?.Any() == true)
            {
                foreach (var p in model.Properties)
                {
                    _applyEnumSchemeRecursively(p.Value);
                }
            }
        }

        public void Apply(Schema model, SchemaFilterContext context)
        {
            _applyEnumSchemeRecursively(model);
        }

        private string _buildDescription(IList<object> enums)
        {
            string a = null;
            if (enums != null)
            {
                foreach (var e in enums)
                {
                    if (!string.IsNullOrEmpty(a))
                    {
                        a += ", ";
                    }

                    a += $"{(int) e}: {e}";
                }

                a = $"[{a}]";
            }

            return a;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters?.Any() == true)
            {
                foreach (var p in operation.Parameters)
                {
                    if (p is NonBodyParameter t)
                    {
                        var s = _buildDescription(t.Enum);
                        if (!string.IsNullOrEmpty(s) && t.Description?.Contains(s) != true)
                        {
                            t.Description += s;
                        }
                    }
                }
            }
        }
    }
}