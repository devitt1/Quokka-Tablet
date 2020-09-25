using System;
using System.Threading.Tasks;
using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.Service.Interfaces
{
    /*
    public interface ISimulatorService
    {
        Task<TResult> Run<TResult>() where TResult : class;
    }
    */

    public interface ISimulatorService
    {
        //        Task<PolarisationResult> Run();
        bool Run();
        float Alpha { set; get; }
        float Theta { set; get; }
    }
    /*
    public interface IPolarisationSimulatorService
    {
        Task<PolarisationResult> Run();
        float Alpha { set; get; }
        float Theta { set; get; }
    }
    */

}
