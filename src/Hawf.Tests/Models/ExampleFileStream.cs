using System;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Hawf.Tests.Models;

public class ExampleFileStream : FileStream
{
    public ExampleFileStream() : base("/", FileMode.Open)
    {
    }
}