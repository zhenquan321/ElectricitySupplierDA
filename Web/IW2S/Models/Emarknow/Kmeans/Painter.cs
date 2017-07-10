using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW2S
{
    public sealed class Painter
    {

        public static void Draw(IDrawable obj, Object surface)
        {
            obj.Draw(surface);
        }
    }
}
