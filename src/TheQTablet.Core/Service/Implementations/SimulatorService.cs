using System;
using System.Threading.Tasks;
using System.Net.Http;
using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.Rest.Interfaces;
using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.Service.Implementations
{


    public class SimulatorService : ISimulatorService
    {
        //private readonly IRestClient _restClient;

        public float Alpha { set; get; }
        public float Theta { set; get; }

        public SimulatorService(IRestClient restClient)
        //public SimulatorService()
        // Add a ConnectionStatus callback/delegate/event??
        // Add REST base URL?
        {
            //    _restClient = restClient;
        }

        //public async Task<PolarisationResult> Run()
        public bool Run()
        {

            Console.WriteLine("SimulatorService:Run()");

            //QsimApiResult_CreateRegister cr_res = await _restClient.MakeApiCallAsync<QsimApiResult_CreateRegister>("http://google.com", HttpMethod.Get);
            /*

            // Create Register API call
            QsimApiResult_CreateRegister cr_res = await _restClient.MakeApiCallAsync<QsimApiResult_CreateRegister>("", HttpMethod.Post);

            // Set State API call
            QsimApiResult_SetState ss_res = await _restClient.MakeApiCallAsync<QsimApiResult_SetState>("", HttpMethod.Post);

            // Gate Operation API call
            QsimApiResult_GateOperation go_res1 = await _restClient.MakeApiCallAsync<QsimApiResult_GateOperation>("", HttpMethod.Post);
            QsimApiResult_GateOperation go_res2 = await _restClient.MakeApiCallAsync<QsimApiResult_GateOperation>("", HttpMethod.Post);

            // Measure API call
            QsimApiResult_Measure me_res = await _restClient.MakeApiCallAsync<QsimApiResult_Measure>("", HttpMethod.Post);

            // Print API call
            QsimApiResult_Print pr_res = await _restClient.MakeApiCallAsync<QsimApiResult_Print>("", HttpMethod.Post);

            // Destroy register API call
            QsimApiResult_DestroyRegister dr_res = await _restClient.MakeApiCallAsync<QsimApiResult_DestroyRegister>("", HttpMethod.Post);
            return new PolarisationResult("no error", (bool)pr_res.result[0][0]);
            */
            return true;// new PolarisationResult("no error", true);

        }
    }
}
