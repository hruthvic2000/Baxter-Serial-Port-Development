using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SerialPortInterface;

namespace SerialPortConnect
{
    public class SerialPortReader : ISerialPortReader,IDisposable
    {
        private SerialPort _port;

        // This method reads data from the specified serial port asynchronously and returns it as a string
        // If any errors occur, it returns null and logs an error message to the console
        public async Task<string> ReadAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, CancellationToken cancellationToken, string Command)
        {
            try
            {
                // Validate input parameters
                if (!IsPortNameValid(portName))
                {
                    throw new ArgumentException("Invalid port name");
                }

                if (!IsBaudRateValid(baudRate))
                {
                    throw new ArgumentException("Invalid baud rate");
                }

                if (!IsDataBitsValid(dataBits))
                {
                    throw new ArgumentException("Invalid data bits");
                }

                if (!IsParityValid(parity))
                {
                    throw new ArgumentException("Invalid parity value");
                }

                if (!IsStopBitsValid(stopBits))
                {
                    throw new ArgumentException("Invalid stop bits value");
                }

                if (!IsHandshakeValid(handshake))
                {
                    throw new ArgumentException("Invalid handshake value");
                }

                // Create a new SerialPort instance with the specified parameters
                _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

                // Set the handshake value
                _port.Handshake = handshake;
                _port.NewLine = "\r\n";
                await Task.Delay(1000);
                // If the port is not open, open it
                if (!_port.IsOpen)
                {
                    _port.Open();
                }
                // Set a timeout for the read operation
                var timeout = TimeSpan.FromSeconds(5); // Set timeout to 5 seconds
                using (var timeoutCts = new CancellationTokenSource(timeout))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
                    // Clear the input buffer
                    _port.DiscardInBuffer();

                    // Send a command to the instrument
                    //string command = "YOUR_COMMAND";
                    _port.WriteLine(Command);

                    // Wait for data to be available on the port or for the timeout to expire
                    var readTask = _port.BaseStream.ReadAsync(new byte[1], 0, 1, linkedCts.Token);

                    // Wait for either the read operation to complete or for the timeout to expire
                    var completedTask = await Task.WhenAny(readTask, Task.Delay(timeout, linkedCts.Token));

                    // If the read operation completed successfully, read the data from the port
                    if (completedTask == readTask)
                    {
                        byte[] buffer = new byte[_port.BytesToRead];
                        int bytesRead = _port.Read(buffer, 0, buffer.Length);
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        //data=_port.ReadLine();

                        // Check the received response for ACK or NAK
                        if (data.Contains("\x06"))
                        {
                            // ACK received, command recognized and completed
                            return "Command successfully completed \nReceived response: " + data.Trim();
                        }
                        else if (data.Contains("\x15"))
                        {
                            // NAK received, error with the command string
                            return "Error: Invalid command string \nReceived response: " + data.Trim();
                        }
                        else
                        {
                            // Other response received, handle as needed
                            return "Received response: " + data.Trim();
                        }

                    }
                    else
                    {
                        // If the read operation was cancelled due to a timeout, throw a TimeoutException
                        throw new TimeoutException("Timeout while reading from port");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                // Log an error message to the console if any of the input parameters are invalid
                Console.WriteLine($"Invalid parameter value: {ex.Message}");
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log an error message to the console if access to the port is denied
                Console.WriteLine($"Access to port {portName} is denied: {ex.Message}");
                return null;
            }
            catch (TimeoutException ex)
            {
                // Log an error message to the console if the read operation times out
                Console.WriteLine($"Timeout while reading from port {portName}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log a generic error message to the console for any other exceptions that may occur
                Console.WriteLine($"Failed to read from the serial port: {ex.Message}");
                return null;
            }
            finally
            {
                // Close and dispose of the SerialPort instance
                _port?.Close();
                _port?.Dispose();
            }
        }
        // This method checks if the specified port name is valid by checking if it exists in the list of available port names
        public bool IsPortNameValid(string portName)
        {
            return SerialPort.GetPortNames().Contains(portName);
        }

        // This method checks if the specified baud rate is valid by ensuring it is greater than 0 and less than or equal to 115200
        public bool IsBaudRateValid(int baudRate)
        {
            return baudRate > 0 && baudRate <= 115200;
        }

        // This method checks if the specified data bits value is valid by ensuring it is either 5, 6, 7, or 8
        public bool IsDataBitsValid(int dataBits)
        {
            return dataBits == 5 || dataBits == 6 || dataBits == 7 || dataBits == 8;
        }

        // This method checks if the specified parity value is valid by ensuring it is one of the valid Parity enum values
        public bool IsParityValid(Parity parity)
        {
            return Enum.IsDefined(typeof(Parity), parity);
        }

        // This method checks if the specified stop bits value is valid by ensuring it is one of the valid StopBits enum values
        public bool IsStopBitsValid(StopBits stopBits)
        {
            return Enum.IsDefined(typeof(StopBits), stopBits);
        }

        //This method checks if the specified handshake value is valid by ensuring it is one of the valid Handshake enum values
        public bool IsHandshakeValid(Handshake handshake)
        {
            return Enum.IsDefined(typeof(Handshake), handshake);
        }

        public void Dispose()
        {
            // Close and dispose of the SerialPort instance when the object is disposed
            _port?.Close();
            _port?.Dispose();
        }
    }

}
