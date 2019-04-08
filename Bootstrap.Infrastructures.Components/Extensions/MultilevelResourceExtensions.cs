using System.Collections.Generic;
using System.Linq;
using Bootstrap.Infrastructures.Models;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Infrastructures.Components.Extensions
{
    public static class MultilevelResourceExtensions
    {
        public enum DbType
        {
            SqlServer = 1,
            Mysql = 2
        }

        /// <summary>
        /// todo: refactor
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static ModelBuilder ConfigureMultilevelResourcePart<TData>(this ModelBuilder modelBuilder,
            DbType dbType = DbType.SqlServer) where TData : MultilevelResource<TData>
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

        public static void BuildTree<T>(this IEnumerable<MultilevelResource<T>> list) where T : MultilevelResource<T>
        {
            var dataCollection = list.ToList();
            for (var i = 0; i < dataCollection.Count; i++)
            {
                var data = dataCollection[i];
                if (i == 0)
                {
                    data.Left = data.Parent?.Left + 1 ?? 1;
                }
                else
                {
                    data.Left = dataCollection[i - 1].Right + 1;
                }

                if (data.Children?.Any() == true)
                {
                    data.Children.BuildTree();
                    data.Right = data.Children.Max(t => t.Right) + 1;
                }
                else
                {
                    data.Right = data.Left + 1;
                }
            }
        }
    }
}