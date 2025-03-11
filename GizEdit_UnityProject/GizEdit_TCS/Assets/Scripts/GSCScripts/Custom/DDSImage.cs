using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
/*
 * Kons 2012-12-03 Version .2
 * 
 * Changelog:
 * .3
 * - Added support for V8U8 (thanks to AmaroK86)
 * .2
 * - Fixed a bug computing width/height of mipmaps (thanks to AmaroK86)
 * .1
 * - Base implementation
 * 
 * Supported features:
 * - DXT1
 * - DXT5
 * - LinearImage (untested)
 * - V8U8 (by AmaroK86)
 * 
 * http://code.google.com/p/kprojects/
 * Send me any change/improvement at kons.snok<at>gmail.com
 * 
 */

namespace KUtility {
	public class DDSImage {

		public class Order
		{
			public byte alphaShift, redShift, greenShift, blueShift;
			public Order(byte a, byte r, byte g, byte b)
			{
				alphaShift = a;
				redShift = r;
				greenShift = g;
				blueShift = b;
			}
		}

		public static Order ARGB = new Order(16, 8, 0, 24);
		public static Order ABGR = new Order(0, 8, 16, 24);

		private const int DDPF_ALPHAPIXELS = 0x00000001;
		private const int DDPF_ALPHA = 0x00000002;
		private const int DDPF_FOURCC = 0x00000004;
		private const int DDPF_RGB = 0x00000040;
		private const int DDPF_YUV = 0x00000200;
		private const int DDPF_LUMINANCE = 0x00020000;
		private const int DDSD_MIPMAPCOUNT    =       0x00020000;
		private const int FOURCC_DXT1 = 0x31545844;
		private const int FOURCC_DX10 = 0x30315844;
		private const int FOURCC_DXT5 = 0x35545844;

		public int dwMagic;
		private DDS_HEADER header = new DDS_HEADER();
		private DDS_HEADER_DXT10 header10 = null;//If the DDS_PIXELFORMAT dwFlags is set to DDPF_FOURCC and dwFourCC is set to "DX10"
		public byte[] bdata;//pointer to an array of bytes that contains the main surface data. 
		public byte[] bdata2;//pointer to an array of bytes that contains the remaining surfaces such as; mipmap levels, faces in a cube map, depths in a volume texture.

		public Bitmap[] images;

		public DDSImage(byte[] rawdata) {
			using (MemoryStream ms = new MemoryStream(rawdata)) {
				using (BinaryReader r = new BinaryReader(ms)) {
					dwMagic = r.ReadInt32();
					if (dwMagic != 0x20534444) {
						throw new Exception("This is not a DDS!");
					}

					Read_DDS_HEADER(header, r);

					if (((header.ddspf.dwFlags & DDPF_FOURCC) != 0) && (header.ddspf.dwFourCC == FOURCC_DX10 /*DX10*/)) {
						throw new Exception("DX10 not supported yet!");
					}

					int mipMapCount = 1;
					if ((header.dwFlags & DDSD_MIPMAPCOUNT) != 0) 
						mipMapCount = header.dwMipMapCount;
					images = new Bitmap[mipMapCount];

					bdata = r.ReadBytes(header.dwPitchOrLinearSize);
					

					for (int i = 0; i < mipMapCount; ++i) {
						// Version .2 changes <AmaroK86>
						int w = (int)(header.dwWidth / Math.Pow(2, i));
						int h = (int)(header.dwHeight / Math.Pow(2, i));

						if ((header.ddspf.dwFlags & DDPF_RGB) != 0) {
							images[i]= readLinearImage(bdata, w, h);
						} else if ((header.ddspf.dwFlags & DDPF_FOURCC) != 0) {
							images[i] = readBlockImage(bdata, w, h);
						} else if ((header.ddspf.dwFlags & DDPF_FOURCC) == 0 &&
									header.ddspf.dwRGBBitCount == 0x10 &&
									header.ddspf.dwRBitMask == 0xFF &&
									header.ddspf.dwGBitMask == 0xFF00 &&
									header.ddspf.dwBBitMask == 0x00 &&
									header.ddspf.dwABitMask == 0x00) {
							images[i] = UncompressV8U8(bdata, w, h);// V8U8 normalmap format
						}
					}

				}
			}
		}

		private Bitmap readBlockImage(byte[] data, int w, int h) {
			switch (header.ddspf.dwFourCC) {
				case FOURCC_DXT1:
					//return UncompressDXT1(data, w, h);
					return null;
				case FOURCC_DXT5:
					//return UncompressDXT5(data, w, h);
					return null;
				default: break;
			}
			throw new Exception(string.Format("0x{0} texture compression not implemented.", header.ddspf.dwFourCC.ToString("X")));
		}

