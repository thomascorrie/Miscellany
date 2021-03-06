﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Algorithms;
using CromulentBisgetti.ContainerPacking.Entities;

namespace Miscellany.ContainerPacking.Entities
{
    /// <summary>
    /// An item to be packed. Also used to hold post-packing details for the item.
    /// </summary>
    public class Item
    {
        //CromulentBisgetti.ContainerPacking.Entities.Item i;

        #region Private Variables

        private double volume;

        #endregion Private Variables


        /// <summary>
        ///     Creates a new Item for Container Packing. Based on David Chapman's 3DContainerPacking library.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <param name="dim1">The length of one of the three item dimensions</param>
        /// <param name="dim2">The length of another of the three item dimensions</param>
        /// <param name="dim3">The length of the other of the three item dimensions</param>
        /// <param name="quantity">The item quantity (the default is 1)</param>
        /// <returns name="item">Item object</returns>
        /// <search>
        /// containerpacking
        /// </search>
        public Item(int id, double dim1, double dim2, double dim3, int quantity = 1)
        {
            this.ID = id;
            this.Dim1 = dim1;
            this.Dim2 = dim2;
            this.Dim3 = dim3;
            this.volume = dim1 * dim2 * dim3;
            this.Quantity = quantity;
        }

        #region Public Properties

        /// <summary>
        ///     Gets the item ID.
        /// </summary>
        /// <value>
        /// The item ID.
        /// </value>
        //[DataMember]
        public int ID { get; set; }

        /// <summary>
        ///     Indicates whether this item has already been packed.
        /// </summary>
        /// <value>
        ///   True if the item has already been packed; otherwise, false.
        /// </value>
        //[DataMember]
        public bool IsPacked { get; set; }

        /// <summary>
        ///     Gets the length of one of the item dimensions.
        /// </summary>
        /// <value>
        /// The first item dimension.
        /// </value>
        //[DataMember]
        public double Dim1 { get; set; }

        /// <summary>
        ///     Gets the length another of the item dimensions.
        /// </summary>
        /// <value>
        /// The second item dimension.
        /// </value>
        //[DataMember]
        public double Dim2 { get; set; }

        /// <summary>
        ///     Gets the third of the item dimensions.
        /// </summary>
        /// <value>
        /// The third item dimension.
        /// </value>
        //[DataMember]
        public double Dim3 { get; set; }

        /// <summary>
        ///     Gets the x coordinate of the location of the packed item within the container.
        /// </summary>
        /// <value>
        /// The x coordinate of the location of the packed item within the container.
        /// </value>
        //[DataMember]
        public double CoordX { get; set; }

        /// <summary>
        ///     Gets the y coordinate of the location of the packed item within the container.
        /// </summary>
        /// <value>
        /// The y coordinate of the location of the packed item within the container.
        /// </value>
        //[DataMember]
        public double CoordY { get; set; }

        /// <summary>
        ///     Gets the z coordinate of the location of the packed item within the container.
        /// </summary>
        /// <value>
        /// The z coordinate of the location of the packed item within the container.
        /// </value>
        //[DataMember]
        public double CoordZ { get; set; }

        /// <summary>
        ///     Gets the item quantity.
        /// </summary>
        /// <value>
        /// The item quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        ///     Gets the x dimension of the orientation of the item as it has been packed.
        /// </summary>
        /// <value>
        /// The x dimension of the orientation of the item as it has been packed.
        /// </value>
        //[DataMember]
        public double PackDimX { get; set; }

        /// <summary>
        ///     Gets the y dimension of the orientation of the item as it has been packed.
        /// </summary>
        /// <value>
        /// The y dimension of the orientation of the item as it has been packed.
        /// </value>
        //[DataMember]
        public double PackDimY { get; set; }

        /// <summary>
        ///     Gets the z dimension of the orientation of the item as it has been packed.
        /// </summary>
        /// <value>
        /// The z dimension of the orientation of the item as it has been packed.
        /// </value>
        //[DataMember]
        public double PackDimZ { get; set; }

        /// <summary>
        ///     Gets the item volume.
        /// </summary>
        /// <value>
        /// The item volume.
        /// </value>
        //[DataMember]
        public double Volume
        {
            get
            {
                return volume;
            }
        }

        #endregion Public Properties
    }
}
