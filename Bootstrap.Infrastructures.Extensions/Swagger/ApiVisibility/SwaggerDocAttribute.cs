using System;

namespace Bootstrap.Infrastructures.Extensions.Swagger.ApiVisibility
{
    public class SwaggerDocAttribute : Attribute
    {
        public SwaggerDocAttribute(string docName)
        {
            DocName = docName;
        }

        public string DocName { get; }

    }
}