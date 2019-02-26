using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Models
{
    public abstract class ActiveMultilevelData<TData> : MultilevelData<TData> where TData : ActiveMultilevelData<TData>
    {
        public bool Active { get; set; }
    }
}