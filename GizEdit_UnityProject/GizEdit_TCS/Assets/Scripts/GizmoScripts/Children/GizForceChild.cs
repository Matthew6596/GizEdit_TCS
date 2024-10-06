using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizForceChild : BaseGizmo
{
    public GizForceChild()
    {
        GizProperties = new GizProperty[]
        {
            new VarStringProp("Child Name",""),
            new Float32Prop("Unknown 1",1),
            new Float32Prop("Animate Length",2),
            new BoolProp("Not Selected",true,1),
            new HexProp("Unknown 2","00 00 00 00 00 "),
        };
    }

    public override string parentName { get => "plugs"; } //doesn't matter where child goes, its basically just a property

    public override void CheckValues()
    {
        name = GizProperties[0].GetValue<string>();
        if (name == "") name = "Unnamed";
    }
    public override string GetGizType() { return "GizForceChild"; }

}
