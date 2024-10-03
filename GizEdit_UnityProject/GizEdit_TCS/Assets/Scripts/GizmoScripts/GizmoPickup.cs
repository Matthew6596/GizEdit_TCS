using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoPickup : BaseGizmo
{
    public GizmoPickup()
    {
        GizProperties = new GizProperty[]{
            new StringProp("Name", "Unnamedp",8),
            new Vec3Prop("Position",Vector3.zero),
            new EnumProp("Type",0,pickupTypeNames,pickupTypeBytes),
            new BoolProp("Triggered Spawn",false,2),
            new Int8Prop("Spawn Group",0)
        };
    }
    override public string parentName =>"pickups";
    override public void CheckValues()
    {
        mfilter.mesh = setMesh();
        mcollider.sharedMesh = mfilter.mesh;
        mrender.material = setMaterial();
        name = GizProperties[0].GetValue<string>();
        if (name == "") name = "Unnamedp";
        transform.position = GizProperties[1].GetValue<Vector3>();
    }

    static public string pickupTypes = "sgbpmcuhrt"; //736762706d6375687274
    static public string[] pickupTypeNames = { "Silver Stud", "Gold Stud", "Blue Stud", "Purple Stud",
        "Minikit", "Challenge Minikit", "Power Up", "Heart", "Red Brick", "Torpedo" };
    static public byte[] pickupTypeBytes = { 0x73, 0x67, 0x62, 0x70,0x6D, 0x63,0x75, 0x68, 0x72, 0x74 };
    Mesh setMesh()
    {
        int m = GizProperties[2].GetValue<int>();
        if (m < 4) return GizmoMeshes.CubeMesh(Vector3.one * 0.1f); //studs
        else if (m < 6) return GizmoMeshes.CubeMesh(new Vector3(1.15f, 1.85f, 1.15f) * 0.2f); //kits
        else if (m == 6) return GizmoMeshes.CubeMesh(Vector3.one * 0.25f); //powerup
        else if (m == 7) return GizmoMeshes.CubeMesh(Vector3.one * 0.1f); //heart
        else if (m == 8) return GizmoMeshes.CubeMesh(new Vector3(1,.75f,1) * 0.25f); //redbrick
        else if (m == 9) return GizmoMeshes.CubeMesh(Vector3.one * 0.15f); //torpedo
        else return GizmoMeshes.CubeMesh(Vector3.one * 0.1f); //
    }
    Material setMaterial()
    {
        int m = GizProperties[2].GetValue<int>();
        return transform.parent.GetComponent<MeshRenderer>().materials[m];
    }

    public override string GetGizType() { return "GizmoPickup"; }
}