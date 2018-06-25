using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary2
{
    public enum LineStinkerModes
    {
        UnInitalized=0,
        StartAtNearVertex =1,
        StartAtFarVertex =2,
        FloatBetweenVerticies =3,
        TestHighValue=10
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
