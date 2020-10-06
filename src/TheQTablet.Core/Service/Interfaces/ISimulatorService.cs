using System;
using System.Threading.Tasks;

namespace TheQTablet.Core.Service.Interfaces
{
    public interface ISimulatorService
    {
        Task RunManySim();
        Task<bool> Run();
        //        Task<PolarisationResult> Run();
        float Alpha { set; get; }
        float Theta { set; get; }
    }
}
