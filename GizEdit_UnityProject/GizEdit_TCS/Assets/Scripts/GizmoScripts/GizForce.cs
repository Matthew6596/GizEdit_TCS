using Palmmedia.ReportGenerator.Core.Parser.Filtering;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GizForce : BaseGizmo
{
    public GizForce()
    {
        GizProperties = new GizProperty[]{
            new StringProp("Name", "force0",16),
            new Vec3Prop("Position",Vector3.zero),
            new Float32Prop("Reset Time",0),
            new Float32Prop("Shake Time",0),
            new Float32Prop("Range",3),
            new BoolGroupProp(new string[]{"Dark Side","End State (unknown)","","","","","",""},
                new bool[]{false,false,false,false,false,false,false,false},new bool[]{true,true,false,false,false,false,false,false}),
            new HexProp("Unknown 1","00 00 00 "),
            new BoolProp("Not Togglable",true,"FF "),
            new HexProp("Unknown 2","00 00 "),
            new ChildListProp("Force Children",0,DefaultChildManager.defaultChildrenGizmos[2]),
            new Float32Prop("Force Speed",1),
            new Float32Prop("Return Speed",1),
            new HexProp("Unknown 3","00 00 00 00 "),
            new Float32Prop("Visual Effect Scale",1),
            new HexProp("Unknown 4","00 00 00 00 "),
            new VarStringProp("Connected Blowup",""),
            new Int16Prop("Min Stud Value",0),
            new Int16Prop("Max Stud Value",0),
            new AngleProp("Stud Spawn Angle",0),
            new Vec3Prop("Stud Spawn Pos",Vector3.zero),
            new Float32Prop("Stud Speed",1.75f),
            new VarStringProp("Process SFX",""),
            new VarStringProp("Complete SFX",""),
            new VarStringProp("Return SFX",""),
        };
    }
    override public string parentName { get => "forces"; }
    override public void CheckValues()
    {
        //Visual stuff
        mfilter.mesh = setMesh();
        mcollider.sharedMesh = mfilter.mesh;
        mrender.material = setMaterial();

        //Children list
        ChildListProp childList = (ChildListProp)GizProperties[9];
        if(childList.Children!=null&&childList.Children.Count>0)
            foreach(BaseGizmo child in childList.Children)
            {
                child.CheckValues();
            }

        //Name & position
        name = GizProperties[0].GetValueString();
        if (name == "") name = "UnnamedForce";
        transform.position = TypeConverter.ParseVec3(GizProperties[1].GetValueString());
    }

    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(Vector3.one * 0.15f); //temp
        //int m = pickupTypes.IndexOf(pickupType);
    }
    Material setMaterial()
    {
        string v = GizProperties[5].GetValueString();
        bool darkSide = bool.Parse(v[..v.IndexOf(',')]);
        int m = darkSide?1 : 0;
        return transform.parent.GetComponent<MeshRenderer>().materials[m];
    }
    public override string GetGizType() { return "GizForce"; }

}
