using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class ListResponse<T> : SingletonResponse<List<T>>
    {
    }
}
