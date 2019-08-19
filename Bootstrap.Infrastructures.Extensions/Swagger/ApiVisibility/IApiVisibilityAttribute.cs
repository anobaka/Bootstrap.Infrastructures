namespace Bootstrap.Infrastructures.Extensions.Swagger.ApiVisibility
{
    public interface IApiVisibilityAttribute<out TRealm>
    {
        TRealm[] Realms { get; }
    }
}
