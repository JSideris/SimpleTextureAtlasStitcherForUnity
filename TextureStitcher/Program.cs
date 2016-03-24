using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureStitcher
{

	class Program
	{
		//TODO: use args to add more functionality.
		static void Main(string[] args)
		{
			string classtext = "public class TextureRegion\r\n{\r\n\tpublic string name;\r\n"
				+ "\tpublic int x;\r\n" 
				+ "\tpublic int y;\r\n"
				+ "\tpublic int width;\r\n"
				+ "\tpublic int height;\r\n"
				+ "\tpublic float u1 { get { return (float)x / TextureAtlas.totalWidth; } }\r\n"
				+ "\tpublic float u2 { get { return (float)(x + width) / TextureAtlas.totalWidth; } }\r\n"
				+ "\tpublic float v1 { get { return (float)y / TextureAtlas.totalHeight; } }\r\n"
				+ "\tpublic float v2 { get { return (float)(y + height) / TextureAtlas.totalHeight; } }\r\n"
				+ "}\r\n\r\n"
				+ "public static class TextureAtlas\r\n{\r\n";
			Console.ForegroundColor = ConsoleColor.White;
			print("Texture folder path?: ");
			var folder = Console.ReadLine();

			if (Directory.Exists(folder))
			{
				string[] files = Directory.GetFiles(folder).OrderBy(o => o).ToArray();
				List<Bitmap> bmList = new List<Bitmap>();

				int totalWidth = 0;
				int maxHeight = 0;

				print();
				print("Loading textures...");
				foreach (var f in files) {
					try {
						var bmp = new Bitmap(f);
						bmList.Add(bmp);
						
						classtext += "\tpublic static TextureRegion " + Path.GetFileNameWithoutExtension(f) + " = new TextureRegion{x = " + totalWidth + ", y = 0, width = " + bmp.Width + ", height = " + bmp.Height + "};\r\n";

						totalWidth += bmp.Width;
						maxHeight = Math.Max(maxHeight, bmp.Height);
						print(Path.GetFileName(f), ConsoleColor.Green);

						
					}
					catch (Exception e) {
						print("File is not a bitmap: " + Path.GetFileName(f), ConsoleColor.Red);
					}
				}

				classtext += "\r\n" 
					+ "\tpublic static int totalWidth = " + totalWidth + ";\r\n" 
					+ "\tpublic static int totalHeight = " + maxHeight + ";\r\n";
				classtext += "}";

				print();
				print("Stitching...");
				try {
					var stitched = new Bitmap(totalWidth, maxHeight);
					Graphics g = Graphics.FromImage(stitched);

					int xCursor = 0;
					for (var i = 0; i < bmList.Count; i++) {
						g.DrawImage(bmList[i], xCursor, 0, bmList[i].Width, bmList[i].Height);
						xCursor += bmList[i].Width;
					}

					// Clean up
					g.Dispose();

					print();
					print("Saving bmp.");
					if (File.Exists("out.png")) {
						File.Delete("out.png");
					}
					stitched.Save("out.png");


					print();
					print("Saving cs.");

					if (File.Exists("out.cs")) {
						File.Delete("out.cs");
					}
					File.WriteAllText("out.cs", classtext);
				}
				catch (Exception e) {
					print("Error: " + e.Message, ConsoleColor.Red);
				}
			}
			else {
				print("Directory does not exist.", ConsoleColor.Red);
			}

			print();
			print("Done - Press Enter.");
			Console.ReadLine();
			
		}

		private static void print()
		{
			Console.WriteLine();
		}
		private static void print(string message)
		{
			print(message, Console.ForegroundColor);
		}

		private static void print(string message, ConsoleColor colour) {
			var temp = Console.ForegroundColor;
			Console.ForegroundColor = colour;
			Console.WriteLine(message);
			Console.ForegroundColor = temp;
		}
	}
}
