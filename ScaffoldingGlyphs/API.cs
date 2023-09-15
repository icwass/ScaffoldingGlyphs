using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Quintessential;
using Quintessential.Settings;
using SDL2;
using System;
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

	/// <summary>
	/// SUMMARY
	/// </summary>
	/// <param name="argument">restriction</param>
	public static void AddScaffold(AtomType atomType, int cost, Texture symbol = null)
	{
		var atomNames = atomType.field_2286;
		if (atomNames == null)
		{
			Logger.Log("[ScaffoldingGlyphs] ERROR: field_2286 in the provided atomType was null.");
			throw new Exception("AddScaffold: could not construct atomName from the provided atomType. Please provide atomName manually.");
		}
		string atomName = atomNames.field_2598[Language.English];
		string saveIDsuffix = atomName.ToLower();
		AddScaffold(atomType, cost, saveIDsuffix, atomName, null, symbol);
	}

	public static void AddScaffold(AtomType atomType, int cost, string saveIDsuffix, string atomName, string description = null, Texture symbol = null, Texture trayIcon = null, Texture trayHoverIcon = null)
	{
		if (description == null) description = scaffoldDescription(atomName);
		if (trayIcon == null) trayIcon = class_238.field_1989.field_97.field_382; // single-hex glow
		if (trayHoverIcon == null) trayHoverIcon = class_238.field_1989.field_97.field_383; // single-hex stroke
		if (symbol == null)
		{
			symbol = atomType.field_2287;
			//at some point, construct symbol from atomType.field_2287 via monochrome
		}

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

		//eventually add part to part dictionary myself to bypass Quintessential not letting me use it in vanilla puzzles
		QApi.AddPartType(scaffold, scaffoldRenderer(atomType, symbol));
		QApi.AddPartTypeToPanel(scaffold, PartTypes.field_1782);
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
			vector2 = (symbol.field_2056.ToVector2() / 2).Rounded() + new Vector2(0.0f, 1f);
			renderer.method_521(symbol, vector2);
			// if simulation is NOT running, draw atom
			if (editor.method_503() == enum_128.Stopped)
			{
				class_236 class236 = editor.method_1989(part, pos);
				Editor.method_925(Molecule.method_1121(atomType), class236.field_1984, new HexIndex(0, 0), class236.field_1985, 1f, 1f, 1f, false, editor);
			}
		};
		
	}

}
