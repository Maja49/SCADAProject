﻿using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();

            byte[] req = new byte[12];

            req[0] = BitConverter.GetBytes(CommandParameters.TransactionId)[1];
            req[1] = BitConverter.GetBytes(CommandParameters.TransactionId)[0];
            req[2] = BitConverter.GetBytes(CommandParameters.ProtocolId)[1];
            req[3] = BitConverter.GetBytes(CommandParameters.ProtocolId)[0];
            req[4] = BitConverter.GetBytes(CommandParameters.Length)[1];
            req[5] = BitConverter.GetBytes(CommandParameters.Length)[0];
            req[6] = CommandParameters.UnitId;
            req[7] = CommandParameters.FunctionCode;
            req[8] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).StartAddress)[1];
            req[9] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).StartAddress)[0];
            req[10] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).Quantity)[1];
            req[11] = BitConverter.GetBytes(((ModbusReadCommandParameters)CommandParameters).Quantity)[0];

            return req;

        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();
            Dictionary<Tuple<PointType, ushort>, ushort> resp = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCount = response[8];
            ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;

            if (response[7] == CommandParameters.FunctionCode + 0x80)
            {
                HandeException(response[8]);
            }
            else
            {
                for (int i = 0; i < byteCount; i += 2)
                {
                    ushort value = BitConverter.ToUInt16(response, 9 + i);
                    value = (ushort)IPAddress.NetworkToHostOrder((short)value);
                    Tuple<PointType, ushort> tuple = new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, startAddress++);
                    resp.Add(tuple, value);
                }
            }

            return resp;
        }
    }
}