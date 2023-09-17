using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Quintessential;
using Quintessential.Settings;
using SDL2;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace ScaffoldingGlyphs;

using PartType = class_139;
using Permissions = enum_149;
//using BondType = enum_126;
//using BondSite = class_222;
using AtomTypes = class_175;
using PartTypes = class_191;
using Texture = class_256;
public static class API
{
	/////////////////////////////////////////////////////////////////////////////////////////////////
	// resources

	public const string descriptionBase = "The glyph of scaffolding creates a single atom that can be used for initialization, wanding, or other purposes.";
	private const string descriptionAtom1 = " This variant spawns an elemental ";
	private const string descriptionAtom2 = " atom.";
	private const string texturePath = "scaffolding/textures/parts/scaffold/";

	private static Texture scaffoldBase, scaffoldLighting;

	/////////////////////////////////////////////////////////////////////////////////////////////////
	// helpers
	private static string scaffoldDescription(string name) => descriptionBase + descriptionAtom1 + name + descriptionAtom2;
	static Texture fetchScaffoldBaseTexture()
	{
		if (scaffoldBase == null) scaffoldBase = class_235.method_615(texturePath + "base");
		return scaffoldBase;
	}
	static Texture fetchScaffoldLightingTexture()
	{
		if (scaffoldLighting == null) scaffoldLighting = class_235.method_615(texturePath + "lighting");
		return scaffoldLighting;
	}

	static Dictionary<PartType, AtomType> scaffoldDictionary = new();

	/////////////////////////////////////////////////////////////////////////////////////////////////
	// public functions

	public static IReadOnlyDictionary<PartType, AtomType> getScaffoldDictionary() => scaffoldDictionary;

	/// <summary>
	/// SUMMARY
	/// </summary>
	/// <param name="argument">restriction</param>
	public static void AddScaffold(AtomType atomType, int cost, string symbolFilepath = null, string iconPath = null, string hoverPath = null)
	{
		Texture symbol = symbolFilepath == null ? null : class_235.method_615(symbolFilepath);
		Texture icon = iconPath == null ? null : class_235.method_615(iconPath);
		Texture hover = hoverPath == null ? null : class_235.method_615(hoverPath);
		AddScaffold(atomType, cost, symbol, icon, hover);
	}
	public static void AddScaffold(AtomType atomType, int cost, Texture symbol, Texture icon = null, Texture hover = null)
	{
		var atomNames = atomType.field_2286;
		if (atomNames == null)
		{
			Logger.Log("[ScaffoldingGlyphs] ERROR: field_2286 in the provided atomType was null.");
			throw new Exception("AddScaffold: could not construct atomName from the provided atomType. Please provide atomName manually.");
		}
		string atomName = atomNames.field_2598[Language.English];
		string saveIDsuffix = atomName.ToLower();

		AddScaffold(atomType, cost, saveIDsuffix, atomName, null, symbol, icon, hover);
	}

	public static void AddScaffold(
		AtomType atomType,
		int cost,
		string saveIDsuffix,
		string atomName,
		string description = null,
		Texture symbol = null,
		Texture trayIcon = null,
		Texture trayHoverIcon = null
	)
	{
		if (description == null) description = scaffoldDescription(atomName);
		if (symbol == null)
		{
			symbol = generateSymbolFromAtomtype(atomType);
		}
		if (trayIcon == null) trayIcon = generateTrayIcon(symbol, false);
		if (trayHoverIcon == null) trayHoverIcon = generateTrayIcon(symbol, true); // single-hex stroke

		//trayIcon = generateTrayIcon(atomType.field_2287);
		//trayHoverIcon = generateTrayIcon(atomType.field_2287, true);

		if (atomName != "") atomName += " ";

		//(string) locString => locString.method_619() => locString.field_2598[Language.English];

		// construct tray icon and tray icon hover the same way that inputs/outputs do

		PartType scaffold = new PartType()
		{
			/*ID*/field_1528 = "glyph-scaffolding-" + saveIDsuffix,
			/*Name*/field_1529 = class_134.method_253("Glyph of " + atomName + "Scaffolding", string.Empty),
			/*Desc*/field_1530 = class_134.method_253(description, string.Empty),
			/*Cost*/field_1531 = cost,
			/*Is a Glyph?*/field_1539 = true,
			/*Hex Footprint*/field_1540 = new HexIndex[1] { new HexIndex(0, 0) },
			/*Icon*/field_1547 = trayIcon,
			/*Hover Icon*/field_1548 = trayHoverIcon,
			/*Glow (Shadow)*/field_1549 = class_238.field_1989.field_97.field_382,
			/*Stroke (Outline)*/field_1550 = class_238.field_1989.field_97.field_383,
			/*Permissions*/field_1551 = Permissions.None,
		};

		scaffoldDictionary.Add(scaffold, atomType);
		QApi.AddPartType(scaffold, scaffoldRenderer(atomType, symbol));
	}

