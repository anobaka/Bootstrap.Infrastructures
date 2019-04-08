using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Models
{
    public abstract class ActiveMultilevelResource<TResource> : MultilevelResource<TResource> where TResource : ActiveMultilevelResource<TResource>
    {
        public bool Active { get; set; }
    }
}