using System;
using System.Collections.Generic;

namespace TheQTablet.Core.DataModel
{
    /// <summary>
    /// Basic class with RegisterId and Operations for QSimulator APIs
    /// </summary>
    public class QSimBasicParams
    {
        public int RegisterId { get; set; } = 0;

        public OperationType Operation { get; set; } = OperationType.CREATE_CIRCUIT;
    }

    public class QsimGateOperation : QSimBasicParams
    {
        /// <summary>
        /// Gate Type
        /// </summary>
        public GateType Gate { get; set; } = GateType.HADAMARD;

        /// <summary>
        /// Number of Qbits
        /// </summary>
        public int Q { get; set; } = 0;

        /// <summary>
        /// Angle
        /// </summary>
        public float Theta { get; set; } = 0.0f;

        /// <summary>
        /// specifies the controlling qubit and in CNOT operations
        /// </summary>
        public int QControl { get; set; } = 0;

        /// <summary>
        /// the target qubit and in CNOT operations
        /// </summary>
        public int QTarget { get; set; } = 0;
    }

    

    public class QSimMeasureParam  : QSimBasicParams
    {
        /// <summary>
        /// list of qubits to measure andthe output is a value between 0 - N-1
        /// N is the number of qubits in the circuit.
        /// </summary>
        public List<int> Lq2m { get; set; } = new List<int>();
    }

    /// <summary>
    /// Specifies the initial state as a complex number (1, 0) assigned to state# 0.
    /// </summary>
    public class QSimStateParam : QSimBasicParams
    {
       
        public int N { get; set; } = 0;

        public int X { get; set; } = 1;

        public int Y { get; set; } = 0;
    }


    public enum GateType
    {
        HADAMARD,
        XROT,
        YROT,
        ZROT,
        X,
        Z,
        XZ,
        CNOT,
        CPHASE,
        LERR,
        SWAP
    }
}
