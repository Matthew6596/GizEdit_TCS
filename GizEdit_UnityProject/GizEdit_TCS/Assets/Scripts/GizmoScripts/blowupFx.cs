using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blowupFx : BaseGizmo
{
    public override string parentName => "blowups";

    public blowupFx()
    {
        GizProperties = new GizProperty[]
        {
            new VarStringProp("Special Object",""),
            new VarStringProp("Name","Unnamed"),
            new VarStringProp("Particle 1",""),
            new VarStringProp("Particle 2",""),
            new VarStringProp("Particle 3",""),
            new VarStringProp("Particle 4",""),
            new VarStringProp("Particle 5",""),
            new VarStringProp("Particle 6",""),
            new VarStringProp("Particle 7",""),
            new VarStringProp("Particle 8",""),
            new VarStringProp("Particle 9",""),
            new VarStringProp("Particle 10",""),
            new HexProp("Unknown 1","C0 00 00 00 00 00 00 01 00 00 00 00 00 "),
            new Float32Prop("Unknown 2",1),
            new Float32Prop("Animate Length",64),
            new HexProp("Unknown 3","00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 "),
            new Float32Prop("Particle 1 speed?",0),
            new Float32Prop("Particle 2 speed?",0),
            new Float32Prop("Particle 3 speed?",0),
            new Float32Prop("Particle 4 speed?",0),
            new Float32Prop("Particle 5 speed?",0),
            new Float32Prop("Particle 6 speed?",0),
            new Float32Prop("Particle 7 speed?",0),
            new Float32Prop("Particle 8 speed?",0),
            new Float32Prop("Particle 9 speed?",0),
            new Float32Prop("Particle 10 speed?",0),
            new HexProp("Unknown 4","00 00 00 00 00 00 00 "),
        };
    }
    public override void CheckValues()
    {
        //mesh
        //mfilter.mesh = setMesh();
        //mcollider.sharedMesh = mfilter.mesh;
        //material
        //mrender.material = setMaterial();
        //name
        name = GizProperties[1].GetValue<string>();
        if (name == "") name = "Unnamed";
        //position
        //transform.position = GizProperties[1].GetValue<Vector3>();

    }

    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(new Vector3(1, 1, .2f) * .25f);
    }
    Material setMaterial()
    {
        return transform.parent.GetComponent<MeshRenderer>().material;
    }

    public override string GetGizType() { return "BlowUp"; }
}
