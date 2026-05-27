using ArcadeMaker.IDE.Items;
using System.Drawing.Drawing2D;

namespace ArcadeMaker.IDE
{
    public partial class SpriteEditor : Form
    {
        private GameSprite sprite = null;

        public SpriteEditor(GameSprite sprite)
        {
            InitializeComponent();
            this.sprite = sprite;

            // update name box when object is renamed
            sprite.NameChanged += (s, e) =>
            {
                if (!renaming)
                    nameBox.Text = e.newName;
            };

            if (sprite == null)
            {
                throw new ArgumentNullException("sprite");
            }
        }

        private void SetOriginBox(bool init)
        {
            if (sprite.image == null)
                return;
            if (init)
            {
                originXBox.Value = sprite.originX;
                originYBox.Value = sprite.originY;
            }
            else
            {
                if (originXBox.Value > sprite.image.Size.Width)
                {
                    originXBox.Value = sprite.image.Size.Width;
                }
                if (originYBox.Value > sprite.image.Size.Height)
                {
                    originYBox.Value = sprite.image.Size.Height;
                }
            }
            originXBox.Maximum = sprite.image.Size.Width;
            originYBox.Maximum = sprite.image.Size.Height;
        }

        private void SpriteEditor_Load(object sender, EventArgs e)
        {
            nameBox.Text = sprite.name;
            imageBox.Image = sprite.image;
            SetDetails();
            SetOriginBox(init: true);
            preciseMaskBtn.Checked = sprite.preciseMask;
            separateMasksBox.Checked = sprite.separateMask;
            modifiedLbl.Visible = !sprite.maskBounding_auto;
        }