		public static Bitmap ReadDDSFromGSC(byte[] bytes,ref int index,out string extension)
        {
			extension = "";
			if (!SearchForNextDDSInGSC(bytes, index, out int offset)) return null;
			index += offset;
			index += 4;

			int height = BitConverter.ToInt32(bytes, index +8);
			int width = BitConverter.ToInt32(bytes, index +12);

			index += 80;
			extension = Encoding.UTF8.GetString(bytes, index, 4);

			int bitCount = BitConverter.ToInt32(bytes, index + 4);
			int[] masks = new int[4]
			{
					BitConverter.ToInt32(bytes,index+8),
					BitConverter.ToInt32(bytes,index+12),
					BitConverter.ToInt32(bytes,index+16),
					BitConverter.ToInt32(bytes,index+20),
			};

			switch (extension)
            {
				case "DXT1": return UncompressDXT1(bytes.Skip(index + 4 + 40).ToArray(), width, height, ref index);
				case "DXT3": return UncompressDXT3(bytes.Skip(index + 4 + 40).ToArray(), width, height, ref index);
                case "DXT5": return UncompressDXT5(bytes.Skip(index + 4 + 40).ToArray(), width, height,ref index);
				default:
					Bitmap bmp = ReadOtherTypes(bitCount, masks[0], masks[1], masks[2], masks[3], bytes, width, height, index+44,out extension);
					if(bmp==null) throw new NotSupportedException($"Cannot determine DDS type {extension}, try checking the file manually at: {index}");
					return bmp;
            }
        }

		public static Bitmap[] GetAllDDSFromGSC(byte[] bytes, ref int index)
        {
			List<Bitmap> bmps = new List<Bitmap>();
			while(SearchForNextDDSInGSC(bytes, index, out int offset))
            {
				index += offset;
				index += 4;

				int height = BitConverter.ToInt32(bytes, index + 8);
				int width = BitConverter.ToInt32(bytes, index + 12);

				index += 80;
				string ddsType = Encoding.UTF8.GetString(bytes, index, 4);

				index += 44;

				switch (ddsType)
				{
					case "DXT1": bmps.Add(UncompressDXT1(bytes.Skip(index).ToArray(), width, height, ref index)); break;
					case "DXT3": bmps.Add(UncompressDXT3(bytes.Skip(index).ToArray(), width, height, ref index)); break;
					case "DXT5": bmps.Add(UncompressDXT5(bytes.Skip(index).ToArray(), width, height, ref index)); break;
					default: throw new NotSupportedException($"Cannot determine DDS type {ddsType}, try checking the file manually at: {index}");
				}
			}

			return bmps.ToArray();
        }

		private static bool SearchForNextDDSInGSC(byte[] bytes, int index, out int offset)
        {
			offset = 0;
			for (; index < bytes.Length-4; index++, offset++)
			{
				int dwMagic = BitConverter.ToInt32(bytes, index);
				if (dwMagic == 0x20534444) return true;
			}
			return false;
		}

		#region DXT1
		public static Bitmap UncompressDXT1(byte[] data, int w, int h, ref int index) {
			Bitmap res = new Bitmap((w < 4) ? 4 : w, (h < 4) ? 4 : h);
			using (MemoryStream ms = new MemoryStream(data)) {
				using (BinaryReader r = new BinaryReader(ms)) {
					for (int j = 0; j < h; j += 4) {
						for (int i = 0; i < w; i += 4) {
							DecompressBlockDXT1(i, j, r.ReadBytes(8), res);
							index += 8;
						}
					}
				}
			}
			return res;
		}

