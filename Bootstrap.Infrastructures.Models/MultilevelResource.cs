using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bootstrap.Infrastructures.Models
{
    public abstract class MultilevelResource<TResource> where TResource : MultilevelResource<TResource>
    {
        [Key] public virtual int Id { get; set; }

        [Required] public virtual string Name { get; set; }

        [NotMapped] public virtual List<TResource> Children { get; set; }

        [NotMapped] public virtual TResource Parent { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual int Left { get; set; }
        public virtual int Right { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual DateTime CreateDt { get; set; } = DateTime.Now;

        public virtual bool IsLeaf => Right - Left <= 1;
    }
}