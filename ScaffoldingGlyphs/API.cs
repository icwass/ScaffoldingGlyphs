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
//using PartTypes = class_191;
using Texture = class_256;
public static class API
{
	private const string descriptionBase = "The glyph of scaffolding creates a single atom that can be used for initialization, wanding, or other purposes. ";
	private const string descriptionAtom1 = "This variant spawns an elemental ";
	private const string descriptionAtom2 = " atom.";
	private static string description(string name) => descriptionBase + descriptionAtom1 + name + descriptionAtom2;

	static Dictionary<PartType, AtomType> scaffoldDictionary = new();


	public static void AddRepeatScaffold()
	{
		AtomType repeat = AtomTypes.field_1689;
		int cost = 1;
		string description = "The glyph of scaffolding creates a single atom that can be used for initialization, wanding, or other purposes. ";
		string Name = "";

		string name = "repeat";
		string Description = descriptionBase + "This variant spawns an atom sheathed in [REDACTED].";

		PartType scaffold = new PartType()
		{
			/*ID*/field_1528 = "glyph-scaffolding-" + name,
			/*Name*/field_1529 = class_134.method_253("Glyph of " + Name + "Scaffolding", string.Empty),
			/*Desc*/field_1530 = class_134.method_253(Description, string.Empty),
			/*Cost*/field_1531 = cost,
			/*Is a Glyph?*/field_1539 = true,
			/*Hex Footprint*/field_1540 = new HexIndex[1] { new HexIndex(0, 0) },
			/*Icon*/field_1547 = class_238.field_1989.field_97.field_382, //class_235.method_615("icwass/textures/parts/icons/scaffolds/" + name),
			/*Hover Icon*/field_1548 = class_238.field_1989.field_97.field_383, //class_235.method_615("icwass/textures/parts/icons/scaffolds/" + name + "_hover"),
			/*Glow (Shadow)*/field_1549 = class_238.field_1989.field_97.field_382,
			/*Stroke (Outline)*/field_1550 = class_238.field_1989.field_97.field_383,
			/*Permissions*/field_1551 = Permissions.None,
		};

		scaffoldDictionary.Add(scaffold, repeat);
	}

	public static void AddScaffold(AtomType atomType, int cost, string atomName, string saveID, string description, Texture icon, Texture hoverIcon)
	{
		if (atomName != "") atomName += " ";

		PartType scaffold = new PartType()
		{
			/*ID*/field_1528 = "glyph-scaffolding-" + saveID,
			/*Name*/field_1529 = class_134.method_253("Glyph of " + atomName + "Scaffolding", string.Empty),
			/*Desc*/field_1530 = class_134.method_253(description, string.Empty),
			/*Cost*/field_1531 = cost,
			/*Is a Glyph?*/field_1539 = true,
			/*Hex Footprint*/field_1540 = new HexIndex[1] { new HexIndex(0, 0) },
			/*Icon*/field_1547 = icon,
			/*Hover Icon*/field_1548 = hoverIcon,
			/*Glow (Shadow)*/field_1549 = class_238.field_1989.field_97.field_382,
			/*Stroke (Outline)*/field_1550 = class_238.field_1989.field_97.field_383,
			/*Permissions*/field_1551 = Permissions.None,
		};

		scaffoldDictionary.Add(scaffold, atomType);
	}


}
