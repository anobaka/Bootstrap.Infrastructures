using System.IO;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Models.ResponseModels;

namespace Bootstrap.Infrastructures.Components.FileUploader
{
    public interface IFileStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeFilename"></param>
        /// <param name="file"></param>
        /// <returns>Full file address</returns>
        Task<SingletonResponse<string>> Upload(string relativeFilename, Stream file);
    }
}