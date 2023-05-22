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
    /// <summary>
    /// Provides a wrapper for reading data from a serial port using the SerialPortReader class.
    /// </summary>
    public class SerialPortDataReader : SerialPortReader
    {
        private SerialPortReader serialPortReader;

        /// <summary>
        /// Gets data from the specified serial port.
        /// </summary>
        /// <param name="portName">The name of the serial port.</param>
        /// <param name="baudRate">The baud rate of the serial port.</param>
        /// <param name="parity">The parity mode of the serial port.</param>
        /// <param name="dataBits">The number of data bits of the serial port.</param>
        /// <param name="stopBits">The stop bits of the serial port.</param>
        /// <param name="handshake">The handshake protocol of the serial port.</param>
        /// <param name="readingTimeOut">The timeout for the read operation in seconds.</param>
        /// <param name="command">The command to send to the instrument (optional).</param>
        /// <returns>The received data as a string.</returns>
        public string GetData(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, int readingTimeOut, string command)
        {
            using (serialPortReader = new SerialPortReader())
            {
                // Lock the serialPortReader instance to ensure thread safety
                lock (serialPortReader)
                {
                    var readTimeOut = TimeSpan.FromSeconds(readingTimeOut);
                    var cancellationToken = new CancellationTokenSource(readTimeOut);

                    // Read data asynchronously from the serial port
                    Task<string> serialPortData = serialPortReader.ReadAsync(portName, baudRate, parity, dataBits, stopBits, handshake, cancellationToken.Token, command);

                    serialPortData.Wait(); // Wait for the read operation to complete
                    return serialPortData.Result; // Return the received data
                }
            }
        }
    }
}
