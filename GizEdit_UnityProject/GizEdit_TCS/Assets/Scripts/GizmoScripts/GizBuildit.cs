using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizBuildit : BaseGizmo
{
    public GizBuildit()
    {
        GizProperties = new GizProperty[]{
            new StringProp("Name", "buildit0",16),
            new Vec3Prop("Position",Vector3.zero),
            new ChildListProp("Buildit Children",0,DefaultChildManager.defaultChildrenGizmos[1]),
            new Float32Prop("Jump Intensity",1),
            new Int16Prop("Min Stud Value",0),
            new Int16Prop("Max Stud Value",0),
            new HexProp("Unknown 1","01 64 "),
            new HexProp("Unknown 2","00 00 00 00 00 "),
            new Float32Prop("Stud Pitch",0),
            new Float32Prop("Stud Yaw",0),
            new Vec3Prop("Stud Spawn Pos",Vector3.zero),
            new Float32Prop("Stud Speed",1.75f),
            new HexProp("Unknown 3","00 00 00 00 00 "),
        };
    }
    override public string parentName { get => "buildits"; }
    override public void CheckValues()
    {
        //Visual stuff
        mfilter.mesh = setMesh();
        mcollider.sharedMesh = mfilter.mesh;
        mrender.material = setMaterial();

        //Children list
        ChildListProp childList = (ChildListProp)GizProperties[2];
        if (childList.Children != null && childList.Children.Count > 0)
            foreach (BaseGizmo child in childList.Children)
            {
                child.CheckValues();
            }

        //Name & position
        name = GizProperties[0].GetValueString();
        if (name == "") name = "UnnamedBuildit";
        transform.position = TypeConverter.ParseVec3(GizProperties[1].GetValueString());
    }

    Mesh setMesh()
    {
        return GizmoMeshes.CubeMesh(Vector3.one * 0.15f); //temp
        //int m = pickupTypes.IndexOf(pickupType);
    }
    Material setMaterial()
    {
        return transform.parent.GetComponent<MeshRenderer>().material;
    }
    public override string GetGizType() { return "GizBuildit"; }
}
