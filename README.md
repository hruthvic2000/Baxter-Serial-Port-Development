# Serial Port Communication Library (Interface and Classes)
The Serial Port Interface and Classes provide a set of components for reading data from a serial port in C#.

## Interface: ISerialPortReader
The **ISerialPortReader** interface defines the contract for reading data from a serial port. It includes the following methods and properties:

* **Task<string> ReadAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, CancellationToken cancellationToken, string command)**: Reads data from the specified serial port asynchronously and returns it as a string. If any errors occur, it returns null and logs an error message to the Event Viewer.
* **LogInformation**: This method logs an information message to the Event Viewer.
* **LogError**: This method logs an error message to the Event Viewer.
* **IsPortNameValid**: Checks if the specified port name is valid.
* **IsBaudRateValid**: Checks if the specified baud rate is valid.
* **IsParityValid**: Checks if the specified parity mode is valid.
* **IsStopBitsValid**: Checks if the specified stop bits value is valid.
* **IsHandshakeValid**: Checks if the specified handshake protocol is valid.

## SerialPortConnect - Serial Port Communication Library
SerialPortConnect is a C# library that provides a SerialPortReader class for reading data from a serial port asynchronously. It also includes methods for validating port settings such as port name, baud rate, data bits, parity, stop bits, and handshake.

## Class: SerialPortReader
The **SerialPortReader** class implements the **ISerialPortReader** interface and provides the functionality to read data from a serial port. It includes the following methods and properties:

### Methods
* **ReadAsync**: This method reads data from the specified serial port asynchronously and returns it as a string. It accepts parameters such as portName, baudRate, parity, dataBits, stopBits, handshake, cancellationToken, and Command. It validates the input parameters, opens the serial port if not already open, sends a command to the instrument, waits for data to be available on the port, and handles the received response. If any errors occur, it logs an error message and returns null. Finally, it closes and disposes of the SerialPort instance.
* **LogInformation**: This method logs an information message to the Event Viewer.
* **LogError**: This method logs an error message to the Event Viewer.
* **IsPortNameValid**: This method checks if the specified port name is valid by checking if it exists in the list of available port names.
* **IsBaudRateValid**: This method checks if the specified baud rate is valid by ensuring it is greater than 0 and less than or equal to 115200.
* **IsDataBitsValid**: This method checks if the specified data bits value is valid by ensuring it is either 5, 6, 7, or 8.
* **IsParityValid**: This method checks if the specified parity value is valid by ensuring it is one of the valid Parity enum values.
* **IsStopBitsValid**: This method checks if the specified stop bits value is valid by ensuring it is one of the valid StopBits enum values.
* **IsHandshakeValid**: This method checks if the specified handshake value is valid by ensuring it is one of the valid Handshake enum values.

## Wrapper Class: SerialPortDataReader
The **SerialPortDataReader** class is a wrapper class that provides a simplified way to read data from a serial port using the **SerialPortReader** class. It includes the following method:

### Methods
* **GetData**: This method gets data from the specified serial port. It accepts parameters such as portName, baudRate, parity, dataBits, stopBits, handshake, readingTimeOut, and command. It creates an instance of SerialPortReader, locks it for thread safety, and asynchronously reads data from the serial port using the ReadAsync method. It waits for the read operation to complete, disposes of the SerialPortReader instance, and returns the received data as a string.

## Exception Handling and Event Viewer Logging
The SerialPortReader class and its derived classes handle various exceptions that may occur during the serial port read operation. If any errors occur, they log the error messages to the Event Viewer using the LogError method.

The exceptions that are handled include:

* `UnauthorizedAccessException`: Occurs when the application does not have the required permissions to access the serial port.
* `IOException`: Occurs when there is an I/O error while reading from the serial port.
* `TimeoutException`: Occurs when the read operation times out.
The exception handling ensures that the read operation gracefully handles errors and returns null when an error occurs.

## Usage

1. Add references to the SerialPortConnect and System.IO.Ports namespaces in your project.
2. Include the **SerialPortInterface** and **SerialPortConnect** namespaces in your code file.
3. Create an instance of the **SerialPortDataReader** class.
4. Call the **GetData** method on the instance, providing the serial port parameters and reading timeout.
5. The method will attempt to read data from the specified serial port with the provided settings. If successful, it will return the received data as a string. If any errors occur, an error message will be logged, and the method will return null.
6. Handle the received data according to your application's requirements.

Example usage:
```csharp
using SerialPortInterface;
using SerialPortWrapper;

// Create an instance of SerialPortDataReader
var serialPortDataReader = new SerialPortDataReader();

// Specify the serial port parameters
string portName = "COM1";
int baudRate = 9600;
Parity parity = Parity.None;
int dataBits = 8;
StopBits stopBits = StopBits.One;
Handshake handshake = Handshake.None;
int readingTimeout = 5; // in seconds
string command ="Your Command";

// Call the GetData method to read data from the serial port
string data = serialPortDataReader.GetData(portName, baudRate, parity, dataBits, stopBits, handshake, readingTimeout, command);

// Process the received data
Console.WriteLine("Received data: " + data);
```
Make sure to replace the serial port parameters with the appropriate values for your specific use case.
