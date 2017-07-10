using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;



namespace IW2S
{
 
    public class Centroid
    {
        public Centroid(float x, float y, Color color)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
        }

        public Color Color { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
    }
    
    public sealed class K_Means: IDrawable
    {
        public  List<Point> points;
        public  List<Centroid> centroids;
        public  UInt32 pointsCount;
        public  UInt32 clustersCount;

        public K_Means()
        {
            pointsCount = 0;
            clustersCount = 0;
            points = new List<Point>();
            centroids = new List<Centroid>();
        }

        public K_Means(UInt32 pointsCount, UInt32 clustersCount) : this()
        {
            this.clustersCount = clustersCount;
            this.pointsCount = pointsCount;
        }

        private Centroid GetMeanCentroid(List<Point> points)
        {
            float X_mean = 0;
            float Y_mean = 0;

            foreach (Point point in points)
            {
                X_mean += point.X;
                Y_mean += point.Y;
            }

            X_mean /= points.Count;
            Y_mean /= points.Count;
            return new Centroid(X_mean, Y_mean, Color.Green);
        }

        public void Randomize()
        {
            if (clustersCount >= pointsCount) throw new Exception("A number of points must be bigger then a number of clusters!");

            Random rand = new Random();
            for (var i = 0; i < pointsCount; i++)
                points.Add(new Point(rand.Next(10, 768-10), rand.Next(10, 500-10)));

            //let the centoids be the 1st clustersCount points as they are random all the same
            for (var i = 0; i < clustersCount; i++)
                centroids.Add(new Centroid(points[i].X, points[i].Y, Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
        }

        public void Draw(Object obj)
        {
            var panel = obj as Bitmap;
          //  if (panel == null) return;

            Graphics gr = Graphics.FromImage(panel);
            
            //var gr = panel.CreateGraphics();
            //gr.Clear(panel.BackColor);

            foreach (var point in points)
                gr.FillRectangle(new SolidBrush(Color.Black), point.X, point.Y, 4, 4);

            foreach (var centroid in centroids)
                gr.FillEllipse(new SolidBrush(centroid.Color), new Rectangle((int)centroid.X - 4, (int)centroid.Y + 4, 8, 8 ));
            
        }

    }
}
