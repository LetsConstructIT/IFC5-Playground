using System;
using System.Collections.Generic;
using System.Text;

namespace IFC5Tekla.Engine;
internal class Program
{
    public static void Main(string[] args)
    {
        var path = @"C:\Users\grzeg\Documents\IFC5\hello-wall.ifcx";

        new Ifc5Reader().Read(path);
    }
}
