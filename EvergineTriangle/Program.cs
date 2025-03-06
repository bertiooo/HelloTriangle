using System;

namespace EvergineTriangle
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            uint width = 1280;
            uint height = 720;

            using (var triangle = new Triangle(width, height))
            {
                triangle.Run();
            }
        }
    }
}
