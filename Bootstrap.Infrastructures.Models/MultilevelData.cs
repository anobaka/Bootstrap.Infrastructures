using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bootstrap.Infrastructures.Models
{
    public abstract class MultilevelData<TData> where TData : MultilevelData<TData>
    {
        [Key] public virtual int Id { get; set; }

        [Required] public virtual string Name { get; set; }

        [NotMapped] public virtual List<TData> Children { get; set; }

        [NotMapped] public virtual TData Parent { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual int Left { get; set; }
        public virtual int Right { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual DateTime CreateDt { get; set; }

        public virtual bool IsLeaf => Right - Left <= 1;
    }
}