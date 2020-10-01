using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

using MvvmCross.Logging;

using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.Service.Implementations
{


    public class SimulatorService : ISimulatorService
    {
        private readonly IRestClient _restClient;
        private readonly IMvxLog _log;

        public float Alpha { set; get; }
        public float Theta { set; get; }

        public SimulatorService(IRestClient restClient, IMvxLog log)
        //public SimulatorService()
        // Add a ConnectionStatus callback/delegate/event??
        // Add REST base URL?
        {
            _restClient = restClient;
            _log = log;
        }

        public async Task RunManySim()
        {
            for (int i =0; i<10; ++i)
            {
                bool res = await this.Run();
                _log.Trace("SimulatorService:RunManySim()[" + i + "]:" + res);
            }
        }

        //public async Task<PolarisationResult> Run()
        public async Task<bool> Run()
        {

            _log.Trace("SimulatorService:Run()");


            // Create Register API call
            
            var json_request_body = new System.Collections.Generic.Dictionary<string, int>();
            json_request_body["num_qubits"] = 1;
            
            QsimApiResult_CreateRegister cr_res = await _restClient.MakeApiCallAsync<QsimApiResult_CreateRegister>("http://127.0.0.1:5002/qsim/create_register", HttpMethod.Post, json_request_body);
            //_log.Trace("SimulatorService:Run(): QsimApiResult_CreateRegister=" + cr_res.result);

            // Set State API call
            json_request_body = new System.Collections.Generic.Dictionary<string, int>();
            json_request_body["register"] = cr_res.result;
            json_request_body["n"] = 1;
            json_request_body["x"] = 1;
            json_request_body["y"] = 0;
            /*
            object object_request_body = new
            {
                register = cr_res.result,
                n = 1,
                x = 1,
                y = 0
            };
            */

            QsimApiResult_SetState ss_res = await _restClient.MakeApiCallAsync<QsimApiResult_SetState>("http://127.0.0.1:5002/qsim/set_state", HttpMethod.Post, json_request_body);
            //_log.Trace("SimulatorService:Run(): QsimApiResult_SetState=" + ss_res.result);
            
            // Gate Operation API call
            var gateOperationAPIParamJson = new QsimApiParam_GateOperation();
            /*
            object data = new
            {
                email = "soething",
                password = "password"
            };
            */

            /*
             *
              [JsonProperty(PropertyName = "quiz")]
        private Quiz userQuiz;
        public Quiz UserQuiz { get => userQuiz; set => userQuiz = value; }
             */
            gateOperationAPIParamJson.register = cr_res.result;
            gateOperationAPIParamJson.gate = "xrot";
            gateOperationAPIParamJson.q = 0;
            gateOperationAPIParamJson.theta = (float)0;
            QsimApiResult_GateOperation go_res1 = await _restClient.MakeApiCallAsync<QsimApiResult_GateOperation>("http://127.0.0.1:5002/qsim/gate", HttpMethod.Post, gateOperationAPIParamJson);
            //_log.Trace("SimulatorService:Run(): QsimApiResult_Gate=" + go_res1.result);
            gateOperationAPIParamJson.q = 0;
            //gateOperationAPIParamJson.theta = (float)0.7857;
            gateOperationAPIParamJson.theta = (float)1.571;
            //gateOperationAPIParamJson.theta = (float)3.1;
            QsimApiResult_GateOperation go_res2 = await _restClient.MakeApiCallAsync<QsimApiResult_GateOperation>("http://127.0.0.1:5002/qsim/gate", HttpMethod.Post, gateOperationAPIParamJson);
            //_log.Trace("SimulatorService:Run(): QsimApiResult_Gate=" + go_res2.result);


            // Measure API call
            var measureAPIParamJson = new QsimApiParam_Measure();
            measureAPIParamJson.register = cr_res.result;
            measureAPIParamJson.lq2m = new List<int>();
            measureAPIParamJson.lq2m.Add(0);
            //_log.Trace("SimulatorService:Run(): Measure. Parameters:" + measureAPIParamJson);
            QsimApiResult_Measure me_res = await _restClient.MakeApiCallAsync<QsimApiResult_Measure>("http://127.0.0.1:5002/qsim/measure", HttpMethod.Post, measureAPIParamJson);
            //_log.Trace("SimulatorService:Run(): QsimApiResult_Measure=" + me_res.result);
            
            // Print API call
            json_request_body = new System.Collections.Generic.Dictionary<string, int>();
            json_request_body["register"] = cr_res.result;
            QsimApiResult_Print pr_res = await _restClient.MakeApiCallAsync<QsimApiResult_Print>("http://127.0.0.1:5002/qsim/print", HttpMethod.Post, json_request_body);
            //string test = pr_res.result[0][0].ToString();
            //_log.Trace("SimulatorService:Run(): QsimApiResult_Print=" + test);


            // Destroy register API call
            QsimApiResult_DestroyRegister dr_res = await _restClient.MakeApiCallAsync<QsimApiResult_DestroyRegister>("http://127.0.0.1:5002/qsim/destroy_register", HttpMethod.Post, json_request_body);
            //_log.Trace("SimulatorService:Run(): QsimApiResult_DestroyRegister=" + dr_res.result);
            
            if (pr_res.result[0][1].ToString() == "0")
            {
                _log.Trace("SimulatorService:Run(): returning false");
                return false;
            }
            else if (pr_res.result[0][1].ToString() == "1")
            {
                _log.Trace("SimulatorService:Run(): returning true");
                return true;
            }

            string exception_message = "SimulatorService:Run(): QsimApiResult_Print neither 1 or 0: " + pr_res.result[0][1];
            _log.Trace(exception_message);
            _log.ErrorException(exception_message, new Exception(exception_message));
            return false;

        }
    }
}
