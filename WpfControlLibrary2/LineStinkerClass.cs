using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary2
{
    public enum LineStinkerModes
    {
        StartAtNearVertex,
        StartAtFarVertex,
        FloatBetweenVerticies
    }

    public class LineStinkerClass
    {
        private LineStinkerModes stinky;

        public LineStinkerModes Stinky
        {
            get
            {
                return this.stinky;
            }

            set
            {
                this.stinky = value;
            }
        }

     }
}