        private void importBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] pathes = openFileDialog.FileNames;
                bool fail = false;
                try
                {
                    sprite.images.Clear();
                    sprite.Import(pathes);
                }
                catch
                {
                    fail = true;
                    MessageBox.Show("Cannot load sprite from the selected file");
                }
                if (!fail)
                {
                    imageBox.Image = sprite.image;
                    SetDetails();
                    SetOriginBox(init: false);
                }
            }
        }

        private void SetDetails()
        {
            int width = 0, height = 0;
            if (sprite.image != null)
            {
                width = sprite.image.Size.Width;
                height = sprite.image.Size.Height;
            }
            detailsLbl.Text = "Width: " + width + "   Height: " + height + "\nNumber of subimages: " + sprite.images.Count;

            if (width > 0 && height > 0)
            {
                if (sprite.maskBounding_auto)
                    SpriteMask.CalculateAutoBounding(sprite, out sprite.maskTop, out sprite.maskRight, out sprite.maskBottom, out sprite.maskLeft);
                else if (sprite.maskBounding_fullImage)
                {
                    sprite.maskTop = 0;
                    sprite.maskRight = width - 1;
                    sprite.maskBottom = height - 1;
                    sprite.maskLeft = 0;
                }
            }
        }

        private bool renaming = false;

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            renaming = true;
            try
            {
                sprite.name = nameBox.Text;
                if (nameBox.BackColor == Color.Red)
                    nameBox.BackColor = Color.White;
            }
            catch
            {
                nameBox.BackColor = Color.Red;
            }
            renaming = false;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            SpriteManager manager = new SpriteManager(sprite);
            manager.HeadImageChanged += (s, image) =>
            {
                imageBox.Image = image;
                SetOriginBox(init: false);
            };
            manager.FormClosed += (s, ea) =>
            {
                SetDetails();
            };
            manager.ShowDialog();
        }

        private void originXBox_ValueChanged(object sender, EventArgs e)
        {
            if (originXBox.Value == 0)
            {
                sprite.originX = (int)originXBox.Value;
                originXBox.BackColor = Color.White;
                return;
            }
            if (originXBox.Value != 0 && (sprite.image == null || originXBox.Value > sprite.image.Size.Width || originXBox.Value < 0))
            {
                originXBox.BackColor = Color.Red;
                return;
            }
            sprite.originX = (int)originXBox.Value;
            imageBox.Invalidate();
        }

        private void originYBox_ValueChanged(object sender, EventArgs e)
        {
            if (originYBox.Value == 0)
            {
                sprite.originY = (int)originYBox.Value;
                originYBox.BackColor = Color.White;
                return;
            }
            if (originYBox.Value != 0 && (sprite.image == null || originYBox.Value > sprite.image.Size.Height || originYBox.Value < 0))
            {
                originYBox.BackColor = Color.Red;
                return;
            }
            sprite.originY = (int)originYBox.Value;
            imageBox.Invalidate();
        }

        private void CenterOriginBtn_Click(object sender, EventArgs e)
        {
            if (sprite.image != null)
            {
                // set origin box, this will also handle the ValueChanged events to set the sprite values
                originXBox.Value = sprite.image.Size.Width / 2;
                originYBox.Value = sprite.image.Size.Height / 2;
            }
        }

        private void imageBox_MouseClick(object sender, MouseEventArgs e)
        {
            // set origin box, this will also handle the ValueChanged events to set the sprite values
            if (e.Location.X <= sprite.image.Size.Width && e.Location.Y <= sprite.image.Size.Height)
            {
                originXBox.Value = e.Location.X;
                originYBox.Value = e.Location.Y;
            }
        }

        private Pen originPen = new Pen(Color.Black);

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            if (sprite.image == null)
                return;
            int startX = sprite.originX;
            int startY = sprite.originY;
            e.Graphics.DrawLine(originPen, startX, 0, startX, sprite.image.Size.Height);
            e.Graphics.DrawLine(originPen, 0, startY, sprite.image.Size.Width, startY);
        }

        private void modifyMaskBtn_Click(object sender, EventArgs e)
        {
            using (SpriteMaskEditor maskEditor = new SpriteMaskEditor(sprite))
            {
                maskEditor.ShowDialog();
                modifiedLbl.Visible = !sprite.maskBounding_auto;
            }
        }

        private void preciseMaskBtn_CheckedChanged(object sender, EventArgs e)
        {
            sprite.preciseMask = preciseMaskBtn.Checked;
        }

        private void separateMasksBox_CheckedChanged(object sender, EventArgs e)
        {
            sprite.separateMask = separateMasksBox.Checked;
        }
    }

    public static class MaskGenerator
    {
        public static GraphicsPath GenerateSingleShapeMask(Bitmap image)
        {
            // Create a path for the opaque pixels in the image
            GraphicsPath path = new GraphicsPath();
            {
                // Create a graphics object
                using (Graphics g = Graphics.FromImage(image))
                {
                    // Loop through the rows and columns of pixels in the image
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            // Get the color of the pixel
                            Color pixelColor = image.GetPixel(x, y);

                            // Check if the pixel is not transparent
                            if (pixelColor.A != 0)
                            {
                                // Move to the first non-transparent pixel in the image
                                if (path.PointCount == 0)
                                {
                                    path.StartFigure();
                                    path.AddLine(x, y, x + 1, y + 1);
                                }

                                // Draw a line to the current pixel
                                else
                                {
                                    path.AddLine(x, y, x + 1, y + 1);
                                }
                            }
                        }
                    }

                    // Close the path
                    path.CloseFigure();

                    // Draw the image onto the graphics object
                    g.DrawImage(image, new Point(0, 0));
                }
            }
            return path;
        }

        public static GraphicsPath GenerateMask(Bitmap image)
        {
            // Create a path for the non-transparent pixels in the image
            GraphicsPath path = new GraphicsPath();

            // Create a graphics object
            using (Graphics g = Graphics.FromImage(image))
            {
                // Loop through the rows and columns of pixels in the image
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // Get the color of the pixel
                        Color pixelColor = image.GetPixel(x, y);

                        // Check if the pixel is not transparent
                        if (pixelColor.A != 0)
                        {
                            // Add a rectangle to the path for this pixel
                            path.AddRectangle(new Rectangle(x, y, 1, 1));
                        }
                    }
                }

                // Create a region from the path
                Region region = new Region(path);

                // Create a graphics path iterator from the region
                GraphicsPathIterator iterator = new GraphicsPathIterator(path);

                // Create a new path to hold the flattened path
                GraphicsPath flattenedPath = new GraphicsPath();

                // Flatten the path and add the result to the flattened path
                while (iterator.NextSubpath(flattenedPath, out bool moreSubpaths) != 0)
                {
                    // Do nothing, we just need to call the NextSubpath method
                }

                // Dispose the objects
                iterator.Dispose();
                region.Dispose();
                return flattenedPath;
            }
            // The flattenedPath now describes the shape of the non-transparent pixels in the image
        }

        public static Point[][] PointsMask(Bitmap image)
        {
            List<List<Point>> segments = new List<List<Point>>();
            List<Point> segment = null;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (image.GetPixel(x, y).A != 0)
                    {
                        if (segment == null)
                        {
                            segment = new List<Point>();
                            segments.Add(segment);
                        }
                        segment.Add(new Point(x, y));
                        break;
                    }
                }
            }

            Point[][] points = new Point[segments.Count][];
            for (int i = 0; i < segments.Count; i++)
            {
                points[i] = segments[i].ToArray();
            }
            return points;
        }

        public static Point[] PointsMask1D(Bitmap image)
        {
            List<Point> points = new List<Point>();

            // top
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (image.GetPixel(x, y).A != 0)
                    {
                        points.Add(new Point(x, y));
                        break;
                    }
                }
            }
            // right
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = image.Width - 1; x >= 0; x--)
                {
                    if (image.GetPixel(x, y).A != 0)
                    {
                        points.Add(new Point(x, y));
                        break;
                    }
                }
            }
            // bottom
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = image.Height - 1; y >= 0; y--)
                {
                    if (image.GetPixel(x, y).A != 0)
                    {
                        points.Add(new Point(x, y));
                        break;
                    }
                }
            }
            // left
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image.GetPixel(x, y).A != 0)
                    {
                        points.Add(new Point(x, y));
                        break;
                    }
                }
            }

            return points.ToArray();
        }

        public static Point[][] PointsMask2D2(Bitmap image)
        {
            int[][] labelMap = new int[image.Width][];
            for (int x = 0; x < image.Width; x++)
            {
                labelMap[x] = new int[image.Height];
            }
            int label = 1;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (image.GetPixel(x, y).A != 0 && labelMap[x][y] == 0)
                    {
                        Queue<Point> queue = new Queue<Point>();
                        queue.Enqueue(new Point(x, y));
                        while (queue.Count > 0)
                        {
                            Point p = queue.Dequeue();
                            if (labelMap[p.X][p.Y] == 0 && image.GetPixel(p.X, p.Y).A != 0)
                            {
                                labelMap[p.X][p.Y] = label;
                                if (p.X > 0) queue.Enqueue(new Point(p.X - 1, p.Y));
                                if (p.X < image.Width - 1) queue.Enqueue(new Point(p.X + 1, p.Y));
                                if (p.Y > 0) queue.Enqueue(new Point(p.X, p.Y - 1));
                                if (p.Y < image.Height - 1) queue.Enqueue(new Point(p.X, p.Y + 1));
                            }
                        }
                        label++;
                    }
                }
            }

            List<Point>[] pointLists = new List<Point>[label];
            for (int i = 0; i < label; i++)
            {
                pointLists[i] = new List<Point>();
            }

            // top
            for (int i = 0; i < label; i++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        if (labelMap[x][y] == i && image.GetPixel(x, y).A != 0)
                        {
                            pointLists[i].Add(new Point(x, y));
                            break;
                        }
                    }
                }
            }

            // right
            for (int i = 0; i < label; i++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = image.Width - 1; x >= 0; x--)
                    {
                        if (labelMap[x][y] == i && image.GetPixel(x, y).A != 0)
                        {
                            pointLists[i].Add(new Point(x, y));
                            break;
                        }
                    }
                }
            }

            // bottom
            for (int i = 0; i < label; i++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = image.Height - 1; y >= 0; y--)
                    {
                        if (labelMap[x][y] == i && image.GetPixel(x, y).A != 0)
                        {
                            pointLists[i].Add(new Point(x, y));
                            break;
                        }
                    }
                }
            }

            // left
            for (int i = 0; i < label; i++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        if (labelMap[x][y] == i && image.GetPixel(x, y).A != 0)
                        {
                            pointLists[i].Add(new Point(x, y));
                            break;
                        }
                    }
                }
            }

            Point[][] points = new Point[label][];
            for (int i = 0; i < label; i++)
            {
                points[i] = pointLists[i].ToArray();
            }

            return points;
        }

        public static ObjectInfo[] AForgeMask(Bitmap image)
        {
            throw new NotImplementedException();
            /*
            // Convert the image to grayscale
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(image);

            // Apply a threshold filter to segment the image into black and white regions
            Threshold thresholdFilter = new Threshold(127);
            Bitmap thresholdedImage = thresholdFilter.Apply(grayImage);

            // Find the connected components in the image
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            blobCounter.ProcessImage(thresholdedImage);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // Extract information about each object
            List<ObjectInfo> objectsInfo = new List<ObjectInfo>();
            foreach (Blob blob in blobs)
            {
                ObjectInfo objectInfo = new ObjectInfo();

                // Get the outer bounds of the object
                objectInfo.OuterBounds = blob.Rectangle.GetVertices();

                // Get the inner bounds of the object
                SimpleSkeletonization skeletonFilter = new SimpleSkeletonization();
                Bitmap skeletonImage = skeletonFilter.Apply(thresholdedImage);
                objectInfo.InnerBounds = blobCounter.GetBlobsEdgePoints(blob);

                // Determine whether the object is filled or empty
                objectInfo.IsFilled = false;
                var blobData = blobCounter.map[blob.ID];
                for (int i = 0; i < blobData.Data.Length; i++)
                {
                    if (blobData.Data[i] > 0)
                    {
                        objectInfo.IsFilled = true;
                        break;
                    }
                }

                objectsInfo.Add(objectInfo);
            }

            return objectsInfo.ToArray();
            */
        }

        public static Point[] MapPixels(Bitmap image, bool precise)
        {
            List<Point> pts = new List<Point>();

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (!precise || image.GetPixel(x, y).A != 0)
                        pts.Add(new Point(x, y));
                }
            }

            return pts.ToArray();
        }
    }

    public class ObjectInfo
    {
        public bool IsFilled { get; set; }
    }
}