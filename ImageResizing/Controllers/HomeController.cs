using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageResizing.Controllers
{
    public class HomeController : Controller
    {
        //GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null)
            {
                //// width + height will force size, care for distortion
                ////Exmaple: ImageUpload imageUpload = new ImageUpload { Width = 800, Height = 700 };

                //// height will increase the width proportionally
                ////Example: ImageUpload imageUpload = new ImageUpload { Height= 600 };

                //// width will increase the height proportionally
                //ImageUpload imageUpload = new ImageUpload { Width = 900, Height = 1350 };

                //// rename, resize, and upload
                ////return object that contains {bool Success,string ErrorMessage,string ImageName}
                //ImageResult imageResult = imageUpload.RenameUploadFile(file);
                //if (imageResult.Success)
                //{
                //    //TODO: write the filename to the db
                //    //  Console.WriteLine(imageResult.ImageName);
                //    ViewBag.filename = "Success: " + file.FileName;
                //}
                //else
                //{
                //    // use imageResult.ErrorMessage to show the error
                //    ViewBag.Error = "Error: " + imageResult.ErrorMessage;
                //}



                //Saeed

                string originalImagePath = Server.MapPath(@"/Images/OriginalImages/");
                string originalImageFullPath = string.Concat(originalImagePath, file.FileName);
                string resizedImagePath = Server.MapPath(@"/Images/ResizedImages/");
                string resizedImageFullPath = string.Concat(resizedImagePath, file.FileName);
                int counter = 1;
                bool flag = true;
                while (flag)
                {
                    if (System.IO.File.Exists(originalImageFullPath) || System.IO.File.Exists(resizedImageFullPath))
                    {
                        originalImageFullPath = string.Concat(originalImagePath, counter.ToString(), file.FileName);
                        resizedImageFullPath = string.Concat(resizedImagePath, counter.ToString(), file.FileName);
                        counter++;
                    }
                    else
                    {
                        flag = false;
                    }
                }

                file.SaveAs(originalImageFullPath);

                using (Image image = Image.FromFile(originalImageFullPath))
                {
                    //Bitmap b = new Bitmap(image);
                    Image resizedImage = ResizeImage(image, new Size(510, 520));

                    //Bitmap b = new Bitmap(resizedImage);
                    //b.Save(resizedImageFullPath);
                    resizedImage.Save(resizedImageFullPath);
                }
            }

            return View();
        }

        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;

        private static Image ResizeImage(Image imgToResize, Size size)
        {
            using (Image fixedImage = imgToResize)
            {
                // Fix orientation if needed.
                if (fixedImage.PropertyIdList.Contains(OrientationKey))
                {
                    var orientation = (int)imgToResize.GetPropertyItem(OrientationKey).Value[0];
                    switch (orientation)
                    {
                        case NotSpecified: // Assume it is good.
                            break;
                        case NormalOrientation:
                            // No rotation required.
                            break;
                        case MirrorHorizontal:
                            fixedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case UpsideDown:
                            fixedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case MirrorVertical:
                            fixedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case MirrorHorizontalAndRotateRight:
                            fixedImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case RotateLeft:
                            fixedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case MirorHorizontalAndRotateLeft:
                            fixedImage.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case RotateRight:
                            fixedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        default:
                            throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
                    }
                }


                // Get the image current width
                int sourceWidth = fixedImage.Width;
                // Get the image current height
                int sourceHeight = fixedImage.Height;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                // Calculate width and height with new desired size
                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);
                nPercent = Math.Min(nPercentW, nPercentH);
                // New Width and Height
                int destWidth = (int)(sourceWidth * nPercentW);
                int destHeight = (int)(sourceHeight * nPercentH);

                Bitmap b = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw image with new width and height
                g.DrawImage(fixedImage, 0, 0, destWidth, destHeight);
                g.Dispose();
                return (Image)b;
            }
        }


    }
}
