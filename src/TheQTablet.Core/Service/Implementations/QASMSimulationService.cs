using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MvvmCross.Logging;
using TheQTablet.Core.DataModel;
using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.Service.Interfaces;

namespace TheQTablet.Core.Service.Implementations
{
    public class QASMSimulationService : IQASMSimulationService
    {
        private readonly IMvxLog _log;
        private readonly IRestClient _restClient;

        public QASMSimulationService(IMvxLog log, IRestClient restClient)
        {
            _log = log;
            _restClient = restClient;
        }

        public async Task<QsamOperationResult> XRotation(float firstAngle, float secondAngle)
        {
            var atm_rot_rad = (float)(2 * firstAngle * (2 * Math.PI) / 360);
            var tel_rot_rad = (float)(2 * secondAngle * (2 * Math.PI) / 360);
            var QasmScript = string.Format("OPENQASM 2.0;\nqreg q[1];\ncreg c[1];\nrx({0}) q[0];\nrx({1}) q[0];\nmeasure q[0] -> c[0];", atm_rot_rad, tel_rot_rad);

            object data = new
            {
                script = QasmScript,
                count = 1,
                state_vector = false
            };

            QsamOperationResult res = await _restClient.MakeApiCallAsync<QsamOperationResult>("qasm", HttpMethod.Post, data);
            _log.Trace("SimulatorService:RunQASM(): result = " + res.Result.ToString());

            return res;
        }

        public Task<List<QsamOperationResult>> XRotation(float firstAngle, float secondAngle, int simulations)
        {
            throw new NotImplementedException();
        }
    }
}
