using System;
using System.Threading.Tasks;

using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.Service.Interfaces
{
    public interface ISimulatorService
    {

        /// <summary>
        /// Run basic simulation 
        /// </summary>
        /// <param name="atmospheric_rot"></param>
        /// <param name="telescope_rot"></param>
        /// <param name="api_type"></param>
        /// <returns></returns>
        Task<bool> RunClassicAPIAsync(float atmospheric_rot, float telescope_rot);

        /// <summary>
        /// Run basic simulation 
        /// </summary>
        /// <param name="atmospheric_rot"></param>
        /// <param name="telescope_rot"></param>
        /// <param name="api_type"></param>
        /// <param name="number_of_experiments"></param>
        /// <returns></returns>
        Task<QASMServiceResults> RunQASMAsync(float atmospheric_rot, float telescope_rot, int number_of_experiments = 1);

        /// <summary>
        /// Run QASM Simulation
        /// </summary>
        /// <param name="atmospheric_rot"></param>
        /// <param name="telescope_rot"></param>
        /// <param name="satelite_rot"></param>
        /// <param name="number_of_simulations"></param>
        /// <returns></returns>
        Task<QASMServiceResults> RunQASMAsync(float atmospheric_rot, float telescope_rot, float satelite_rot, int number_of_simulations = 1);

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
