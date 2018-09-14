//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CromulentBisgetti.ContainerPacking;
//using CromulentBisgetti.ContainerPacking.Algorithms;
//using CromulentBisgetti.ContainerPacking.Entities;

//namespace Miscellany.ContainerPacking
//{
//    /// <summary>
//    /// Entities
//    /// </summary>
//    public static class Entities
//    {
//        /// <summary>
//        /// Creates a ContainerPacking Container
//        /// </summary>
//        /// <param name="id">id</param>
//        /// <param name="length">length</param>
//        /// <param name="width">width</param>
//        /// <param name="height">height</param>
//        /// <returns name="container">Container object</returns>
//        /// <search>
//        /// containerpacking
//        /// </search>
//        public static Container Container(int id, double length, double width, double height)
//        {
//            decimal dLength = Miscellany.Math.Functions.ToDecimal(length);
//            decimal dWidth = Miscellany.Math.Functions.ToDecimal(width);
//            decimal dHeight = Miscellany.Math.Functions.ToDecimal(height);
//            return new Container(id, dLength, dWidth, dHeight);
//        }

//        /// <summary>
//        /// Creates a ContainerPacking Item
//        /// </summary>
//        /// <param name="id">id</param>
//        /// <param name="dim1">dimension 1</param>
//        /// <param name="dim2">dimension 2</param>
//        /// <param name="dim3">dimension 3</param>
//        /// <param name="quantity">quantity</param>
//        /// <returns name="item">Item object</returns>
//        /// <search>
//        /// containerpacking
//        /// </search>
//        public static Item Item(int id, double dim1, double dim2, double dim3, int quantity)
//        {
//            decimal ddim1 = Miscellany.Math.Functions.ToDecimal(dim1);
//            decimal ddim2 = Miscellany.Math.Functions.ToDecimal(dim2);
//            decimal ddim3 = Miscellany.Math.Functions.ToDecimal(dim3);
//            return new Item(id, ddim1, ddim2, ddim3, quantity);
//        }
//    }
//}
