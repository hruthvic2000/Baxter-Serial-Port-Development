using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SerialPortConnect;

namespace SerialPortWrapper
{
    public class SerialPortDataReader:SerialPortReader
    {
        private SerialPortReader serialPortReader;
        public string GetData(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, int readingTimeOut)
        {
            using(serialPortReader=new SerialPortReader())
            {
                lock(serialPortReader)
                {
                    var readTimeOut = TimeSpan.FromSeconds(readingTimeOut); // Set timeout to 5 seconds
                    var cancellationToken = new CancellationTokenSource(readTimeOut);
                        Task<string> serialPortData = serialPortReader.ReadAsync(portName, baudRate, parity, dataBits, stopBits, handshake, cancellationToken.Token);
                    serialPortData.Wait();
                    return serialPortData.Result;
                }
            }
        }
    }
}
