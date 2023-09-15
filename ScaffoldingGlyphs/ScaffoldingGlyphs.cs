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
//using PartTypes = class_191;
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

		API.AddScaffold(AtomTypes.field_1689,   1, "repeat", "", API.descriptionBase + " This variant spawns an atom sheathed in [REDACTED].", class_235.method_615(path + "repeat"));
		API.AddScaffold(AtomTypes.field_1675,   5, class_235.method_615(path + "salt"));
		API.AddScaffold(AtomTypes.field_1676,  10, class_235.method_615(path + "air"));
		API.AddScaffold(AtomTypes.field_1679,  10, class_235.method_615(path + "water"));
		API.AddScaffold(AtomTypes.field_1678,  10, class_235.method_615(path + "fire"));
		API.AddScaffold(AtomTypes.field_1677,  10, class_235.method_615(path + "earth"));
		API.AddScaffold(AtomTypes.field_1687,  10, class_235.method_615(path + "vitae"));
		API.AddScaffold(AtomTypes.field_1688,  10, class_235.method_615(path + "mors"));
		API.AddScaffold(AtomTypes.field_1690,  40, class_235.method_615(path + "quintessence"));
		API.AddScaffold(AtomTypes.field_1680,   5, class_235.method_615(path + "quicksilver"));
		API.AddScaffold(AtomTypes.field_1681,   5, class_235.method_615(path + "lead"));
		API.AddScaffold(AtomTypes.field_1683,  10, class_235.method_615(path + "tin"));
		API.AddScaffold(AtomTypes.field_1684,  20, class_235.method_615(path + "iron"));
		API.AddScaffold(AtomTypes.field_1682,  40, class_235.method_615(path + "copper"));
		API.AddScaffold(AtomTypes.field_1685,  80, class_235.method_615(path + "silver"));
		API.AddScaffold(AtomTypes.field_1686, 160, class_235.method_615(path + "gold"));


	}
	public override void PostLoad()
	{
		hook_Sim_method_1828 = new Hook(PrivateMethod<Sim>("method_1828"), OnSimMethod1828_SpawnScaffolds);
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
