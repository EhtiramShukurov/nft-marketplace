using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace Xhibiter.Utilities
{
    public static class FileExtension
    {
        public static bool CheckType(this IFormFile file)
        {
            string type = file.ContentType;
            if (type.Contains("image/")|| type.Contains("video/") ||type.Contains("audio/"))
            {
                return true;
            }
            return false;
        }
        public static bool CheckTypeToDisplay(this IFormFile file)
        {
            string type = file.ContentType;
            if (type.Contains("image/") || type.Contains("video/") || type.Contains("audio/"))
            {
                return true;
            }
            return false;
        }
        public static bool CheckType2(this IFormFile file)
        {
            string type = file.ContentType;
            if (type.Contains("image/"))
            {
                return true;
            }
            return false;
        }
        public static bool CheckSize(this IFormFile file, int kb)
            => kb * 1024 * 1024 > file.Length;
        public static string SaveFile(this IFormFile file, string path)
        {
            string fileName = ChangeFileName(file.FileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(fs);
            }
            return fileName;
        }
        public static string SaveImage(this IFormFile file, string path)
        {
            string fileName = ChangeFileName(file.FileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 600),
                    Mode = ResizeMode.Max
                }));
                var type = fileName.Substring(fileName.LastIndexOf("."));
                IImageEncoder encoder;
                switch (type.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        encoder = new JpegEncoder
                        {
                            Quality = 75
                        };
                        break;

                    case ".png":
                        encoder = new PngEncoder
                        {
                            CompressionLevel = PngCompressionLevel.BestSpeed
                        };
                        break;

                    case ".gif":
                        encoder = new GifEncoder();
                        break;

                    case ".bmp":
                        encoder = new BmpEncoder();
                        break;

                    case ".tiff":
                        encoder = new TiffEncoder();
                        break;
                    default:
                        encoder = new JpegEncoder
                        {
                            Quality = 75
                        };
                        break;
                }
                using (var outputStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    image.Save(outputStream, encoder);
                }
            }
            return fileName;
        }


        public static string ChangeFileName(string oldName)
        {
            string extension = oldName.Substring(oldName.LastIndexOf("."));
            if (oldName.Length < 32)
            {
                oldName = oldName.Substring(0, oldName.LastIndexOf("."));
            }
            else
            {
                oldName = oldName.Substring(0, 31);
            }
            return Guid.NewGuid() + oldName + extension;
        }
        public static void DeleteFile(this string fileName, string root, string folder)
        {
            string path = Path.Combine(root, folder, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static string CheckValidate(this IFormFile? file, int kb)
        {
            string result = "";
            if (!file.CheckSize(kb))
            {
                result += $"Size of the {file.FileName} file shouldn't be higher than {kb} mb!";
            }
            if (!file.CheckType())
            {
                result += $"\nType of {file.FileName} file should be an image,video,or audio!";
            }
            return result;
        }
        public static string CheckValidate2(this IFormFile? file, int kb)
        {
            string result = "";
            if (!file.CheckSize(kb))
            {
                result += $"Size of the {file.FileName} file shouldn't be higher than {kb} mb!";
            }
            if (!file.CheckType2())
            {
                result += $"\nType of {file.FileName} file should be an image!";
            }
            return result;
        }
    }
}
