# Serial Port Interface and Classes
The Serial Port Interface and Classes provide a set of components for reading data from a serial port in C#.

## Interface: ISerialPortReader
The **ISerialPortReader** interface defines the contract for reading data from a serial port. It includes the following methods and properties:

* **ReadAsync**: Reads data asynchronously from a specified serial port.
* **IsPortNameValid**: Checks if the specified port name is valid.
* **IsBaudRateValid**: Checks if the specified baud rate is valid.
* **IsParityValid**: Checks if the specified parity mode is valid.
* **IsStopBitsValid**: Checks if the specified stop bits value is valid.
* **IsHandshakeValid**: Checks if the specified handshake protocol is valid.

## Class: SerialPortReader
The **SerialPortReader** class implements the **ISerialPortReader** interface and provides the functionality to read data from a serial port. It includes the following methods and properties:

* **ReadAsync**: Reads data asynchronously from a serial port using the specified parameters.
* **IsPortNameValid**: Checks if the specified port name is valid.
* **IsBaudRateValid**: Checks if the specified baud rate is valid.
* **IsParityValid**: Checks if the specified parity mode is valid.
* **IsStopBitsValid**: Checks if the specified stop bits value is valid.
* **IsHandshakeValid**: Checks if the specified handshake protocol is valid.

## Wrapper Class: SerialPortDataReader
The **SerialPortDataReader** class is a wrapper class that provides a simplified way to read data from a serial port using the **SerialPortReader** class. It includes the following method:

* **GetData**: Gets data from a specified serial port using the SerialPortReader class. It takes the serial port parameters and a reading timeout as input, and returns the received data as a string.

## Usage

1. Include the **SerialPortInterface** and **SerialPortConnect** namespaces in your code file.
2. Create an instance of the **SerialPortDataReader** class.
3. Call the **GetData** method on the instance, providing the serial port parameters and reading timeout.
4. The method will return the received data as a string.

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
