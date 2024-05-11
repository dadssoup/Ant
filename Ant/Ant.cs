using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ant
{
    public class AntClass
    {

        AntState state = AntState.SearchForLeaves;
        public System.Drawing.Point position = new System.Drawing.Point(0, 0);
        System.Windows.Shapes.Rectangle rect;
        public System.Windows.Shapes.Rectangle Rect 
        { 
            get { return rect; }
            set { rect = value; }
        }
        public AntClass(System.Drawing.Point position)
        {
            
            this.position = position;
            rect = new();
            rect.Width = 16;
            rect.Height = 8;
            rect.Fill = Brushes.DarkRed;

        }

        
    }
    public enum AntState
    {
        SearchForLeaves,
        MoveLeafToHome,
        Run
    }
}
