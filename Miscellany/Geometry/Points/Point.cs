using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Decompositions;
using Autodesk.DesignScript.Runtime;
using DS = Autodesk.DesignScript.Geometry;

namespace Miscellany.Geometry.Points
{
    /// <summary>
    /// Point
    /// </summary>
    public static class Point
    {
        /// <summary>
        ///     Finds the optimal translation and rotation between two sets of paired points using the Kabsch Algorithm.
        /// </summary>
        /// <param name="points">List of points to match</param>
        /// <param name="pointsToMove">List of points to translate and rotate to best match points</param>
        /// <returns name="points">Translated and rotated points</returns>
        /// <search>
        /// kabsch, translation, rotation
        /// </search>
        public static List<DS.Point> KabschAlgorithm(List<DS.Point> points, List<DS.Point> pointsToMove)
        {
            if (points.Count != pointsToMove.Count)
            {
                return null;
            }
            
            double x1 = 0;
            double x2 = 0;
            double y1 = 0;
            double y2 = 0;
            double z1 = 0;
            double z2 = 0;

            foreach (DS.Point p in points)
            {
                x1 += p.X;
                y1 += p.Y;
                z1 += p.Z;
            }

            foreach (DS.Point p in pointsToMove)
            {
                x2 += p.X;
                y2 += p.Y;
                z2 += p.Z;
            }
            
            List<DS.Point> pOut = new List<DS.Point>();
            var p1 = DS.Point.ByCoordinates(x1 / points.Count, y1 / points.Count, z1 / points.Count);
            pOut.Add(p1);
            var p2 = DS.Point.ByCoordinates(x2 / pointsToMove.Count, y2 / pointsToMove.Count, z2 / pointsToMove.Count);
            pOut.Add(p2);
            
            //Dispose of non-returned geometry
            //pt.Dispose();

            //Return values
            return pOut;
        }
    }
}
