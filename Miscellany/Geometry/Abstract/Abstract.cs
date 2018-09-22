using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;

namespace Miscellany.Geometry.Abstract
{
    /// <summary>
    /// CoordinateSystem
    /// </summary>
    public static class CoordinateSystem
    {
        /// <summary>
        /// Shows scalable lines representing the CoordinateSystem axes and rectangles for the planes
        /// </summary>
        /// <param name="coordinateSystem">Autodesk.DesignScript.Geometry.CoordinateSystem</param>
        /// <param name="length">double</param>
        /// <returns name="Display">GeometryColor</returns>
        /// <returns name="Origin">Point</returns>
        /// <returns name="XAxis">Vector</returns>
        /// <returns name="YAxis">Vector</returns>
        /// <returns name="ZAxis">Vector</returns>
        /// <returns name="XYPlane">Plane</returns>
        /// <returns name="YZPlane">Plane</returns>
        /// <returns name="ZXPlane">Plane</returns>
        [MultiReturn(new[] { "Display", "Origin", "XAxis", "YAxis", "ZAxis", "XYPlane", "YZPlane", "ZXPlane" })]
        public static Dictionary<string, object> Display(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, double length = 1000)
        {
            //Avoid zero length
            if (length == 0)
            {
                length = 1;
            }
            //Origin and Axes
            var pt = coordinateSystem.Origin;
            var lineX = Line.ByStartPointDirectionLength(pt, coordinateSystem.XAxis, length);
            var colorX = DSCore.Color.ByARGB(255, 255, 0, 0);
            var lineY = Line.ByStartPointDirectionLength(pt, coordinateSystem.YAxis, length);
            var colorY = DSCore.Color.ByARGB(255, 0, 255, 0);
            var lineZ = Line.ByStartPointDirectionLength(pt, coordinateSystem.ZAxis, length);
            var colorZ = DSCore.Color.ByARGB(255, 0, 0, 255);
            //Build List of Axes
            List<Modifiers.GeometryColor> display = new List<Modifiers.GeometryColor>();
            display.Add(Modifiers.GeometryColor.ByGeometryColor(lineX, colorX));
            display.Add(Modifiers.GeometryColor.ByGeometryColor(lineY, colorY));
            display.Add(Modifiers.GeometryColor.ByGeometryColor(lineZ, colorZ));

            //Dispose of non-returned geometry
            //pt.Dispose();

            //Return values
            var d = new Dictionary<string, object>();
            d.Add("Display", display);
            d.Add("Origin", pt);
            d.Add("XAxis", coordinateSystem.XAxis);
            d.Add("YAxis", coordinateSystem.YAxis);
            d.Add("ZAxis", coordinateSystem.ZAxis);
            d.Add("XYPlane", coordinateSystem.XYPlane);
            d.Add("YZPlane", coordinateSystem.YZPlane);
            d.Add("ZXPlane", coordinateSystem.ZXPlane);
            return d;
        }
    }

    /// <summary>
    /// Plane
    /// </summary>
    public static class Plane
    {
        /// <summary>
        /// Shows scalable lines representing the axes and a rectangle for the Plane
        /// </summary>
        /// <param name="plane">Autodesk.DesignScript.Geometry.Plane</param>
        /// <param name="length">double</param>
        /// <returns name="Display">GeometryColor</returns>
        /// <returns name="Origin">Point</returns>
        /// <returns name="XAxis">Vector</returns>
        /// <returns name="YAxis">Vector</returns>
        /// <returns name="Normal">Vector</returns>
        [MultiReturn(new[] { "Display", "Origin", "XAxis", "YAxis", "Normal" })]
        public static Dictionary<string, object> Display(Autodesk.DesignScript.Geometry.Plane plane, double length = 1000)
        {
            //Avoid zero length
            if (length == 0)
            {
                length = 1;
            }
            //Origin and Axes
            var pt = plane.Origin;
            var lineX = Line.ByStartPointDirectionLength(pt, plane.XAxis, length);
            var colorX = DSCore.Color.ByARGB(255, 255, 0, 0);
            var lineY = Line.ByStartPointDirectionLength(pt, plane.YAxis, length);
            var colorY = DSCore.Color.ByARGB(255, 0, 255, 0);
            var lineN = Line.ByStartPointDirectionLength(pt, plane.Normal, length);
            var colorN = DSCore.Color.ByARGB(255, 0, 0, 255);
            //Plane
            var rect = Rectangle.ByWidthLength(plane, length, length);
            var colorR = DSCore.Color.ByARGB(50, 50, 50, 50);
            //Build List of Display
            List<Modifiers.GeometryColor> display = new List<Modifiers.GeometryColor>();
            display.Add(Modifiers.GeometryColor.ByGeometryColor(lineX, colorX));
            display.Add(Modifiers.GeometryColor.ByGeometryColor(lineY, colorY));
            display.Add(Modifiers.GeometryColor.ByGeometryColor(lineN, colorN));
            display.Add(Modifiers.GeometryColor.ByGeometryColor(rect, colorR));
            //Return values
            var d = new Dictionary<string, object>();
            d.Add("Display", display);
            d.Add("Origin", pt);
            d.Add("XAxis", plane.XAxis);
            d.Add("YAxis", plane.YAxis);
            d.Add("Normal", plane.Normal);
            return d;
        }
    }

    /// <summary>
    /// Vector
    /// </summary>
    public static class Vector
    {
        /// <summary>
        /// Shows a scalable line representing a Vector from a chosen starting point
        /// </summary>
        /// <param name="vector">Autodesk.DesignScript.Geometry.Vector</param>
        /// <param name="startPoint">Autodesk.DesignScript.Geometry.Point</param>
        /// <param name="scale">double</param>
        /// <returns name="Display">GeometryColor</returns>
        /// <returns name="x">double</returns>
        /// <returns name="y">double</returns>
        /// <returns name="z">double</returns>
        /// <returns name="Length">double</returns>
        [MultiReturn(new[] { "Display", "x", "y", "z", "Length" })]
        public static Dictionary<string, object> Display(Autodesk.DesignScript.Geometry.Vector vector, Autodesk.DesignScript.Geometry.Point startPoint, double scale = 1000)
        {
            //Avoid zero length
            if (scale == 0)
            {
                scale = 1;
            }
            //Origin and Axes
            var line = Line.ByStartPointDirectionLength(startPoint, vector, vector.Length * scale);
            var color = DSCore.Color.ByARGB(255, 255, 0, 0);
            //Build List of Display
            List<Modifiers.GeometryColor> display = new List<Modifiers.GeometryColor>();
            display.Add(Modifiers.GeometryColor.ByGeometryColor(line, color));
            //Return values
            var d = new Dictionary<string, object>();
            d.Add("Display", display);
            d.Add("x", vector.X);
            d.Add("y", vector.Y);
            d.Add("z", vector.Z);
            d.Add("Length", vector.Length);
            return d;
        }
    }
}
