using SerialPortInterface;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialPortConnect
{
    public class SerialPortReader : ISerialPortReader,IDisposable
    {
        private SerialPort _port;
        private EventLog _systemEventLog;

        // Specify the name of the Event Log as a constant
        private const string EventSourceName = "bxtrSerialPortSource";
        private const string EventLogName = "bxtrSerialPort";

        public SerialPortReader()
        {
            // Create an instance of the EventLog using the specified event log name
            EventLog eventLog = new EventLog(EventLogName);

            // Check if the event log doesn't already exist and the event source doesn't already exist
            if (!EventLog.GetEventLogs().Contains(eventLog) && !EventLog.SourceExists(EventSourceName))
            {
                // Create a new event source using the specified event source name and event log name
                EventSourceCreationData data1 = new EventSourceCreationData(EventSourceName, EventLogName);
                EventLog.CreateEventSource(data1);
            }

            // Create a new instance of the EventLog
            _systemEventLog = new EventLog();

            // Set the source of the _systemEventLog to the specified event source name
            _systemEventLog.Source = EventSourceName;
        }

        // This method reads data from the specified serial port asynchronously and returns it as a string
        // If any errors occur, it returns null and logs an error message to the Event Viewer
        public async Task<string> ReadAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, CancellationToken cancellationToken, string Command)
        {
            try
            {
                // Validate input parameters
                if (!IsPortNameValid(portName))
                {
                    string errorMessage = "Invalid port name";
                    LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                if (!IsBaudRateValid(baudRate))
                {
                    string errorMessage = "Invalid baud rate";
                    LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                if (!IsDataBitsValid(dataBits))
                {
                    string errorMessage = "Invalid data bits";
                    LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                if (!IsParityValid(parity))
                {
                    string errorMessage = "Invalid parity value";
                    LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                if (!IsStopBitsValid(stopBits))
                {
                    string errorMessage = "Invalid stop bits value";
                    LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                if (!IsHandshakeValid(handshake))
                {
                    string errorMessage = "Invalid handshake value";
                    LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
                }

                // Create a new SerialPort instance with the specified parameters
                _port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

                // Set the handshake value
                _port.Handshake = handshake;
                _port.NewLine = "\r\n";
                //await Task.Delay(1000);
                // If the port is not open, open it
                if (!_port.IsOpen)
                {
                    _port.Open();
                }

                // Clear the input buffer
                _port.DiscardInBuffer();

                // Send a command to the instrument
                //string command = "YOUR_COMMAND";
                _port.WriteLine(Command);

                // Set a timeout for the read operation
                var timeout = TimeSpan.FromSeconds(5); // Set timeout to 5 seconds
                using (var timeoutCts = new CancellationTokenSource(timeout))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
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
                            string successMessage = "Command successfully completed \nReceived response: " + data.Trim();
                            LogInformation(successMessage);
                            return successMessage;
                        }
                        else if (data.Contains("\x15"))
                        {
                            // NAK received, error with the command string
                            string errorMessage = "Error: Invalid command string \nReceived response: " + data.Trim();
                            LogError(errorMessage);
                            return errorMessage;
                        }
                        else
                        {
                            // Other response received, handle as needed
                            string responseMessage = "Received response: " + data.Trim();
                            LogInformation(responseMessage);
                            return responseMessage;
                        }
                    }
                    else
                    {
                        // If the read operation was cancelled due to a timeout, throw a TimeoutException
                        string errorMessage = $"Timeout while reading from port {portName}";
                        LogError(errorMessage);
                        throw new TimeoutException(errorMessage);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                // Log an error message to the Event Viewer if any of the input parameters are invalid
                string errorMessage = $"Invalid parameter value: {ex.Message}";
                LogError(errorMessage);
                throw new ArgumentException(errorMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log an error message to the Event Viewer if access to the port is denied
                string errorMessage = $"Access to port {portName} is denied: {ex.Message}";
                LogError(errorMessage);
                throw new UnauthorizedAccessException(errorMessage);
            }
            catch (TimeoutException ex)
            {
                // Log an error message to the Event Viewer if the read operation times out
                string errorMessage = $"Timeout while reading from port {portName}: {ex.Message}";
                //LogError(errorMessage);
                throw new TimeoutException(errorMessage);
            }
            catch (Exception ex)
            {
                // Log a generic error message to the Event Viewer for any other exceptions that may occur
                string errorMessage = $"Failed to read from the serial port: {ex.Message}";
                LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            finally
            {
                // Close and dispose of the SerialPort instance
                _port?.Close();
                _port?.Dispose();
            }
        }

        // This method logs an information message to the Event Viewer
        public void LogInformation(string message)
        {
            _systemEventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        // This method logs an error message to the Event Viewer
        public void LogError(string message)
        {
            _systemEventLog.WriteEntry(message, EventLogEntryType.Error);
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
