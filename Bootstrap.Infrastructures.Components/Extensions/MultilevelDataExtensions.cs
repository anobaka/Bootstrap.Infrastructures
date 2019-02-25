using Bootstrap.Infrastructures.Models;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Infrastructures.Components.Extensions
{
    public static class MultilevelDataExtensions
    {
        public enum DbType
        {
            SqlServer = 1,
            Mysql = 2
        }

        public static ModelBuilder ConfigureMultilevelDataPart<TData>(this ModelBuilder modelBuilder,
            DbType dbType = DbType.SqlServer) where TData : MultilevelData<TData>
        {
            return modelBuilder.Entity<TData>(r =>
            {
                r.HasIndex(t => new {t.Left, t.Right});
                r.HasIndex(t => t.ParentId);
                r.HasIndex(t => t.Name);

                if (dbType == DbType.Mysql)
                {
                    r.Property(a => a.Id).HasColumnName("id");
                    r.Property(a => a.Left).HasColumnName("left");
                    r.Property(a => a.Right).HasColumnName("right");
                    r.Property(a => a.Name).HasColumnName("name");
                    r.Property(a => a.ParentId).HasColumnName("parent_id");
                    r.Property(a => a.CreateDt).HasColumnName("create_dt");
                }
            });
        }
    }
}