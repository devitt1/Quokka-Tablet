using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheQTablet.Core.DataModel;

namespace TheQTablet.Core.Service.Interfaces
{
    public interface IQASMSimulationService
    {
        //Run Single XRotation using QASM
        Task<QsamOperationResult> XRotation(float firstAngle, float secondAngle);

        //Run More of one XRotation using QASM
        Task<List<QsamOperationResult>> XRotation(float firstAngle, float secondAngle, int simulations);
    }
}
