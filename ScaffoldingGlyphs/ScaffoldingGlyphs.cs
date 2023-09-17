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
//using Permissions = enum_149;
//using BondType = enum_126;
//using BondSite = class_222;
using AtomTypes = class_175;
using PartTypes = class_191;
using Texture = class_256;

public class MainClass : QuintessentialMod
{
	public static MethodInfo PrivateMethod<T>(string method) => typeof(T).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

	private static IDetour hook_Sim_method_1828;
	private delegate void orig_Sim_method_1828(Sim sim); //code that runs every cycle but before parts are processed



	public override void Load()
	{
		//
	}
	public override void LoadPuzzleContent()
	{
		//

		string path = "scaffolding/textures/parts/scaffold/";
		string iconpath = "scaffolding/textures/parts/icons/scaffold/";

		string repeatDesc = API.descriptionBase + " This variant spawns an atom sheathed in [REDACTED].";

		API.AddScaffold(AtomTypes.field_1689, 1, "repeat", "", repeatDesc, class_235.method_615(path + "repeat"), class_235.method_615(iconpath + "repeat"), class_235.method_615(iconpath + "repeat_hover"));

		List<Tuple<AtomType, int, string>> vanillaScaffolds = new()
		{
			Tuple.Create(AtomTypes.field_1675,   5, "salt"),
			Tuple.Create(AtomTypes.field_1676,  10, "air"),
			Tuple.Create(AtomTypes.field_1679,  10, "water"),
			Tuple.Create(AtomTypes.field_1678,  10, "fire"),
			Tuple.Create(AtomTypes.field_1677,  10, "earth"),
			Tuple.Create(AtomTypes.field_1687,  10, "vitae"),
			Tuple.Create(AtomTypes.field_1688,  10, "mors"),
			Tuple.Create(AtomTypes.field_1690,  40, "quintessence"),
			Tuple.Create(AtomTypes.field_1680,   5, "quicksilver"),
			Tuple.Create(AtomTypes.field_1681,   5, "lead"),
			Tuple.Create(AtomTypes.field_1683,  10, "tin"),
			Tuple.Create(AtomTypes.field_1684,  20, "iron"),
			Tuple.Create(AtomTypes.field_1682,  40, "copper"),
			Tuple.Create(AtomTypes.field_1685,  80, "silver"),
			Tuple.Create(AtomTypes.field_1686, 160, "gold"),
		};

		foreach (var scaffold in vanillaScaffolds)
		{
			string id = scaffold.Item3;
			API.AddScaffold(scaffold.Item1, scaffold.Item2, path + id, iconpath + id, iconpath + id + "_hover");
		}


	}
	public override void PostLoad()
	{
		hook_Sim_method_1828 = new Hook(PrivateMethod<Sim>("method_1828"), OnSimMethod1828_SpawnScaffolds);
		On.SolutionEditorPartsPanel.class_428.method_2047 += method_2047_AddScaffoldTray; ;
	}

	private void method_2047_AddScaffoldTray(On.SolutionEditorPartsPanel.class_428.orig_method_2047 orig, SolutionEditorPartsPanel.class_428 class428_self, string trayName, List<PartTypeForToolbar> list)
	{
		orig(class428_self, trayName, list);
		if (trayName != class_134.method_253("Glyphs", string.Empty)) return;

		//append Debugging Tray
		List<PartTypeForToolbar> toolbarList = new List<PartTypeForToolbar>();

		foreach (var scaffoldParttype in API.getScaffoldDictionary().Keys)
		{
			toolbarList.Add(PartTypeForToolbar.method_1225(scaffoldParttype, true, true));
		}
		orig(class428_self, class_134.method_253("Scaffolding Glyphs", string.Empty), toolbarList);
	}

	public override void Unload()
	{
		hook_Sim_method_1828.Dispose();
	}

	private static void OnSimMethod1828_SpawnScaffolds(orig_Sim_method_1828 orig, Sim sim)
	{
		orig(sim);
		if (sim.method_1818() == 0)//run once at the start of simulation, before arms execute grabs
		{
			var partDict = sim.field_3821;
			var scaffoldDict = API.getScaffoldDictionary();
			var molecules = sim.field_3823;

			foreach (var part in partDict.Keys)
			{
				var partType = part.method_1159();
				if (scaffoldDict.ContainsKey(partType))
				{
					var atomType = scaffoldDict[partType];
					var scaffold = new Molecule();
					scaffold.method_1105(new Atom(atomType), part.method_1184(new HexIndex(0, 0)));
					molecules.Add(scaffold);
				}
			}
		}
	}
}