		private static void DecompressBlockDXT1(int x, int y, byte[] blockStorage, Bitmap image) {
			ushort color0 = (ushort)(blockStorage[0] | blockStorage[1] << 8);
			ushort color1 = (ushort)(blockStorage[2] | blockStorage[3] << 8);

			int temp;

			temp = (color0 >> 11) * 255 + 16;
			byte r0 = (byte)((temp / 32 + temp) / 32);
			temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
			byte g0 = (byte)((temp / 64 + temp) / 64);
			temp = (color0 & 0x001F) * 255 + 16;
			byte b0 = (byte)((temp / 32 + temp) / 32);

			temp = (color1 >> 11) * 255 + 16;
			byte r1 = (byte)((temp / 32 + temp) / 32);
			temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
			byte g1 = (byte)((temp / 64 + temp) / 64);
			temp = (color1 & 0x001F) * 255 + 16;
			byte b1 = (byte)((temp / 32 + temp) / 32);

			uint code = (uint)(blockStorage[4] | blockStorage[5] << 8 | blockStorage[6] << 16 | blockStorage[7] << 24);

			for (int j = 0; j < 4; j++) {
				for (int i = 0; i < 4; i++) {
					Color finalColor = Color.FromArgb(0);
					byte positionCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

					if (color0 > color1) {
						switch (positionCode) {
							case 0:
								finalColor = Color.FromArgb(255, r0, g0, b0);
								break;
							case 1:
								finalColor = Color.FromArgb(255, r1, g1, b1);
								break;
							case 2:
								finalColor = Color.FromArgb(255, (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
								break;
							case 3:
								finalColor = Color.FromArgb(255, (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
								break;
						}
					} else {
						switch (positionCode) {
							case 0:
								finalColor = Color.FromArgb(255, r0, g0, b0);
								break;
							case 1:
								finalColor = Color.FromArgb(255, r1, g1, b1);
								break;
							case 2:
								finalColor = Color.FromArgb(255, (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
								break;
							case 3:
								finalColor = Color.FromArgb(255, 0, 0, 0);
								break;
						}
					}

					image.SetPixel(x + i, y + j, finalColor);
				}
			}
		}
		#endregion
		#region DXT3
		public static Bitmap UncompressDXT3(byte[] data, int w, int h, ref int index)
		{
			Bitmap res = new Bitmap((w < 4) ? 4 : w, (h < 4) ? 4 : h);
			using (MemoryStream ms = new MemoryStream(data))
			{
				using (BinaryReader r = new BinaryReader(ms))
				{
					for (int j = 0; j < h; j += 4)
					{
						//ushort alphaData = r.ReadUInt16();
						//int[] alphaValues = new int[4];
						for (int i = 0; i < w; i += 4)
						{
							byte[] alphabytes = r.ReadBytes(8);
							byte[] alphas = new byte[16];
							for (int k = 0; k <8; k++)
							{
                                alphas[k*2] = (byte)((((alphabytes[k]) >> 0) & 0xF) * 17);
								alphas[k*2 + 1] = (byte)((((alphabytes[k]) >> 4) & 0xF) * 17);
								//alphas[k * 4 + 2] = (byte)((((alphabytes[k+1]) >> 0) & 0xF) * 17);
								//alphas[k * 4 + 3] = (byte)((((alphabytes[k+1]) >> 4) & 0xF) * 17);
                            }

                            DecompressBlockDXT3(i, j, r.ReadBytes(8), res, alphas);
							index += 16;
							
						}
					}
				}
			}
			return res;
		}

		private static void DecompressBlockDXT3(int x, int y, byte[] blockStorage, Bitmap image, byte[]alphas)
		{
			ushort color0 = (ushort)(blockStorage[0] | blockStorage[1] << 8);
			ushort color1 = (ushort)(blockStorage[2] | blockStorage[3] << 8);

			int temp;

			temp = (color0 >> 11) * 255 + 16;
			byte r0 = (byte)((temp / 32 + temp) / 32);
			temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
			byte g0 = (byte)((temp / 64 + temp) / 64);
			temp = (color0 & 0x001F) * 255 + 16;
			byte b0 = (byte)((temp / 32 + temp) / 32);

			temp = (color1 >> 11) * 255 + 16;
			byte r1 = (byte)((temp / 32 + temp) / 32);
			temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
			byte g1 = (byte)((temp / 64 + temp) / 64);
			temp = (color1 & 0x001F) * 255 + 16;
			byte b1 = (byte)((temp / 32 + temp) / 32);

			uint code = (uint)(blockStorage[4] | blockStorage[5] << 8 | blockStorage[6] << 16 | blockStorage[7] << 24);

			for (int j = 0; j < 4; j++)
			{
				for (int i = 0; i < 4; i++)
				{
					Color finalColor = Color.FromArgb(0);
					byte positionCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

					if (color0 > color1)
					{
						switch (positionCode)
						{
							case 0:
								finalColor = Color.FromArgb(alphas[i+j*4], r0, g0, b0);
								break;
							case 1:
								finalColor = Color.FromArgb(alphas[i + j * 4], r1, g1, b1);
								break;
							case 2:
								finalColor = Color.FromArgb(alphas[i + j * 4], (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
								break;
							case 3:
								finalColor = Color.FromArgb(alphas[i + j * 4], (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
								break;
						}
					}
					else
					{
						switch (positionCode)
						{
							case 0:
								finalColor = Color.FromArgb(alphas[i + j * 4], r0, g0, b0);
								break;
							case 1:
								finalColor = Color.FromArgb(alphas[i + j * 4], r1, g1, b1);
								break;
							case 2:
								finalColor = Color.FromArgb(alphas[i + j * 4], (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
								break;
							case 3:
								finalColor = Color.FromArgb(alphas[i + j * 4], 0, 0, 0);
								break;
						}
					}

					image.SetPixel(x + i, y + j, finalColor);
				}
			}
		}
		#endregion
		#region DXT5
		private static Bitmap UncompressDXT5(byte[] data, int w, int h, ref int index) {
			Bitmap res = new Bitmap((w < 4) ? 4 : w, (h < 4) ? 4 : h);
			using (MemoryStream ms = new MemoryStream(data)) {
				using (BinaryReader r = new BinaryReader(ms)) {
					for (int j = 0; j < h; j += 4) {
						for (int i = 0; i < w; i += 4) {
							DecompressBlockDXT5(i, j, r.ReadBytes(16), res);
							index += 16;
						}
					}
				}
			}
			return res;
		}

		static void DecompressBlockDXT5(int x, int y, byte[] blockStorage, Bitmap image) {
			byte alpha0 = blockStorage[0];
			byte alpha1 = blockStorage[1];

			int bitOffset = 2;
			uint alphaCode1 = (uint)(blockStorage[bitOffset + 2] | (blockStorage[bitOffset + 3] << 8) | (blockStorage[bitOffset + 4] << 16) | (blockStorage[bitOffset + 5] << 24));
			ushort alphaCode2 = (ushort)(blockStorage[bitOffset + 0] | (blockStorage[bitOffset + 1] << 8));

			ushort color0 = (ushort)(blockStorage[8] | blockStorage[9] << 8);
			ushort color1 = (ushort)(blockStorage[10] | blockStorage[11] << 8);

			int temp;

			temp = (color0 >> 11) * 255 + 16;
			byte r0 = (byte)((temp / 32 + temp) / 32);
			temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
			byte g0 = (byte)((temp / 64 + temp) / 64);
			temp = (color0 & 0x001F) * 255 + 16;
			byte b0 = (byte)((temp / 32 + temp) / 32);

			temp = (color1 >> 11) * 255 + 16;
			byte r1 = (byte)((temp / 32 + temp) / 32);
			temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
			byte g1 = (byte)((temp / 64 + temp) / 64);
			temp = (color1 & 0x001F) * 255 + 16;
			byte b1 = (byte)((temp / 32 + temp) / 32);

			uint code = (uint)(blockStorage[12] | blockStorage[13] << 8 | blockStorage[14] << 16 | blockStorage[15] << 24);

			for (int j = 0; j < 4; j++) {
				for (int i = 0; i < 4; i++) {
					int alphaCodeIndex = 3 * (4 * j + i);
					int alphaCode;

					if (alphaCodeIndex <= 12) {
						alphaCode = (alphaCode2 >> alphaCodeIndex) & 0x07;
					} else if (alphaCodeIndex == 15) {
						alphaCode = (int)((alphaCode2 >> 15) | ((alphaCode1 << 1) & 0x06));
					} else {
						alphaCode = (int)((alphaCode1 >> (alphaCodeIndex - 16)) & 0x07);
					}

					byte finalAlpha;
					if (alphaCode == 0) {
						finalAlpha = alpha0;
					} else if (alphaCode == 1) {
						finalAlpha = alpha1;
					} else {
						if (alpha0 > alpha1) {
							finalAlpha = (byte)(((8 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 7);
						} else {
							if (alphaCode == 6)
								finalAlpha = 0;
							else if (alphaCode == 7)
								finalAlpha = 255;
							else
								finalAlpha = (byte)(((6 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 5);
						}
					}

					byte colorCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

					Color finalColor = new Color();
					switch (colorCode) {
						case 0:
							finalColor = Color.FromArgb(finalAlpha, r0, g0, b0);
							break;
						case 1:
							finalColor = Color.FromArgb(finalAlpha, r1, g1, b1);
							break;
						case 2:
							finalColor = Color.FromArgb(finalAlpha, (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
							break;
						case 3:
							finalColor = Color.FromArgb(finalAlpha, (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
							break;
					}
					image.SetPixel(x + i, y + j, finalColor);
				}
			}
		}
		#endregion

		#region V8U8
		private Bitmap UncompressV8U8(byte[] data, int w, int h) {
			Bitmap res = new Bitmap(w, h);
			using (MemoryStream ms = new MemoryStream(data)) {
				using (BinaryReader r = new BinaryReader(ms)) {
					for (int y = 0; y < h; y++) {
						for (int x = 0; x < w; x++) {
							sbyte red = r.ReadSByte();
							sbyte green = r.ReadSByte();
							byte blue = 0xFF;

							res.SetPixel(x, y, Color.FromArgb(0x7F - red, 0x7F - green, blue));
						}
					}
				}
			}
			return res;
		}
		#endregion

		private Bitmap readLinearImage(byte[] data, int w, int h) {
			Bitmap res = new Bitmap(w, h);
			using (MemoryStream ms = new MemoryStream(data)) {
				using (BinaryReader r = new BinaryReader(ms)) {
					for (int y = 0; y < h; y++) {
						for (int x = 0; x < w; x++) {
							res.SetPixel(x, y, Color.FromArgb(r.ReadInt32()));
						}
					}
				}
			}
			return res;
		}

		private void Read_DDS_HEADER(DDS_HEADER h, BinaryReader r) {
			h.dwSize = r.ReadInt32();
			h.dwFlags = r.ReadInt32();
			h.dwHeight = r.ReadInt32();
			h.dwWidth = r.ReadInt32();
			h.dwPitchOrLinearSize = r.ReadInt32();
			h.dwDepth = r.ReadInt32();
			h.dwMipMapCount = r.ReadInt32();
			for (int i = 0; i < 11; ++i) {
				h.dwReserved1[i] = r.ReadInt32();
			}
			Read_DDS_PIXELFORMAT(h.ddspf, r);
			h.dwCaps = r.ReadInt32();
			h.dwCaps2 = r.ReadInt32();
			h.dwCaps3 = r.ReadInt32();
			h.dwCaps4 = r.ReadInt32();
			h.dwReserved2 = r.ReadInt32();
		}

		private void Read_DDS_PIXELFORMAT(DDS_PIXELFORMAT p, BinaryReader r) {
			p.dwSize = r.ReadInt32();
			p.dwFlags = r.ReadInt32();
			p.dwFourCC = r.ReadInt32();
			p.dwRGBBitCount = r.ReadInt32();
			p.dwRBitMask = r.ReadInt32();
			p.dwGBitMask = r.ReadInt32();
			p.dwBBitMask = r.ReadInt32();
			p.dwABitMask = r.ReadInt32();
		}

		private static Bitmap ReadOtherTypes(int bitCount, int redMask, int greenMask, int blueMask, int alphaMask, byte[] buffer, int w, int h, int offset, out string ext)
		{
			int[] pixels;
			if (bitCount == 16)
			{
				if (redMask == A1R5G5B5_MASKS[0] && greenMask == A1R5G5B5_MASKS[1] && blueMask == A1R5G5B5_MASKS[2] && alphaMask == A1R5G5B5_MASKS[3])
				{
					// A1R5G5B5
					pixels = readA1R5G5B5(w, h, offset, buffer, ARGB);
					ext = "DXT5";
				}
				else if (redMask == X1R5G5B5_MASKS[0] && greenMask == X1R5G5B5_MASKS[1] && blueMask == X1R5G5B5_MASKS[2] && alphaMask == X1R5G5B5_MASKS[3])
				{
					// X1R5G5B5
					pixels = readX1R5G5B5(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
				else if (redMask == A4R4G4B4_MASKS[0] && greenMask == A4R4G4B4_MASKS[1] && blueMask == A4R4G4B4_MASKS[2] && alphaMask == A4R4G4B4_MASKS[3])
				{
					// A4R4G4B4
					pixels = readA4R4G4B4(w, h, offset, buffer, ARGB);
                    ext = "DXT5";
                }
				else if (redMask == X4R4G4B4_MASKS[0] && greenMask == X4R4G4B4_MASKS[1] && blueMask == X4R4G4B4_MASKS[2] && alphaMask == X4R4G4B4_MASKS[3])
				{
					// X4R4G4B4
					pixels = readX4R4G4B4(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
				else if (redMask == R5G6B5_MASKS[0] && greenMask == R5G6B5_MASKS[1] && blueMask == R5G6B5_MASKS[2] && alphaMask == R5G6B5_MASKS[3])
				{
					// R5G6B5
					pixels = readR5G6B5(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
				else
				{
					pixels = readA1R5G5B5(w, h, offset, buffer, ARGB);
                    // Unsupported 16bit RGB image
                    //pixels = new int[0];
                    ext = "DXT5";
                }
			}
			else if (bitCount == 24)
			{
				if (redMask == R8G8B8_MASKS[0] && greenMask == R8G8B8_MASKS[1] && blueMask == R8G8B8_MASKS[2] && alphaMask == R8G8B8_MASKS[3])
				{
					// R8G8B8
					pixels = readR8G8B8(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
				else
				{
					// Unsupported 24bit RGB image
					//pixels = new int[0];
					pixels = readR8G8B8(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
			}
			else if (bitCount == 32)
			{
				if (redMask == A8B8G8R8_MASKS[0] && greenMask == A8B8G8R8_MASKS[1] && blueMask == A8B8G8R8_MASKS[2] && alphaMask == A8B8G8R8_MASKS[3])
				{
					// A8B8G8R8
					pixels = readA8B8G8R8(w, h, offset, buffer, ARGB);
                    ext = "DXT5";
                }
				else if (redMask == X8B8G8R8_MASKS[0] && greenMask == X8B8G8R8_MASKS[1] && blueMask == X8B8G8R8_MASKS[2] && alphaMask == X8B8G8R8_MASKS[3])
				{
					// X8B8G8R8
					pixels = readX8B8G8R8(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
				else if (redMask == A8R8G8B8_MASKS[0] && greenMask == A8R8G8B8_MASKS[1] && blueMask == A8R8G8B8_MASKS[2] && alphaMask == A8R8G8B8_MASKS[3])
				{
					// A8R8G8B8
					pixels = readA8R8G8B8(w, h, offset, buffer, ARGB);
                    ext = "DXT5";
                }
				else if (redMask == X8R8G8B8_MASKS[0] && greenMask == X8R8G8B8_MASKS[1] && blueMask == X8R8G8B8_MASKS[2] && alphaMask == X8R8G8B8_MASKS[3])
				{
					// X8R8G8B8
					pixels = readX8R8G8B8(w, h, offset, buffer, ARGB);
                    ext = "DXT1";
                }
				else
				{
					// Unsupported 32bit RGB image
					//pixels = new int[0];
					pixels = readA8R8G8B8(w, h, offset, buffer, ARGB);
                    ext = "DXT5";
                }
			}
			else
			{
                ext = "fail";
                return null;
			}

			return pixelsToBitmap(pixels, w, h);
		}

		private static int[] readA1R5G5B5(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int rgba = (buffer[index] & 0xFF) | (buffer[index + 1] & 0xFF) << 8; index += 2;
				int r = BIT5[(rgba & A1R5G5B5_MASKS[0]) >> 10];
				int g = BIT5[(rgba & A1R5G5B5_MASKS[1]) >> 5];
				int b = BIT5[(rgba & A1R5G5B5_MASKS[2])];
				int a = 255 * ((rgba & A1R5G5B5_MASKS[3]) >> 15);
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readX1R5G5B5(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int rgba = (buffer[index] & 0xFF) | (buffer[index + 1] & 0xFF) << 8; index += 2;
				int r = BIT5[(rgba & X1R5G5B5_MASKS[0]) >> 10];
				int g = BIT5[(rgba & X1R5G5B5_MASKS[1]) >> 5];
				int b = BIT5[(rgba & X1R5G5B5_MASKS[2])];
				int a = 255;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readA4R4G4B4(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int rgba = (buffer[index] & 0xFF) | (buffer[index + 1] & 0xFF) << 8; index += 2;
				int r = 17 * ((rgba & A4R4G4B4_MASKS[0]) >> 8);
				int g = 17 * ((rgba & A4R4G4B4_MASKS[1]) >> 4);
				int b = 17 * ((rgba & A4R4G4B4_MASKS[2]));
				int a = 17 * ((rgba & A4R4G4B4_MASKS[3]) >> 12);
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readX4R4G4B4(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int rgba = (buffer[index] & 0xFF) | (buffer[index + 1] & 0xFF) << 8; index += 2;
				int r = 17 * ((rgba & A4R4G4B4_MASKS[0]) >> 8);
				int g = 17 * ((rgba & A4R4G4B4_MASKS[1]) >> 4);
				int b = 17 * ((rgba & A4R4G4B4_MASKS[2]));
				int a = 255;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readR5G6B5(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int rgba = (buffer[index] & 0xFF) | (buffer[index + 1] & 0xFF) << 8; index += 2;
				int r = BIT5[((rgba & R5G6B5_MASKS[0]) >> 11)];
				int g = BIT6[((rgba & R5G6B5_MASKS[1]) >> 5)];
				int b = BIT5[((rgba & R5G6B5_MASKS[2]))];
				int a = 255;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readR8G8B8(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int b = buffer[index++] & 0xFF;
				int g = buffer[index++] & 0xFF;
				int r = buffer[index++] & 0xFF;
				int a = 255;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readA8B8G8R8(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int r = buffer[index++] & 0xFF;
				int g = buffer[index++] & 0xFF;
				int b = buffer[index++] & 0xFF;
				int a = buffer[index++] & 0xFF;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readX8B8G8R8(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int r = buffer[index++] & 0xFF;
				int g = buffer[index++] & 0xFF;
				int b = buffer[index++] & 0xFF;
				int a = 255; index++;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readA8R8G8B8(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int b = buffer[index++] & 0xFF;
				int g = buffer[index++] & 0xFF;
				int r = buffer[index++] & 0xFF;
				int a = buffer[index++] & 0xFF;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static int[] readX8R8G8B8(int width, int height, int offset, byte[] buffer, Order order)
		{
			int index = offset;
			int[] pixels = new int[width * height];
			for (int i = 0; i < height * width; i++)
			{
				int b = buffer[index++] & 0xFF;
				int g = buffer[index++] & 0xFF;
				int r = buffer[index++] & 0xFF;
				int a = 255; index++;
				pixels[i] = (a << order.alphaShift) | (r << order.redShift) | (g << order.greenShift) | (b << order.blueShift);
			}
			return pixels;
		}

		private static Bitmap pixelsToBitmap(int[] pixels, int w, int h)
		{
			Bitmap bmp = new Bitmap(w, h);
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					bmp.SetPixel(j, i, Color.FromArgb(pixels[j + i * w]));
				}
			}
			return bmp;
		}

		private static int[] A1R5G5B5_MASKS = { 0x7C00, 0x03E0, 0x001F, 0x8000 };
		private static int[] X1R5G5B5_MASKS = { 0x7C00, 0x03E0, 0x001F, 0x0000 };
		private static int[] A4R4G4B4_MASKS = { 0x0F00, 0x00F0, 0x000F, 0xF000 };
		private static int[] X4R4G4B4_MASKS = { 0x0F00, 0x00F0, 0x000F, 0x0000 };
		private static int[] R5G6B5_MASKS = { 0xF800, 0x07E0, 0x001F, 0x0000 };
		private static int[] R8G8B8_MASKS = { 0xFF0000, 0x00FF00, 0x0000FF, 0x000000 };
		private static int[] A8B8G8R8_MASKS = { 0x000000FF, 0x0000FF00, 0x00FF0000, unchecked((int)0xFF000000) };
		private static int[] X8B8G8R8_MASKS = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0x00000000 };
		private static int[] A8R8G8B8_MASKS = { 0x00FF0000, 0x0000FF00, 0x000000FF, unchecked((int)0xFF000000) };
		private static int[] X8R8G8B8_MASKS = { 0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000 };
		private static int[] BIT5 = { 0, 8, 16, 25, 33, 41, 49, 58, 66, 74, 82, 90, 99, 107, 115, 123, 132, 140, 148, 156, 165, 173, 181, 189, 197, 206, 214, 222, 230, 239, 247, 255 };
		private static int[] BIT6 = { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 45, 49, 53, 57, 61, 65, 69, 73, 77, 81, 85, 89, 93, 97, 101, 105, 109, 113, 117, 121, 125, 130, 134, 138, 142, 146, 150, 154, 158, 162, 166, 170, 174, 178, 182, 186, 190, 194, 198, 202, 206, 210, 215, 219, 223, 227, 231, 235, 239, 243, 247, 251, 255 };
	}

	class DDS_HEADER {
		public int dwSize;
		public int dwFlags;
		/*	DDPF_ALPHAPIXELS   0x00000001 
			DDPF_ALPHA   0x00000002 
			DDPF_FOURCC   0x00000004 
			DDPF_RGB   0x00000040 
			DDPF_YUV   0x00000200 
			DDPF_LUMINANCE   0x00020000 
		 */
		public int dwHeight;
		public int dwWidth;
		public int dwPitchOrLinearSize;
		public int dwDepth;
		public int dwMipMapCount;
		public int[] dwReserved1 = new int[11];
		public DDS_PIXELFORMAT ddspf = new DDS_PIXELFORMAT();
		public int dwCaps;
		public int dwCaps2;
		public int dwCaps3;
		public int dwCaps4;
		public int dwReserved2;
	}

	class DDS_HEADER_DXT10 {
		public DXGI_FORMAT dxgiFormat;
		public D3D10_RESOURCE_DIMENSION resourceDimension;
		public uint miscFlag;
		public uint arraySize;
		public uint reserved;
	}

	class DDS_PIXELFORMAT {
		public int dwSize;
		public int dwFlags;
		public int dwFourCC;
		public int dwRGBBitCount;
		public int dwRBitMask;
		public int dwGBitMask;
		public int dwBBitMask;
		public int dwABitMask;

		public DDS_PIXELFORMAT() {
		}
	}

	enum DXGI_FORMAT:uint {
		DXGI_FORMAT_UNKNOWN = 0,
		DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
		DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
		DXGI_FORMAT_R32G32B32A32_UINT = 3,
		DXGI_FORMAT_R32G32B32A32_SINT = 4,
		DXGI_FORMAT_R32G32B32_TYPELESS = 5,
		DXGI_FORMAT_R32G32B32_FLOAT = 6,
		DXGI_FORMAT_R32G32B32_UINT = 7,
		DXGI_FORMAT_R32G32B32_SINT = 8,
		DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
		DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
		DXGI_FORMAT_R16G16B16A16_UNORM = 11,
		DXGI_FORMAT_R16G16B16A16_UINT = 12,
		DXGI_FORMAT_R16G16B16A16_SNORM = 13,
		DXGI_FORMAT_R16G16B16A16_SINT = 14,
		DXGI_FORMAT_R32G32_TYPELESS = 15,
		DXGI_FORMAT_R32G32_FLOAT = 16,
		DXGI_FORMAT_R32G32_UINT = 17,
		DXGI_FORMAT_R32G32_SINT = 18,
		DXGI_FORMAT_R32G8X24_TYPELESS = 19,
		DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
		DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
		DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
		DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
		DXGI_FORMAT_R10G10B10A2_UNORM = 24,
		DXGI_FORMAT_R10G10B10A2_UINT = 25,
		DXGI_FORMAT_R11G11B10_FLOAT = 26,
		DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
		DXGI_FORMAT_R8G8B8A8_UNORM = 28,
		DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
		DXGI_FORMAT_R8G8B8A8_UINT = 30,
		DXGI_FORMAT_R8G8B8A8_SNORM = 31,
		DXGI_FORMAT_R8G8B8A8_SINT = 32,
		DXGI_FORMAT_R16G16_TYPELESS = 33,
		DXGI_FORMAT_R16G16_FLOAT = 34,
		DXGI_FORMAT_R16G16_UNORM = 35,
		DXGI_FORMAT_R16G16_UINT = 36,
		DXGI_FORMAT_R16G16_SNORM = 37,
		DXGI_FORMAT_R16G16_SINT = 38,
		DXGI_FORMAT_R32_TYPELESS = 39,
		DXGI_FORMAT_D32_FLOAT = 40,
		DXGI_FORMAT_R32_FLOAT = 41,
		DXGI_FORMAT_R32_UINT = 42,
		DXGI_FORMAT_R32_SINT = 43,
		DXGI_FORMAT_R24G8_TYPELESS = 44,
		DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
		DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
		DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
		DXGI_FORMAT_R8G8_TYPELESS = 48,
		DXGI_FORMAT_R8G8_UNORM = 49,
		DXGI_FORMAT_R8G8_UINT = 50,
		DXGI_FORMAT_R8G8_SNORM = 51,
		DXGI_FORMAT_R8G8_SINT = 52,
		DXGI_FORMAT_R16_TYPELESS = 53,
		DXGI_FORMAT_R16_FLOAT = 54,
		DXGI_FORMAT_D16_UNORM = 55,
		DXGI_FORMAT_R16_UNORM = 56,
		DXGI_FORMAT_R16_UINT = 57,
		DXGI_FORMAT_R16_SNORM = 58,
		DXGI_FORMAT_R16_SINT = 59,
		DXGI_FORMAT_R8_TYPELESS = 60,
		DXGI_FORMAT_R8_UNORM = 61,
		DXGI_FORMAT_R8_UINT = 62,
		DXGI_FORMAT_R8_SNORM = 63,
		DXGI_FORMAT_R8_SINT = 64,
		DXGI_FORMAT_A8_UNORM = 65,
		DXGI_FORMAT_R1_UNORM = 66,
		DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
		DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
		DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
		DXGI_FORMAT_BC1_TYPELESS = 70,
		DXGI_FORMAT_BC1_UNORM = 71,
		DXGI_FORMAT_BC1_UNORM_SRGB = 72,
		DXGI_FORMAT_BC2_TYPELESS = 73,
		DXGI_FORMAT_BC2_UNORM = 74,
		DXGI_FORMAT_BC2_UNORM_SRGB = 75,
		DXGI_FORMAT_BC3_TYPELESS = 76,
		DXGI_FORMAT_BC3_UNORM = 77,
		DXGI_FORMAT_BC3_UNORM_SRGB = 78,
		DXGI_FORMAT_BC4_TYPELESS = 79,
		DXGI_FORMAT_BC4_UNORM = 80,
		DXGI_FORMAT_BC4_SNORM = 81,
		DXGI_FORMAT_BC5_TYPELESS = 82,
		DXGI_FORMAT_BC5_UNORM = 83,
		DXGI_FORMAT_BC5_SNORM = 84,
		DXGI_FORMAT_B5G6R5_UNORM = 85,
		DXGI_FORMAT_B5G5R5A1_UNORM = 86,
		DXGI_FORMAT_B8G8R8A8_UNORM = 87,
		DXGI_FORMAT_B8G8R8X8_UNORM = 88,
		DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
		DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
		DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
		DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
		DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
		DXGI_FORMAT_BC6H_TYPELESS = 94,
		DXGI_FORMAT_BC6H_UF16 = 95,
		DXGI_FORMAT_BC6H_SF16 = 96,
		DXGI_FORMAT_BC7_TYPELESS = 97,
		DXGI_FORMAT_BC7_UNORM = 98,
		DXGI_FORMAT_BC7_UNORM_SRGB = 99,
		DXGI_FORMAT_AYUV = 100,
		DXGI_FORMAT_Y410 = 101,
		DXGI_FORMAT_Y416 = 102,
		DXGI_FORMAT_NV12 = 103,
		DXGI_FORMAT_P010 = 104,
		DXGI_FORMAT_P016 = 105,
		DXGI_FORMAT_420_OPAQUE = 106,
		DXGI_FORMAT_YUY2 = 107,
		DXGI_FORMAT_Y210 = 108,
		DXGI_FORMAT_Y216 = 109,
		DXGI_FORMAT_NV11 = 110,
		DXGI_FORMAT_AI44 = 111,
		DXGI_FORMAT_IA44 = 112,
		DXGI_FORMAT_P8 = 113,
		DXGI_FORMAT_A8P8 = 114,
		DXGI_FORMAT_B4G4R4A4_UNORM = 115,
		DXGI_FORMAT_FORCE_UINT = 0xffffffff
	}

	enum D3D10_RESOURCE_DIMENSION {
		D3D10_RESOURCE_DIMENSION_UNKNOWN = 0,
		D3D10_RESOURCE_DIMENSION_BUFFER = 1,
		D3D10_RESOURCE_DIMENSION_TEXTURE1D = 2,
		D3D10_RESOURCE_DIMENSION_TEXTURE2D = 3,
		D3D10_RESOURCE_DIMENSION_TEXTURE3D = 4
	}
}
