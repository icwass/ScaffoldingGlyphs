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

		API.AddScaffold(AtomTypes.field_1689, 1, "repeat", "", API.descriptionBase + " This variant spawns an atom sheathed in [REDACTED].");
		API.AddScaffold(AtomTypes.field_1675, 5); // salt



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
