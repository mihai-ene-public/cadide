
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core
{
    public static class Extensions
    {
        //public static T Clone<T>(this T objectToClone)
        //{
        //    var xml = XmlHelper.SerializeObjectToXmlString(objectToClone);
        //    var cloned = XmlHelper.GetObjectFromXmlString<T>(xml);
        //    return cloned;
        //}

        public static byte[] getByteArrayFromObject(Object o)
        {
            MemoryStream ms = new MemoryStream();
            var bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }


        public static object getObjectFromByteArray(byte[] theByteArray)
        {
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;

            return bf1.Deserialize(ms);
        }

        public static byte[] Compress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(buffer, 0, buffer.Length);
            zip.Close();
            ms.Position = 0;

            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return gzBuffer;
        }

        public static byte[] Decompress(byte[] gzBuffer)
        {
            MemoryStream ms = new MemoryStream();
            int msgLength = BitConverter.ToInt32(gzBuffer, 0);
            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

            byte[] buffer = new byte[msgLength];

            ms.Position = 0;
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
            zip.Read(buffer, 0, buffer.Length);

            return buffer;
        }


        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            var pi = obj.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                var v = pi.GetValue(obj);
                if (v is T)
                    return (T)v;
            }

            return default(T);
        }



        public static XRect GetBoardRectangle(this IBoardDesigner board)
        {
            if (board.BoardOutline == null)
                return XRect.Empty;

            var upperLeft = board.BoardOutline.StartPoint;
            var lowerRight = upperLeft;
            var startPoint = board.BoardOutline.StartPoint;
            foreach (var item in board.BoardOutline.Items)
            {
                //upperLeft: Xmin, Ymin
                if (upperLeft.X > item.EndPointX)
                    upperLeft.X = item.EndPointX;
                if (upperLeft.Y > item.EndPointY)
                    upperLeft.Y = item.EndPointY;

                //lowerRight: Xmax, Ymax
                if (lowerRight.X < item.EndPointX)
                    lowerRight.X = item.EndPointX;
                if (lowerRight.Y < item.EndPointY)
                    lowerRight.Y = item.EndPointY;
            }

            return new XRect(upperLeft, lowerRight);
        }

        static double clamp(double value, double min, double max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }
        public static bool IntersectsCircle(this XRect rect, XPoint center, double radius)
        {

            // Find the closest point to the circle within the rectangle
            var closestX = clamp(center.X, rect.Left, rect.Right);
            var closestY = clamp(center.Y, rect.Top, rect.Bottom);

            // Calculate the distance between the circle's center and this closest point
            var distanceX = center.X - closestX;
            var distanceY = center.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            var distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (radius * radius);
        }



        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        public static void AddRange(this IList collection, IEnumerable items)
        {
            foreach (var item in items)
                collection.Add(item);
        }


        public static void CopyDirectory(string sourceDirectory, string targetDirectory, bool overwrite = true)
        {
            Directory.CreateDirectory(targetDirectory);

            // Copy each file into the new directory.
            foreach (var filePath in Directory.GetFiles(sourceDirectory))
            {
                var fn = Path.GetFileName(filePath);
                var destFile = Path.Combine(targetDirectory, fn);
                File.Copy(filePath, destFile, overwrite);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in Directory.GetDirectories(sourceDirectory))
            {
                var dirName = Path.GetFileName(diSourceSubDir);
                var subDir = Path.Combine(targetDirectory, dirName);
                CopyDirectory(diSourceSubDir, subDir);
            }
        }

        public static void MeasureTime(string actionName, Action action)
        {
            var sw = Stopwatch.StartNew();

            action();

            sw.Stop();
            Debug.WriteLine($"{actionName}: {sw.ElapsedMilliseconds} ms");
        }
    }
}
