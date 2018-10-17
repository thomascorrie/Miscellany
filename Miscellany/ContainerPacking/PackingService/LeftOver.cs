using System;
using System.Collections.Generic;
using System.Linq;
using CromulentBisgetti.ContainerPacking.Entities; //added to interact with ContainerPacking
using Autodesk.DesignScript.Runtime;

namespace Miscellany.ContainerPacking
{
    /// <summary>
    /// LeftOver space from Item placing
    /// </summary>
    internal class LeftOver //Internal class to hide from Dynamo
    {
        public LeftOver(double width, double depth, double height, double x, double y, double z)
        {
            Width = width;
            Depth = depth;
            Height = height;
            X = x;
            Y = y;
            Z = z;
            Volume = width * depth * height;
        }
        
        public double Width { get; set; }
        public double Depth { get; set; }
        public double Height { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Volume { get; set; }

    }
}
