using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blowupGiz : BaseGizmo
{
    public override string parentName => "blowups";
    public blowupGiz()
    {
        GizProperties = new GizProperty[]
        {
            new VarStringProp("blowupFx Name",""),
            new VarStringProp("Name","Unnamed"),
            new Vec3Prop("Position",Vector3.zero),
            new HexProp("Unknown 1","00 00 00 00 00 00 00 00 00 00 00 00 00 00 "),
            new Int16Prop("Min Stud Value",0),
            new Int16Prop("Max Stud Value",0),
            new Int8Prop("Stud Value Multiplier?",1),
            new AngleProp("Stud Angle",0),
            new Float32Prop("Unknown 1",0),
            new HexProp("Unknown 2","00 00 00 00 00 00 00 00 00 00 00 00 00 00 "),
            new Vec3Prop("Unknown 3",Vector3.zero),
            new Float32Prop("Unknown 4",0),
            new HexProp("Unknown 5","00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 "),

        };
    }
    public override void CheckValues()
    {
        //mesh
        mfilter.mesh = setMesh();
        mcollider.sharedMesh = mfilter.mesh;
        //material
        mrender.material = setMaterial();
        //name
        name = GizProperties[1].GetValue<string>();
        if (name == "") name = "Unnamed";
        //position
        transform.position = GizProperties[2].GetValue<Vector3>();

    }

    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(new Vector3(1, .2f, 1) * .25f);
    }
    Material setMaterial()
    {
        return transform.parent.GetComponent<MeshRenderer>().material;
    }

    public override string GetGizType(){return "BlowUp";}
}
