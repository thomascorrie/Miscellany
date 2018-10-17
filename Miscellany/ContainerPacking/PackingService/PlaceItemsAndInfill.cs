using System;
using System.Collections.Generic;
using System.Linq;
using CromulentBisgetti.ContainerPacking.Entities; //added to interact with ContainerPacking
using Autodesk.DesignScript.Runtime;
using System.Text;

namespace Miscellany.ContainerPacking
{
    /// <summary>
    /// PackingService
    /// </summary>
    public static partial class PackingService
    {

        #region PlaceItemsAndInfill

        /// <summary>
        /// Intended for packing books on shelves. The books are first placed vertically on a shelf and then the remaining space is packed using the chosen packing algorithm. When the number of Items in a group dwindles below the minimum number, the difference is made up with Items taken from the next group. It aims to solve optimally at each container in turn and is not globally optimal. The default algorithm (1) is EB-AFIT from David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="containers">Containers in order</param>
        /// <param name="itemsToPack">Items to pack</param>
        /// <param name="algorithm">Algorithm ID</param>
        /// <param name="minimumItems">The minimum number of items to consider for a container</param>
        /// <param name="maximumGap">The maximum gap to ignore at the end of a shelf</param>
        /// <returns name="packedItems">Items that were successfully packed</returns>
        /// <returns name="unpackedItems">Items that were not packed</returns>
        /// <returns name="isCompletePack">Are all items packed?</returns>
        /// <returns name="packTimeInMilliseconds">Total pack time</returns>
        /// <returns name="totalPackTimeInMilliseconds">Pack time per container</returns>
        /// <returns name="percentContainerVolumePacked">Percentage of the container that is packed</returns>
        /// <returns name="percentItemVolumePacked">Percentage of items packed</returns>
        /// <search>
        /// containerpacking
        /// </search>
        [MultiReturn(new[] { "packedItems", "unpackedItems", "isCompletePack", "packTimeInMilliseconds", "totalPackTimeInMilliseconds", "percentContainerVolumePacked", "percentItemVolumePacked", "report", "vLeftOver" })]
        public static Dictionary<string, object> PlaceItemsAndInfill(List<Miscellany.ContainerPacking.Entities.Container> containers, List<List<Miscellany.ContainerPacking.Entities.Item>> itemsToPack, int algorithm = 1, int minimumItems = 20, double maximumGap = 1)
        {
            List<string> report = new List<string>();
            //Select algorithm using integer
            List<int> algorithms = new List<int> { algorithm };

            //Sequence for packed Items
            int seq = 1;

            //Create CromulentBisgetti Items, retaining the nested structure
            List<List<Item>> items = new List<List<Item>>();
            foreach (List<Miscellany.ContainerPacking.Entities.Item> l in itemsToPack)
            {
                List<Item> subList = new List<Item>();
                foreach (Miscellany.ContainerPacking.Entities.Item i in l)
                {
                    Item cbItem = ItemToCB(i);
                    subList.Add(cbItem);
                }
                items.Add(subList);
            }

            //Reverse List to start from back so removals don't cause problems to indexing
            items.Reverse();

            //Output lists
            List<List<Miscellany.ContainerPacking.Entities.Item>> itemsPacked = new List<List<Miscellany.ContainerPacking.Entities.Item>>();
            bool IsCompletePack = false;
            List<int> PackTimeInMilliseconds = new List<int>();
            int TotalPackTimeInMilliseconds = 0;
            List<double> PercentContainerVolumePacked = new List<double>();
            List<double> PercentItemVolumePacked = new List<double>();

            //Temp
            List<LeftOver> vls = new List<LeftOver>();

            //Items Count
            int currentPackGroup = items.Count - 1; //Set to last index

            //Temporary group of Items to pack
            List<Item> tempItemsToPack = new List<Item>();

            //Loop through the containers
            foreach (Miscellany.ContainerPacking.Entities.Container container in containers)
            {
                StringBuilder s = new StringBuilder();
                //No more pack groups to consider
                //if (currentPackGroup < 0) { break; }

                //How many Items in temp list?
                int tempCount = tempItemsToPack.Count;
                s.AppendLine("Start: " + tempCount);

                //Ensure count matches the number of groups remaining
                //currentPackGroup = items.Count - 1;

                if (tempCount >= minimumItems)
                {
                    //There are more Items than the minimum or the same
                }
                else if (items.Count == 0)
                {
                    //No more Items in further groups
                    //There are fewer Items than the minimum but there are no more Items to draw on
                    if (tempCount == 0)
                    {
                        //No Items left so break the loop
                        break;
                    }
                }
                else if (currentPackGroup < 0 || (currentPackGroup == 0 && items[currentPackGroup].Count == 0))
                {
                    //There are fewer Items than the minimum but there are no more Items to draw on
                    if (tempCount == 0)
                    {
                        //No Items left so break the loop
                        break;
                    }
                }
                else
                {
                    //There are fewer Items than the minimum but there are further Items to use
                    //int whileCount = 0;
                    while (tempCount < minimumItems && (items.Count > 0 && items[0].Count > 0))
                    {
                        //whileCount++;
                        //if (whileCount > 10)
                        //{
                            //continue; //temp
                        //}
                        if (currentPackGroup < 0)
                        {
                            break;
                        }
                        if (items[currentPackGroup].Count == 0)
                        {
                            //No items left, move onto next group for next loop
                            if (currentPackGroup > 0)
                            {
                                items.RemoveAt(currentPackGroup);
                                currentPackGroup--;
                            }
                            else if (currentPackGroup == 0 && items.Count > 0)
                            {
                                items.RemoveAt(currentPackGroup);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (items[currentPackGroup].Count <= (minimumItems - tempCount))
                        {
                            //Fewer Items are available than desired
                            tempItemsToPack.AddRange(items[currentPackGroup]); //Add all Items
                            items.RemoveAt(currentPackGroup); //Remove all items from the group
                            currentPackGroup--; //Move to the next group index for the next loop
                            tempCount = tempItemsToPack.Count; //Set the cou
                        }
                        else if (items[currentPackGroup].Count > (minimumItems - tempCount))
                        {
                            //The desired number or greater are available
                            int numberToTake = minimumItems - tempCount;
                            if (numberToTake < 1)
                            {
                                continue;
                            }
                            for (int i = numberToTake - 1; i >= 0; i--)
                            {
                                //Take each required Item and remove from list
                                tempItemsToPack.Add(items[currentPackGroup][i]);
                                items[currentPackGroup].RemoveAt(i);
                            }
                            tempCount = tempItemsToPack.Count; //Report the current size of the temporary group
                        }
                        //tempCount++;
                    }
                }

                //LeftOver Spaces
                List<LeftOver> vLeftOver = new List<LeftOver>();
                List<LeftOver> hLeftOver = new List<LeftOver>();

                //Place Items
                List<Item> placedItems = new List<Item>();
                double x = container.Length; //Set the width to place
                double y = container.Width;
                double z = container.Height;
                double remainingX = x;
                tempItemsToPack.Reverse(); //Reverse the list of Items
                int count = tempItemsToPack.Count;
                for (int i = count - 1; i >= 0; i--) //Start at the end of the list
                {
                    if (remainingX < maximumGap)
                    {
                        break; //Remaining space is less than 1
                    }
                    double bx = Miscellany.Math.Functions.ToDouble(tempItemsToPack[i].Dim1);
                    double by = Miscellany.Math.Functions.ToDouble(tempItemsToPack[i].Dim2);
                    double bz = Miscellany.Math.Functions.ToDouble(tempItemsToPack[i].Dim3);
                    if (bz < z && by < y && bx < remainingX)
                    {
                        //Item can be placed

                        //Vertical Leftover Space
                        LeftOver vL = new LeftOver(remainingX - bx, y - by, z, x - remainingX, by, 0);
                        for (int j = vLeftOver.Count - 1; j >= 0; j--)
                        {
                            if (vL.Depth < vLeftOver[j].Depth)
                            {
                                //Reset depth and position to narrowest point
                                vLeftOver[j].Depth = vL.Depth;
                                vLeftOver[j].Y = vL.Y;
                            }
                        }
                        vLeftOver.Add(vL);

                        //Horizontal Leftover Space
                        LeftOver hL = new LeftOver(remainingX - bx, y, z - bz, x - remainingX, 0, bz);
                        for (int j = hLeftOver.Count - 1; j >= 0; j--)
                        {
                            if (hL.Height < hLeftOver[j].Height)
                            {
                                //Reset depth and position to narrowest point
                                hLeftOver[j].Height = hL.Height;
                                hLeftOver[j].Z = hL.Z;
                            }
                        }
                        hLeftOver.Add(hL);

                        tempItemsToPack[i].IsPacked = true;
                        tempItemsToPack[i].PackDimX = tempItemsToPack[i].Dim1;
                        tempItemsToPack[i].PackDimY = tempItemsToPack[i].Dim3; //----NEED TO CHECK ORIENTATION
                        tempItemsToPack[i].PackDimZ = tempItemsToPack[i].Dim2; //----NEED TO CHECK ORIENTATION
                        tempItemsToPack[i].CoordX = Miscellany.Math.Functions.ToDecimal(x - remainingX);
                        tempItemsToPack[i].CoordY = Miscellany.Math.Functions.ToDecimal(0);
                        tempItemsToPack[i].CoordZ = Miscellany.Math.Functions.ToDecimal(0);
                        //Add to packed list
                        placedItems.Add(tempItemsToPack[i]);
                        remainingX = remainingX - bx; //Remaining shelf width for next loop
                        tempItemsToPack.RemoveAt(i); //Remove Item from list
                    }
                }
                s.AppendLine("tempItemsToPack after place: " + tempItemsToPack.Count);
                s.AppendLine("Placed Items: " + placedItems.Count);
                s.AppendLine("Count: " + vLeftOver.Count);

                //Loop through vertical volumes and select largest
                double vVolMax = 0;
                LeftOver vLeftOverMax = vLeftOver[0];
                foreach (LeftOver v in vLeftOver)
                {
                    s.AppendLine(v.Width + " / " + v.Depth + " / " + v.Height + " / " + v.Volume);
                    if (v.Volume > vVolMax)
                    {
                        vLeftOverMax = v;
                        vVolMax = v.Volume;
                    }
                }

                //Loop through horizontal volumes and select largest
                double hVolMax = 0;
                LeftOver hLeftOverMax = hLeftOver[0];
                foreach (LeftOver h in hLeftOver)
                {
                    if (h.Volume > hVolMax)
                    {
                        hLeftOverMax = h;
                        hVolMax = h.Volume;
                    }
                }

                //Compare volumes for leftover space and choose the largest to take priority
                if (vLeftOverMax.Volume > hLeftOverMax.Volume)
                {
                    hLeftOverMax.Depth = y - vLeftOverMax.Depth;
                }
                else
                {
                    vLeftOverMax.Height = z - hLeftOverMax.Height;
                }

                //vLeftOverMax = new LeftOver(x,y,z,0,0,0);
                //vls.Add(vLeftOverMax);
                //s.AppendLine(vLeftOverMax.Width + " / " + vLeftOverMax.Depth + " / " + vLeftOverMax.Height);


                if (tempItemsToPack.Count > 0)
                {
                    //Vertical Packing
                    //Create CB Container
                    Container con = new Container(10 + container.ID, Miscellany.Math.Functions.ToDecimal(vLeftOverMax.Width), Miscellany.Math.Functions.ToDecimal(vLeftOverMax.Depth), Miscellany.Math.Functions.ToDecimal(vLeftOverMax.Height));
                    //Create CromulentBisgetti Container
                    List<Container> cons = new List<Container> { con };
                    //Get container packing result
                    ContainerPackingResult containerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(cons, tempItemsToPack, algorithms).FirstOrDefault();
                    //Get the single algorithm packing result from the container packing result
                    AlgorithmPackingResult algorithmPackingResult = containerPackingResult.AlgorithmPackingResults.FirstOrDefault();
                    List<Item> vPackedItems = new List<Item>();
                    foreach (Item v in algorithmPackingResult.PackedItems)
                    {
                        v.CoordX = v.CoordX + Miscellany.Math.Functions.ToDecimal(vLeftOverMax.X);
                        v.CoordY = v.CoordY + Miscellany.Math.Functions.ToDecimal(vLeftOverMax.Z); //----NEED TO CHECK ORIENTATION
                        v.CoordZ = v.CoordZ + Miscellany.Math.Functions.ToDecimal(vLeftOverMax.Y); //----NEED TO CHECK ORIENTATION
                        vPackedItems.Add(v);
                    }

                    IsCompletePack = algorithmPackingResult.IsCompletePack;
                    if (IsCompletePack) //If all the items from that group are packed
                    {
                        tempItemsToPack.Clear(); //Clear all Items from temp list
                    }
                    else
                    {
                        tempItemsToPack = algorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                    }
                    placedItems.AddRange(vPackedItems);
                    s.AppendLine("tempItemsToPack after v: " + tempItemsToPack.Count);
                    s.AppendLine("vPackedItems: " + vPackedItems.Count);
                    s.AppendLine("PlacedItems: " + placedItems.Count);
                }


                if (tempItemsToPack.Count > 0)
                {
                    //Horizontal Packing
                    //Create CB Container
                    Container hcon = new Container(10000 + container.ID, Miscellany.Math.Functions.ToDecimal(hLeftOverMax.Width), Miscellany.Math.Functions.ToDecimal(hLeftOverMax.Depth), Miscellany.Math.Functions.ToDecimal(hLeftOverMax.Height));
                    //Create CromulentBisgetti Container
                    List<Container> hcons = new List<Container> { hcon };
                    //Get container packing result
                    ContainerPackingResult hcontainerPackingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(hcons, tempItemsToPack, algorithms).FirstOrDefault();
                    //Get the single algorithm packing result from the container packing result
                    AlgorithmPackingResult halgorithmPackingResult = hcontainerPackingResult.AlgorithmPackingResults.FirstOrDefault();
                    List<Item> hPackedItems = new List<Item>();
                    foreach (Item h in halgorithmPackingResult.PackedItems)
                    {
                        h.CoordX = h.CoordX + Miscellany.Math.Functions.ToDecimal(hLeftOverMax.X);
                        h.CoordY = h.CoordY + Miscellany.Math.Functions.ToDecimal(hLeftOverMax.Z); //----NEED TO CHECK ORIENTATION
                        h.CoordZ = h.CoordZ + Miscellany.Math.Functions.ToDecimal(hLeftOverMax.Y); //----NEED TO CHECK ORIENTATION
                        hPackedItems.Add(h);
                    }

                    IsCompletePack = halgorithmPackingResult.IsCompletePack;
                    if (IsCompletePack) //If all the items from that group are packed
                    {
                        tempItemsToPack.Clear(); //Clear all Items from temp list
                    }
                    else
                    {
                        tempItemsToPack = halgorithmPackingResult.UnpackedItems; //items is set to unpacked items for next loop
                    }

                    placedItems.AddRange(hPackedItems);
                    s.AppendLine("tempItemsToPack after h: " + tempItemsToPack.Count);
                    s.AppendLine("hPackedItems: " + hPackedItems.Count);
                    s.AppendLine("PlacedItems: " + placedItems.Count);
                }

                if (tempItemsToPack.Count > 0)
                {
                    tempItemsToPack.Reverse();
                }

                //Packed Items
                List<Miscellany.ContainerPacking.Entities.Item> itemsPackedPass = new List<Miscellany.ContainerPacking.Entities.Item>();
                foreach (Item i in placedItems)
                {
                    Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                    mItem.Sequence = seq;
                    seq++;
                    itemsPackedPass.Add(mItem);
                }

                itemsPacked.Add(itemsPackedPass);

                //PackTimeInMilliseconds.Add(Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds));
                //TotalPackTimeInMilliseconds += Convert.ToInt32(algorithmPackingResult.PackTimeInMilliseconds);
                //PercentContainerVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentContainerVolumePacked));
                //PercentItemVolumePacked.Add(Miscellany.Math.Functions.ToDouble(algorithmPackingResult.PercentItemVolumePacked));
                s.AppendLine("tempItemsToPack at end of the loop: " + tempItemsToPack.Count);
                s.AppendLine("current pack group: " + currentPackGroup);
                report.Add(s.ToString());

            }

            //Convert CromulentBisgetti items to Miscellany Items for Unpacked Items
            List<Miscellany.ContainerPacking.Entities.Item> itemsUnpacked = new List<Miscellany.ContainerPacking.Entities.Item>();
            foreach (List<Item> l in items)
            {
                foreach (Item i in l)
                {
                    Miscellany.ContainerPacking.Entities.Item mItem = ItemToMiscellany(i);
                    itemsUnpacked.Add(mItem);
                }
            }

            //Return values
            var d = new Dictionary<string, object>();
            d.Add("packedItems", itemsPacked);
            d.Add("unpackedItems", itemsUnpacked);
            d.Add("isCompletePack", IsCompletePack);
            d.Add("packTimeInMilliseconds", PackTimeInMilliseconds);
            d.Add("totalPackTimeInMilliseconds", TotalPackTimeInMilliseconds);
            d.Add("percentContainerVolumePacked", PercentContainerVolumePacked);
            d.Add("percentItemVolumePacked", PercentItemVolumePacked);
            d.Add("report", report);
            d.Add("vLeftOver", vls);
            return d;
        }
        #endregion

    }
}
