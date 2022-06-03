using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityTools.TextureTools
{
	public class TextureFunc : Editor
	{

		[MenuItem("Assets/TextureTools/提取 TexAndResize100")]
		public static void ChouquSelectTexAndReset()
		{
			SelectTex(true);
		}

		[MenuItem("Assets/TextureTools/Tiqu Tex")]
		public static void ChouquSelectTex()
		{
			SelectTex(false);
		}

		public static void SelectTex(bool resize)
		{
			//      GameObject go = Selection.activeGameObject;
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				var objs = AssetDatabase.LoadAllAssetsAtPath(path);
				Texture2D tex = objs[0] as Texture2D;
				Color[,] colors = new Color[tex.width, tex.height];
				for (int i = 0; i < tex.width; i++)
				{
					for (int j = 0; j < tex.height; j++)
					{
						colors[i, j] = tex.GetPixel(i, j);
					}
				}

				for (int i = 1; i < objs.Length; i++)
				{

					var sprite = objs[i] as Sprite;

					Debug.LogWarning(sprite.name + " " + sprite.rect.ToString());
					int width = (int) sprite.rect.width;
					int heigth = (int) sprite.rect.height;
					int startX = (int) sprite.rect.x;
					int startY = (int) sprite.rect.y;
					Texture2D target = new Texture2D(width, heigth, TextureFormat.RGBA32, false);
					for (int x = startX; x < startX + width; x++)
					{
						for (int y = startY; y < startY + heigth; y++)
						{
							target.SetPixel(x - startX, y - startY, colors[x, y]);
						}
					}

					var sizeTarget = target;
					if (resize)
					{
						sizeTarget = new Texture2D(100, 100, TextureFormat.RGBA32, false);
						ScaleTexture(target, ref sizeTarget);
					}

					if (!Directory.Exists(dir + "/" + tex.name + "/"))
					{
						Directory.CreateDirectory(dir + "/" + tex.name + "/");
					}

					System.IO.File.WriteAllBytes(dir + "/" + tex.name + "/" + sprite.name + ".png",
						sizeTarget.EncodeToPNG());
				}

				AssetDatabase.Refresh();
			}
		}

		[MenuItem("Assets/TextureTools/重置图片大小1242_2688")]
		public static void Resize19_9()
		{
			Resize(1242, 2688);
		}

		[MenuItem("Assets/TextureTools/重置图片大小2048_2732")]
		public static void Resize2048_2732()
		{
			Resize(2048, 2732);
		}

		[MenuItem("Assets/TextureTools/重置图片大小128")]
		public static void Resize128()
		{
			Resize(128, 128);
		}

		[MenuItem("Assets/TextureTools/重置图片大小80")]
		public static void Resize80()
		{
			Resize(80, 80);
		}

		[MenuItem("Assets/TextureTools/重置图片大小512")]
		public static void Resize512()
		{
			Resize(512, 512);
		}

		static void Resize(int sizeX, int sizeY)
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);
				TextureSettingTools.AssetSetCompresss(path, TextureImporterCompression.Uncompressed);

				var objs = AssetDatabase.LoadAllAssetsAtPath(path);
				Texture2D tex = objs[0] as Texture2D;
				Texture2D sizeTarget = new Texture2D(sizeX, sizeY, TextureFormat.RGBA32, false);
				ScaleTexture(tex, ref sizeTarget);

				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + tex.name + ".png", sizeTarget.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/TextureTools/左右翻转图片")]
		public static void RevertSelectTex()
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				Debug.LogWarning(obj.name);
				Color[,] color = new Color[obj.width, obj.height];
				for (int i = 0; i < obj.width; i++)
				{
					for (int j = 0; j < obj.height; j++)
					{
						color[i, j] = obj.GetPixel(i, j);
					}
				}

				Texture2D target = new Texture2D(obj.width, obj.height, obj.format, false);
				for (int i = 0; i < obj.width; i++)
				{
					for (int j = 0; j < obj.height; j++)
					{
						target.SetPixel(i, j, color[obj.width - 1 - i, j]);
					}
				}

				target.Apply();
				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + obj.name + ".png", target.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/TextureTools/组合图片，大小50*50（位置5，5）,制作卷用途")]
		public static void CombineTex()
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				TextureSettingTools.AssetSetReadWriteEnabled("Exports/Res/GoodIcon/juanzhou/juanzhou.png", true, true);
				TextureSettingTools.AssetSetFormat("Exports/Res/GoodIcon/juanzhou/juanzhou.png",
					TextureImporterFormat.ARGB32);
				var backs = AssetDatabase.LoadAllAssetsAtPath("Exports/Res/GoodIcon/juanzhou/juanzhou.png");
				Texture2D back_tex = backs[0] as Texture2D;
				Texture2D newText = new Texture2D(back_tex.width, back_tex.height, TextureFormat.RGBA32, false);
				newText.SetPixels(back_tex.GetPixels());

				var objs = AssetDatabase.LoadAllAssetsAtPath(path);
				Texture2D tex = objs[0] as Texture2D;
				Texture2D sizeTarget = new Texture2D(45, 45, TextureFormat.RGBA32, false);
				ScaleTexture(tex, ref sizeTarget);


				int width = back_tex.width;
				int height = back_tex.height;

				for (int x = width / 2 - 18, countx = 0; x < width && countx < 45; x++, countx++)
				{
					for (int y = height / 2 - 18, county = 0; y < height && county < 45; y++, county++)
					{
						Color color = sizeTarget.GetPixel(countx, county);
						if (color.a == 0)
						{
							continue;
						}

						newText.SetPixel(x, y, color);
					}
				}

				newText.Apply();

				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + tex.name + "_zhizuojuan.png", newText.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/TextureTools/去黑色区域")]
		public static void RemoveBlack()
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
//			Debug.LogWarning (obj.name);
//			Color[,] color = new Color[obj.width, obj.height];
//			for (int i = 0; i < obj.width; i++) {
//				for (int j = 0; j < obj.height; j++) {
//					color [i, j] = obj.GetPixel (i, j);
//				}
//			}
				var fc = 150.0f / 250.0f;
				var clear = new Color(0.5f, 0.5f, 0.5f, 0);
				Texture2D target = new Texture2D(obj.width, obj.height, obj.format, false);
				for (int i = 0; i < obj.width; i++)
				{
					for (int j = 0; j < obj.height; j++)
					{
						var c = obj.GetPixel(i, j);
						if (c.r > fc || c.g > fc
						             || c.b > fc)
						{
							target.SetPixel(i, j, c);
						}
						else
						{
							target.SetPixel(i, j, clear);
						}
					}
				}

				target.Apply();
				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + obj.name + ".png", target.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}

		static void ScaleTexture(Texture2D source, ref Texture2D target)
		{
			Texture2D result = target;

			float incX = (1.0f / (float) target.width);
			float incY = (1.0f / (float) target.height);

			for (int i = 0; i < result.height; ++i)
			{
				for (int j = 0; j < result.width; ++j)
				{
					Color newColor = source.GetPixelBilinear((float) j / (float) result.width,
						(float) i / (float) result.height);
					result.SetPixel(j, i, newColor);
				}
			}

			result.Apply();
		}


		[MenuItem("Assets/TextureTools/获取上半身，高度对齐宽度")]
		public static void GetShangBanSheng()
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				Debug.LogWarning(obj.name);
				Color[,] colors = new Color[obj.width, obj.height];
				if (obj.width == 1 || obj.height == 1)
				{
					continue;
				}

				for (int i = 0; i < obj.width; i++)
				{
					for (int j = 0; j < obj.height; j++)
					{
						colors[i, j] = obj.GetPixel(i, j);
					}
				}

				int size = obj.width;
				int startx = obj.width / 2 - size / 2;
				int starty = obj.height - size;
				Texture2D target = new Texture2D(size, size, obj.format, false);
				for (int i = 0; i < size; i++)
				{
					int objx = startx + i;


					for (int j = 0; j < size; j++)
					{

						int objy = starty + j;
						Color setColor;
						if (objx < 0 || objx >= obj.width || objy < 0 || objy >= obj.height)
						{
							setColor = new Color(0, 0, 0, 0);
						}
						else
						{
							setColor = colors[objx, objy];
						}

						target.SetPixel(i, j, setColor);
					}
				}

				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + obj.name + ".png", target.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/TextureTools/获取下半身，高度对齐宽度")]
		public static void GetXiaBanSheng()
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				Debug.LogWarning(obj.name);
				Color[,] colors = new Color[obj.width, obj.height];
				if (obj.width == 1 || obj.height == 1)
				{
					continue;
				}

				for (int i = 0; i < obj.width; i++)
				{
					for (int j = 0; j < obj.height; j++)
					{
						colors[i, j] = obj.GetPixel(i, j);
					}
				}

				int size = obj.width;
				int startx = obj.width / 2 - size / 2;
				int starty = 0;
				Texture2D target = new Texture2D(size, size, obj.format, false);
				for (int i = 0; i < size; i++)
				{
					int objx = startx + i;


					for (int j = 0; j < size; j++)
					{

						int objy = starty + j;
						Color setColor;
						if (objx < 0 || objx >= obj.width || objy < 0 || objy >= obj.height)
						{
							setColor = new Color(0, 0, 0, 0);
						}
						else
						{
							setColor = colors[objx, objy];
						}

						target.SetPixel(i, j, setColor);
					}
				}

				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + obj.name + ".png", target.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/TextureTools/剔除左右边界空白")]
		public static void GetRemoveWSpaces()
		{
			GetRemoveAllSpaces(true, false);
		}

		[MenuItem("Assets/TextureTools/剔除上下边界空白")]
		public static void GetRemoveHSpaces()
		{
			GetRemoveAllSpaces(false, true);
		}

		[MenuItem("Assets/TextureTools/剔除边界空白")]
		public static void GetRemoveSpaces()
		{
			GetRemoveAllSpaces(true, true);
		}


		public static void GetRemoveAllSpaces(bool removeW, bool removeH)
		{
			var selects = Selection.assetGUIDs;
			foreach (var s in selects)
			{
				string path = AssetDatabase.GUIDToAssetPath(s);
				Debug.LogWarning(path);
				string dir = Path.GetDirectoryName(path);
				Debug.LogWarning(dir);

				TextureSettingTools.AssetSetReadWriteEnabled(path, true, true);
				TextureSettingTools.AssetSetFormat(path, TextureImporterFormat.ARGB32);

				var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
				Debug.LogWarning(obj.name);

				Color[,] colors = new Color[obj.width, obj.height];
				if (obj.width == 1 || obj.height == 1)
				{
					continue;
				}

				int startx = 0;
				int starty = 0;
				int endx = obj.width - 1;
				int endy = obj.height - 1;

				for (int i = 0; i < obj.width; i++)
				{
					for (int j = 0; j < obj.height; j++)
					{
						colors[i, j] = obj.GetPixel(i, j);
					}
				}

				if (removeW)
				{
					for (int i = 0; i < obj.width; i++)
					{
						startx = i;
						bool existXColor = false;
						for (int j = 0; j < obj.height; j++)
						{
							if (colors[i, j].a > 0)
							{
								existXColor = true;
								break;
							}
						}

						if (existXColor)
						{
							break;
						}
					}


					for (int i = obj.width - 1; i >= 0; i--)
					{
						endx = i;
						bool existXColor = false;
						for (int j = 0; j < obj.height; j++)
						{
							if (colors[i, j].a > 0)
							{
								existXColor = true;
								break;
							}
						}

						if (existXColor)
						{
							break;
						}
					}
				}

				if (removeH)
				{
					for (int j = 0; j < obj.height; j++)
					{
						starty = j;
						bool existYColor = false;
						for (int i = 0; i < obj.width; i++)
						{
							if (colors[i, j].a > 0)
							{
								existYColor = true;
								break;
							}
						}

						if (existYColor)
						{
							break;
						}
					}

					for (int j = obj.height - 1; j >= 0; j--)
					{
						endy = j;
						bool existYColor = false;
						for (int i = 0; i < obj.width; i++)
						{
							if (colors[i, j].a > 0)
							{
								existYColor = true;
								break;
							}
						}

						if (existYColor)
						{
							break;
						}
					}
				}

				int width = endx - startx;
				int height = endy - starty;
				if (width <= 0)
				{
					width = 1;

				}

				if (height <= 0)
				{
					height = 1;
				}

				Texture2D target = new Texture2D(width, height, obj.format, false);
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						target.SetPixel(i, j, colors[startx + i, starty + j]);
					}
				}

				if (!Directory.Exists(dir + "new/"))
				{
					Directory.CreateDirectory(dir + "new/");
				}

				System.IO.File.WriteAllBytes(dir + "new/" + obj.name + ".png", target.EncodeToPNG());
			}

			AssetDatabase.Refresh();
		}


		[MenuItem("Assets/TextureTools/OneMinusRGBA")]
		private static void OneMinusRGBA()
		{
			var selects = Selection.objects;
			foreach (var sSelect in selects)
			{
				var tx = sSelect as Texture2D;
				var path = AssetDatabase.GetAssetPath(tx);
				var dir = Path.GetDirectoryName(path);
				TextureSettingTools.AssetSetReadWriteEnabled(path,true,true);
				OneMinusRGBA(tx,dir+"/"+sSelect.name+"_OneMinusRGBA.png");
			}

		}
		
		public static void OneMinusRGBA(Texture2D rgba,string path)
		{
			UnityEngine.Color[] color = new Color[rgba.width*rgba.height];
			UnityEngine.Color[] source = rgba.GetPixels();
			for (int i = 0; i < color.Length; i++)
			{
				color[i] = Color.white - source[i];
			}
			
			Texture2D newTex = new Texture2D(rgba.width,rgba.height);
			newTex.SetPixels(color);
			newTex.Apply();
			File.WriteAllBytes(path,newTex.EncodeToPNG());
			AssetDatabase.Refresh();
		}
		

		public static void CombineTexByRGBA(Texture2D[] rgba,int[] sampleIndex, string combinePath)
		{
			if (rgba[3] == null)
			{
				rgba[3] = rgba[0];
				sampleIndex[3] = 0;
			}
			
			int width = rgba[0].width;
			int height =  rgba[0].height;
			// UnityEngine.Color[]  cols = new Color[width*height];
			UnityEngine.Color[][] colors = new Color[4][];
			for (int i = 0; i < rgba.Length; i++)
			{
				if (rgba[i].width != width)
				{
					Debug.LogError("width 不匹配");
					return;
				}
				if (rgba[i].height != height)
				{
					Debug.LogError("height 不匹配");
					return;
				}
				var path = AssetDatabase.GetAssetPath(rgba[i]);
				TextureSettingTools.AssetSetReadWriteEnabled(path,true,true);
				colors[i] = rgba[i].GetPixels();
			}
			UnityEngine.Color[] color = new Color[rgba[0].width*rgba[0].height];

			for (int i = 0; i < color.Length; i++)
			{
				var r = colors[0][i][sampleIndex[0]];
				var g = colors[1][i][sampleIndex[1]];
				var b = colors[2][i][sampleIndex[2]];
				var a = colors[3][i][sampleIndex[3]];
				
				color[i] = new Color(r,g,b,a);
			}
			
			Texture2D newTex = new Texture2D(width,height);
			newTex.SetPixels(color);
			newTex.Apply();
			File.WriteAllBytes(combinePath,newTex.EncodeToPNG());
			AssetDatabase.Refresh();
			
		}
	}
}