using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miscellany
{
    /// <summary>
    /// About
    /// </summary>
    public static class About
    {
        /// <summary>
        ///     About Miscellany. Only to force the logo to be default icon.
        /// </summary>
        /// <returns name="About">Miscellany is a package for Dynamo 2</returns>
        /// <search>
        /// Miscellany
        /// </search>
        public static string Miscellany()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            return "This is version " + version + " of Miscellany";
        }
    }
}
