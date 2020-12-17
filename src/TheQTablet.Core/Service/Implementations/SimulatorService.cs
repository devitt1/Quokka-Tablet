using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

using MvvmCross.Logging;

using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.DataModel;
using TheQTablet.Core.Utils;

namespace TheQTablet.Core.Service.Implementations
{


    public class SimulatorService : ISimulatorService
    {
        private readonly IRestClient _restClient;
        private readonly IMvxLog _log;

        private const string PERFORM_OPERATION_URL = "perform_operation";

        public SimulatorService(IRestClient restClient, IMvxLog log)
        {
            _restClient = restClient;
            _log = log;
        }

        /// <summary>
        /// Start a new Circuit
        /// </summary>
        /// <param name="param"></param>
        /// <param name="qbits"></param>
        /// <returns></returns>
        public async Task<QbitsOperationResult> StartOperationAsync(QSimBasicParams param, int qbits)
        {
            object data = new
            {
                operation = param.Operation.ToString().ToLower(),

                num_qubits = qbits
            };

            QbitsOperationResult result = await _restClient.MakeApiCallAsync<QbitsOperationResult>(PERFORM_OPERATION_URL, HttpMethod.Post, data);
            _log.Trace("SimulatorService:Run(): Create Gate = " + result.Result);

            return result;
        }

        /// <summary>
        /// Set initial state
        /// </summary>
        /// <param name="qSimStateParam"></param>
        /// <returns></returns>
        public async Task<BoolOperationResult> SetStateOperationAsync(QSimStateParam qSimStateParam)
        {

            object data = new
            {
                operation = qSimStateParam.Operation.ToString().ToLower(),
                register = qSimStateParam.RegisterId,
                state = qSimStateParam.State,
                complex_value = new
                {
                    im = qSimStateParam.ComplexValue.Im,
                    re = qSimStateParam.ComplexValue.Re
                }
            };

            BoolOperationResult res = await _restClient.MakeApiCallAsync<BoolOperationResult>(PERFORM_OPERATION_URL, HttpMethod.Post, data);
            _log.Trace("SimulatorService:Run(): Set State Operation = " + res.Result);

            return res;
        }

        /// <summary>
        /// Perform Gate Operations
        /// </summary>
        /// <param name="qsimGateOperation"></param>
        /// <returns></returns>
        public async Task<BoolOperationResult> GateOperationAsync(QsimGateOperation qsimGateOperation)
        {
            object data = new
            {
                register = qsimGateOperation.RegisterId,
                operation = qsimGateOperation.Operation.ToString().ToLower(),
                gate = qsimGateOperation.Gate.ToString().ToLower(),
                q = qsimGateOperation.Q,
                theta = qsimGateOperation.Theta,
            };

            BoolOperationResult res = await _restClient.MakeApiCallAsync<BoolOperationResult>(PERFORM_OPERATION_URL, HttpMethod.Post, data);
            _log.Trace("SimulatorService:Run(): Gate Operation = " + res.Result);

            return res;
        }

        /// <summary>
        /// Measure values after gate operations
        /// </summary>
        /// <param name="qSimMeasureParam"></param>
        /// <returns></returns>
        public async Task<QbitsOperationResult> MesureOperationAsync(QSimMeasureParam qSimMeasureParam)
        {
            object data = new
            {
                operation = qSimMeasureParam.Operation.ToString().ToLower(),
                register = qSimMeasureParam.RegisterId,
                lq2m = qSimMeasureParam.Lq2m,
            };

            QbitsOperationResult res = await _restClient.MakeApiCallAsync<QbitsOperationResult>(PERFORM_OPERATION_URL, HttpMethod.Post, data);
            _log.Trace("SimulatorService:Run(): Measure = " + res.Result);

            return res;
        }

        /// <summary>
        /// Read the state 
        /// </summary>
        /// <param name="param"></param>
        /// <returns>VectorOperationResult</returns>
        public async Task<VectorOperationResult> StateVectorOperationAsync(QSimBasicParams param)
        {
            object data = new
            {
                operation = param.Operation.ToString().ToLower(),
                register = param.RegisterId,
            };

            VectorOperationResult res = await _restClient.MakeApiCallAsync<VectorOperationResult>(PERFORM_OPERATION_URL, HttpMethod.Post, data);
            _log.Trace("SimulatorService:Run(): Vector State = " + res.Result);

            return res;
        }

