﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IFC5.Reader.UnitTests
{
    [TestClass]
    public class SampleUsage
    {
        [TestMethod]
        public void ReadingFromFile()
        {
            var path = @"C:\Users\grzeg\Documents\IFC5\hello-wall.ifcx";

            var composedObjects = new IFC5.Reader.Reader().Read(path);
        }
    }
}
