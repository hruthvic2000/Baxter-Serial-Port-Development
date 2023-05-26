using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace SerialPortInterface
{
    /// <summary>
    /// Represents an interface for reading data from a serial port.
    /// </summary>
    public interface ISerialPortReader
    {
        /// <summary>
        /// Reads data asynchronously from the specified serial port.
        /// </summary>
        /// <param name="portName">The name of the serial port to read from.</param>
        /// <param name="baudRate">The baud rate of the serial port.</param>
        /// <param name="parity">The parity mode of the serial port.</param>
        /// <param name="dataBits">The number of data bits of the serial port.</param>
        /// <param name="stopBits">The stop bits of the serial port.</param>
        /// <param name="handshake">The handshake protocol of the serial port.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the read operation (optional).</param>
        /// <param name="command">The command to send to the instrument (optional).</param>
        /// <returns>A task representing the asynchronous read operation. The task returns the received data as a string.</returns>
        Task<string> ReadAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, CancellationToken cancellationToken, string command = null);

        /// <summary>
        /// Logs an information message to the event log or logging system.
        /// </summary>
        /// <param name="message">The information message to be logged.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs an error message to the event log or logging system.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        void LogError(string message);

        /// <summary>
        /// Checks if the specified port name is valid.
        /// </summary>
        /// <param name="portName">The port name to validate.</param>
        /// <returns>True if the port name is valid; otherwise, false.</returns>
        bool IsPortNameValid(string portName);

        /// <summary>
        /// Checks if the specified baud rate is valid.
        /// </summary>
        /// <param name="baudRate">The baud rate to validate.</param>
        /// <returns>True if the baud rate is valid; otherwise, false.</returns>
        bool IsBaudRateValid(int baudRate);

        /// <summary>
        /// Checks if the specified parity mode is valid.
        /// </summary>
        /// <param name="parity">The parity mode to validate.</param>
        /// <returns>True if the parity mode is valid; otherwise, false.</returns>
        bool IsParityValid(Parity parity);

        /// <summary>
        /// Checks if the specified stop bits value is valid.
        /// </summary>
        /// <param name="stopBits">The stop bits value to validate.</param>
        /// <returns>True if the stop bits value is valid; otherwise, false.</returns>
        bool IsStopBitsValid(StopBits stopBits);

        /// <summary>
        /// Checks if the specified handshake protocol is valid.
        /// </summary>
        /// <param name="handshake">The handshake protocol to validate.</param>
        /// <returns>True if the handshake protocol is valid; otherwise, false.</returns>
        bool IsHandshakeValid(Handshake handshake);
    }
}