        /// <summary>
        /// Destroy Cirtuit
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<BoolOperationResult> DestroyCircuitAsync(QSimBasicParams param)
        {
            object data = new
            {
                register = param.RegisterId,
                operation = param.Operation.ToString().ToLower()

            };

            BoolOperationResult res = await _restClient.MakeApiCallAsync<BoolOperationResult>("qasm", HttpMethod.Post, data);
            _log.Trace("SimulatorService:Run(): Destroy Circuit = " + res.Result);

            return res;
        }

        public async Task<QASMServiceResults> RunQASMAsync(float atmospheric_rot, float telescope_rot, int number_of_simulations = 1)
        {
            _log.Trace("SimulatorService:RunQASM()");

            // 2x to emulate Malus' law
            var atm_rot_rad = 2 * MathHelpers.ToRadF(atmospheric_rot);
            var tel_rot_rad = 2 * MathHelpers.ToRadF(telescope_rot);
            var QasmScript = string.Format("OPENQASM 2.0;\nqreg q[1];\ncreg c[1];\nrx({0}) q[0];\nrx({1}) q[0];\nmeasure q[0] -> c[0];", atm_rot_rad, tel_rot_rad);

            try
            {
                object data = new
                {
                    script = QasmScript,
                    count = number_of_simulations,
                    state_vector = false
                };

                QsamOperationResult res = await _restClient.MakeApiCallAsync<QsamOperationResult>("qasm", HttpMethod.Post, data);
                _log.Trace("SimulatorService:RunQASM(): result = " + res.Result.ToString());

                var ret = new QASMServiceResults("no error");
                foreach (List<int> element in res.Result.C)
                {
                    ret.Results.Add(element[0] == 0);
                }
                return ret;

            }
            catch (Exception ex)
            {
                _log.Trace("Error: {0}", ex);
                return new QASMServiceResults("API call failed");
            }
        }


        //Run basic Simulation
        public async Task<bool> RunClassicAPIAsync(float atmospheric_rot, float telescope_rot)
        {

             _log.Trace("SimulatorService:Run()");

            try
            {
                // Create Register API call

                var createParams = new QSimBasicParams()
                {
                    Operation = OperationType.CREATE_CIRCUIT
                };

                QbitsOperationResult createCircuitResult = await StartOperationAsync(createParams, 1);
                var registerId = createCircuitResult.Result;


                //Set State
                var setStateParams = new QSimStateParam()
                {
                    Operation = OperationType.SET_STATE,
                    RegisterId = registerId,
                    State = 0,
                    ComplexValue = new ComplexValue()
                    {
                        Re = 1,
                        Im = 0
                    }
                };

                BasicOperationResult stateResult = await SetStateOperationAsync(setStateParams);

                //Perform Atmospheric XRot Operation
                var atmosphereGateOperationParam = new QsimGateOperation()
                {
                    RegisterId = registerId,
                    Operation = OperationType.GATE,
                    Gate = GateType.XROT,
                    Q = 0,
                    Theta = MathHelpers.ToRadF(atmospheric_rot)
                };
                BoolOperationResult gateOperation1 = await GateOperationAsync(atmosphereGateOperationParam);

                //Perform Telescope XRot Operation
                var telescopeGateOperationParam = new QsimGateOperation()
                {
                    RegisterId = registerId,
                    Operation = OperationType.GATE,
                    Gate = GateType.XROT,
                    Q = 0,
                    Theta = MathHelpers.ToRadF(telescope_rot)
                };
                BoolOperationResult gateOperation2 = await GateOperationAsync(telescopeGateOperationParam);


                //Measure the result
                var mesureParams = new QSimMeasureParam()
                {
                    RegisterId = registerId,
                    Operation = OperationType.MEASURE

                };
                mesureParams.Lq2m.Add(0);

                QbitsOperationResult measureOperation = await MesureOperationAsync(mesureParams);

                var stateVectorParam = new QSimBasicParams()
                {
                    RegisterId = registerId,
                    Operation = OperationType.STATE_VECTOR
                };

                // Read current state
                VectorOperationResult stateVectorOperation = await StateVectorOperationAsync(stateVectorParam);
                if (stateVectorOperation.Result.Count != 1)
                {
                    throw new Exception("StateVectorOperationAsync returned an array of size != 1.");
                }
                bool result = (stateVectorOperation.Result[0].State == 1);

                var destroyCircuitParam = new QSimBasicParams()
                {
                    RegisterId = registerId,
                    Operation = OperationType.DESTROY_CIRCUIT
                };

                // Destroy Cirtuit
                BoolOperationResult destroyRegistare = await DestroyCircuitAsync(destroyCircuitParam);

                _log.Trace("SimulatorService: End, result is {0}", result);

                return result;
            }
            catch (Exception ex)
            {
                _log.Trace("Error: {0}",ex);
                return false;
            }

        }

    }
}