	private static PartRenderer scaffoldRenderer(AtomType atomType, Texture symbol)
	{
		return (part, pos, editor, renderer) =>
		{
			var baseTexture = fetchScaffoldBaseTexture();
			Vector2 vector2 = (baseTexture.field_2056.ToVector2() / 2).Rounded() + new Vector2(0.0f, 1f);
			renderer.method_521(baseTexture, vector2);
			// marker lighting
			renderer.method_528(fetchScaffoldLightingTexture(), new HexIndex(0, 0), Vector2.Zero);
			// marker details
			vector2 = (symbol.field_2056.ToVector2() / 2).Rounded() + new Vector2(0f, 1f);
			renderer.method_521(symbol, vector2);
			// if simulation is NOT running, draw atom
			if (editor.method_503() == enum_128.Stopped)
			{
				class_236 class236 = editor.method_1989(part, pos);
				//Editor.method_925(Molecule.method_1121(atomType), class236.field_1984, new HexIndex(0, 0), class236.field_1985, 1f, 1f, 1f, false, editor);
			}
		};
		
	}

	static byte[] modifiedPixels(byte[] pixelData, Func<float, float, float, float, Tuple<float, float, float, float>> pixelTransformer)
	{
		if (pixelData.Length % 4 != 0) return pixelData;

		byte[] newPixelData = new byte[pixelData.Length];

		for (int k = 0; k < newPixelData.Length; k += 4)
		{
			var newPixel = pixelTransformer(pixelData[k + 0] / 255f, pixelData[k + 1] / 255f, pixelData[k + 2] / 255f, pixelData[k + 3] / 255f);
			newPixelData[k + 0] = (byte)(newPixel.Item1 * 255f);
			newPixelData[k + 1] = (byte)(newPixel.Item2 * 255f);
			newPixelData[k + 2] = (byte)(newPixel.Item3 * 255f);
			newPixelData[k + 3] = (byte)(newPixel.Item4 * 255f);
		}
		return newPixelData;
	}
	static byte[] copyPixels(byte[] pixelData) => modifiedPixels(pixelData, (r, g, b, a) => Tuple.Create(r, g, b, a));



	private static Texture generateSymbolFromAtomtype(AtomType atomType)
	{
		RenderTargetHandle renderTargetHandle = new RenderTargetHandle();
		// draw in the target
		Texture symbol = atomType.field_2287;
		Vector2 pos = new Vector2(-1f, 1f);
		renderTargetHandle.field_2987 = symbol.field_2056;
		class_95 class95 = renderTargetHandle.method_1352(out _);
		using (class_226.method_597(class95, Matrix4.method_1074(new Vector3(1, 1, 1))))
		{
			class_226.method_600(Color.Transparent);
			class_135.method_272(symbol, pos);
		}

		// extract resulting image from target
		class_272 class272 = Renderer.method_1313(renderTargetHandle.method_1351().field_937);
		byte[] newBytes = modifiedPixels(class272.field_2126, (r,g,b,a) =>
		{
			float gray = (0.299f * r + 0.587f * g + 0.114f * b) / 8f;
			return Tuple.Create(gray, gray, gray, a * a * a);
		});
		class_272 newclass272 = new class_272(class272.field_2123, class272.field_2124, newBytes);
		//debug save-to-file
		//string outDir = Path.Combine("ScaffoldingTemp", "DumpedAtomSprites");
		//Directory.CreateDirectory(outDir);
		//string path = Path.Combine(outDir, "image_" + counter++);
		//class272.method_735(path + "_1.png");
		//newclass272.method_735(path + "_2.png");
		return Renderer.method_1310(newclass272);
	}

	private static Texture generateTrayIcon(Texture texSymbol, bool drawHover = false)
	{
		RenderTargetHandle renderTargetHandle = new RenderTargetHandle();
		// draw in the target
		Index2 index2 = new Index2(182, 184);
		Texture texBase = fetchScaffoldBaseTexture();
		Texture texLight = fetchScaffoldLightingTexture();
		Texture texHover = class_238.field_1989.field_90.field_245.field_307;

		Vector2 center = index2.ToVector2() / 2;
		Vector2 offset(Texture texture) => (center - texture.field_2056.ToVector2() / 2).Rounded();

		renderTargetHandle.field_2987 = index2;
		class_95 class95 = renderTargetHandle.method_1352(out _);
		using (class_226.method_597(class95, Matrix4.method_1074(new Vector3(1, 1, 1))))
		{
			class_226.method_600(Color.Transparent);
			if (drawHover) class_135.method_272(texHover, offset(texHover));
			class_135.method_272(texBase, offset(texBase));
			class_135.method_272(texLight, offset(texLight));
			class_135.method_272(texSymbol, offset(texSymbol) - new Vector2(-1f, 1f));
		}

		// extract resulting image from target
		class_272 class272 = Renderer.method_1313(renderTargetHandle.method_1351().field_937);
		byte[] newBytes = copyPixels(class272.field_2126);
		class_272 newclass272 = new class_272(class272.field_2123, class272.field_2124, newBytes);
		return Renderer.method_1310(newclass272);
	}

}
