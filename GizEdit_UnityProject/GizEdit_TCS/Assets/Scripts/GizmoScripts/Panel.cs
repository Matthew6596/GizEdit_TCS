using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : BaseGizmo
{
    public Panel()
    {
        GizProperties = new GizProperty[]{
            new VarString32Prop("Name", "Unnamed"),
            new Vec3Prop("Position",Vector3.zero),
            new AngleProp("Rotation",0),
            new EnumProp("Type",0,panelTypes,panelTypeBytes),
            new BoolProp("Invisible",false,1),
            new Vec3Prop("Trigger Position",Vector3.zero),
            new Float32Prop("Trigger Range",1),
            new BoolProp("Trigger Invisible",false,1),
            new BoolProp("Alternative Face Color",false,1),
            new BoolProp("Alternative Body Color",false,1),
            new HexProp("Unknown","01 00 "),
        };
    }
    override public string parentName => "panels";
    override public void CheckValues()
    {
        mfilter.mesh = setMesh();
        mcollider.sharedMesh = mfilter.mesh;
        mrender.material = setMaterial();
        mrender.enabled = !GizProperties[4].GetValue<bool>();
        name = GizProperties[0].GetValue<string>();
        if (name == "") name = "Unnamed";
        transform.position = GizProperties[1].GetValue<Vector3>();
    }

    static public string[] panelTypes = { "Astromech Droid","Protocol Droid","Bounty Hunter","Stormtrooper"};
    static public byte[] panelTypeBytes = { 0,1,2,3};
    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(new Vector3(1, 2, 1)*.25f);
    }
    Material setMaterial()
    {
        return transform.parent.GetComponent<MeshRenderer>().material;
    }

    public override string GetGizType() { return "panel"; }
}
