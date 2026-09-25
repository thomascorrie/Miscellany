using System;
using System.Collections.Generic;
using System.Linq;
using CromulentBisgetti.ContainerPacking.Entities; //added to interact with ContainerPacking

namespace Miscellany.ContainerPacking
{
    /// <summary>
    /// PackingService
    /// </summary>
    public static partial class PackingService
    {
        #region Methods

        //Convert a CromulentBisgetti Item to a Miscellany Item
        private static Miscellany.ContainerPacking.Entities.Item ItemToMiscellany(Item i)
        {
            double ddim1 = Miscellany.Maths.ToDouble(i.Dim1);
            double ddim2 = Miscellany.Maths.ToDouble(i.Dim2);
            double ddim3 = Miscellany.Maths.ToDouble(i.Dim3);
            //Create Item
            Miscellany.ContainerPacking.Entities.Item mItem = new Miscellany.ContainerPacking.Entities.Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
            //Add packed information
            mItem.IsPacked = i.IsPacked;
            if (i.IsPacked)
            {
                //Swap Y and Z values to shift from the CromulentBisgetti coordinate system and Dynamo
                mItem.PackDimX = Miscellany.Maths.ToDouble(i.PackDimX);
                mItem.PackDimY = Miscellany.Maths.ToDouble(i.PackDimZ);
                mItem.PackDimZ = Miscellany.Maths.ToDouble(i.PackDimY);
                mItem.CoordX = Miscellany.Maths.ToDouble(i.CoordX);
                mItem.CoordY = Miscellany.Maths.ToDouble(i.CoordZ);
                mItem.CoordZ = Miscellany.Maths.ToDouble(i.CoordY);
            }
            return mItem;
        }

        //Convert Miscellany Container to CromulentBisgetti Container
        private static Container ContainerToCB(Miscellany.ContainerPacking.Entities.Container c)
        {
            decimal dLength = Miscellany.Maths.ToDecimal(c.Length);
            decimal dWidth = Miscellany.Maths.ToDecimal(c.Width);
            decimal dHeight = Miscellany.Maths.ToDecimal(c.Height);
            Container cbContainer = new Container(c.ID, dLength, dWidth, dHeight);
            return cbContainer;
        }

        //Convert Miscellany Item to CromulentBisgetti Item
        private static Item ItemToCB(Miscellany.ContainerPacking.Entities.Item i)
        {
            decimal ddim1 = Miscellany.Maths.ToDecimal(i.Dim1);
            decimal ddim2 = Miscellany.Maths.ToDecimal(i.Dim2);
            decimal ddim3 = Miscellany.Maths.ToDecimal(i.Dim3);
            Item cbItem = new Item(i.ID, ddim1, ddim2, ddim3, i.Quantity);
            return cbItem;
        }

        #endregion
    }
}
