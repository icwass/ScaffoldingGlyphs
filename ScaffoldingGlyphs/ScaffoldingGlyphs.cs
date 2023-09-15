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

//using PartType = class_139;
//using Permissions = enum_149;
//using BondType = enum_126;
//using BondSite = class_222;
using AtomTypes = class_175;
//using PartTypes = class_191;
using Texture = class_256;

public class MainClass : QuintessentialMod
{
	public static MethodInfo PrivateMethod<T>(string method) => typeof(T).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);




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
	public override void Unload()
	{
		//
	}
	public override void PostLoad()
	{
		//
	}




	public PartRenderer scaffoldRenderer(Texture Base, Texture Lighting, Texture Details)
	{
		return (part, pos, editor, renderer) =>
		{
			Vector2 vector2 = (Base.field_2056.ToVector2() / 2).Rounded() + new Vector2(0.0f, 1f);
			renderer.method_521(Base, vector2);
			/*marker_lighting*/
			renderer.method_528(Lighting, new HexIndex(0, 0), Vector2.Zero);
			/*marker_details*/
			renderer.method_521(Details, vector2);
		};
	}
}
