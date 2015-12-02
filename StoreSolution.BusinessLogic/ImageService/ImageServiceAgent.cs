using System.Drawing;
using StoreSolution.BusinessLogic.ImageService.Contracts;

namespace StoreSolution.BusinessLogic.ImageService
{
    public class ImageServiceAgent : IImageService
    {
        public Image ByteArrayToImage(byte[] imageBytes)
        {
            return (Image)new ImageConverter().ConvertFrom(imageBytes);
        }

        public byte[] ImageToByteArray(Image image)
        {
            return (byte[]) new ImageConverter().ConvertTo(image, typeof (byte[]));
        }

        public Size GetSize(Size size, int bound)
        {
            return size.Width >= size.Height
                ? new Size(bound, bound*size.Height/size.Width)
                : new Size(bound*size.Width/size.Height, bound);
        } 
    }
}