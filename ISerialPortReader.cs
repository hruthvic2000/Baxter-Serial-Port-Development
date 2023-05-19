using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialPortInterface
{
    public interface ISerialPortReader
    {
        Task<string> ReadAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, CancellationToken cancellationToken);
        bool IsPortNameValid(string portName);
        bool IsBaudRateValid(int baudRate);
        bool IsParityValid(Parity parity);
        bool IsStopBitsValid(StopBits stopBits);
        bool IsHandshakeValid(Handshake handshake);
    }
}
