using System;
using System.Threading.Tasks;

using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.Service.Interfaces
{
    public interface ISimulatorService
    {
        Task RunManySim();

        /// <summary>
        /// Run basic simulation 
        /// </summary>
        /// <returns></returns>
        Task<bool> Run();

        /// <summary>
        /// Run basic simulation 
        /// </summary>
        /// <param name="atmospheric_rot"></param>
        /// <param name="telescope_rot"></param>
        /// <returns></returns>
        Task<bool> Run(float atmospheric_rot, float telescope_rot);

        /// <summary>
        /// Start a new Circuit
        /// </summary>
        /// <param name="param"></param>
        /// <param name="qbits"></param>
        /// <returns></returns>
        Task<QbitsOperationResult> StartOperationAsync(QSimBasicParams param, int qbits);

        /// <summary>
        /// Set initial state
        /// </summary>
        /// <param name="qSimStateParam"></param>
        /// <returns></returns>
        Task<BoolOperationResult> SetStateOperationAsync(QSimStateParam qSimStateParam);

        /// <summary>
        /// Perform Gate Operations
        /// </summary>
        /// <param name="qsimGateOperation"></param>
        /// <returns></returns>
        Task<BoolOperationResult> GateOperationAsync(QsimGateOperation qsimGateOperation);

        /// <summary>
        /// Measure values after gate operations
        /// </summary>
        /// <param name="qSimMeasureParam"></param>
        /// <returns></returns>
        Task<QbitsOperationResult> MesureOperationAsync(QSimMeasureParam qSimMeasureParam);

        /// <summary>
        /// Read the state 
        /// </summary>
        /// <param name="param"></param>
        /// <returns>VectorOperationResult</returns>
        Task<VectorOperationResult> StateVectorOperationAsync(QSimBasicParams param);

        /// <summary>
        /// Destroy Cirtuit
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<BoolOperationResult> DestroyCircuitAsync(QSimBasicParams param);

    }

}
