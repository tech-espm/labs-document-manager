using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace DocumentManager.Utils {
	public static class ImageManipulation {
		public static ImageCodecInfo GetEncoder(ImageFormat format) {
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo codec in codecs) {
				if (codec.FormatID == format.Guid) {
					return codec;
				}
			}
			return null;
		}

		public static ImageCodecInfo GetJpegEncoder(out EncoderParameters parameters, int quality = 94) {
			ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);
			if (jpegEncoder == null) {
				parameters = null;
				return null;
			}
			Encoder encoder = Encoder.Quality;
			parameters = new EncoderParameters(1);
			parameters.Param[0] = new EncoderParameter(encoder, (long)quality);
			return jpegEncoder;
		}

		public static void SaveJpeg(Bitmap bmp, string file, int quality = 94) {
			ImageCodecInfo jpgEncoder = GetJpegEncoder(out EncoderParameters parameters, quality);
			bmp.Save(file, jpgEncoder, parameters);
		}

		public static byte[] SaveJpeg(Bitmap bmp, int quality = 94) {
			ImageCodecInfo jpgEncoder = GetJpegEncoder(out EncoderParameters parameters, quality);
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(1024 * 1024)) {
				bmp.Save(stream, jpgEncoder, parameters);
				return stream.ToArray();
			}
		}

		private static byte[] SaveBmp(Bitmap bmp, out int length) {
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(1024 * 1024)) {
				bmp.Save(stream, ImageFormat.Bmp);
				length = (int)stream.Length;
				return stream.GetBuffer();
			}
		}
	}
}
