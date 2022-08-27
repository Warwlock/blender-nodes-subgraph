using System;

[Serializable]
public class CategoryClass
{
    public int m_SGVersion = 0;
    public string m_Type = "UnityEditor.ShaderGraph.CategoryData";
    public string m_ObjectId;
    public string m_Name;
    public DataIDs[] m_ChildObjectList;
}

[Serializable]
public class GraphDataClass
{
    public int m_SGVersion = 3;
    public string m_Type = "UnityEditor.ShaderGraph.GraphData";
    public string m_ObjectId;
    public DataIDs[] m_Properties;
    public DataIDs[] m_CategoryData;
    public DataIDs[] m_Nodes;
    public EdgesClass[] m_Edges;
    public string m_Path = "Sub Graphs";
    public int m_GraphPrecision = 1;
    public int m_PreviewMode = 2;
    public DataIDs m_OutputNode;
}

[Serializable]
public class NodeClass
{
    public int m_SGVersion = 0;
    public string m_Type;
    public string m_ObjectId;
    public string m_Name;
    public DrawState m_DrawState;
    public DataIDs[] m_Slots;
    public string[] synonyms;
    public int m_Precision = 1;
    public bool m_PreviewExpanded = false;
    public int m_PreviewMode = 0;
    public CustomGraphColors m_CustomColors;
    public DataIDs m_Property;
    public string m_FunctionName;
    public string m_FunctionSource;
    public int m_Space;
    public int m_ScreenSpaceType;
    public int m_OutputChannel;
}

[Serializable]
public class PropertyClass
{
    public int m_SGVersion = 1;
    public string m_Type;
    public string m_ObjectId;
    public GUIDs m_Guid;
    public string m_Name;
    public int m_DefaultRefNameVersion = 1;
    public string m_RefNameGeneratedByDisplayName;
    public string m_DefaultReferenceName;
    public bool m_UseCustomSlotLabel = false;
    public string m_CustomSlotLabel = "";
}

[Serializable]
public class GUIDs
{
    public string m_GuidSerialized;
}

[Serializable]
public class DataIDs
{
    public string m_Id;
}

[Serializable]
public class SerializedTextureGUID
{
    public string m_SerializedTexture;
}

[Serializable]
public class EdgesClass
{
    public EdgesSlotsClass m_OutputSlot;
    public EdgesSlotsClass m_InputSlot;
}

[Serializable]
public class EdgesSlotsClass
{
    public DataIDs m_Node;
    public int m_SlotId;
}

[Serializable]
public class CustomGraphColors
{
    public string m_SerializableColors;
}

[Serializable]
public class DrawState
{
    public bool m_Expanded = true;
    public PositionClass m_Position;
}

[Serializable]
public class PositionClass
{
    public string serializedVersion = "2";
    public float x;
    public float y;
    public float width;
    public float height;
}

[Serializable]
public class SlotClass
{
    public int m_SGVersion = 0;
    public string m_Type = "UnityEditor.ShaderGraph.Vector1MaterialSlot";
    public string m_ObjectId;
    public int m_Id;
    public string m_DisplayName;
    public int m_SlotType;
    public bool m_Hidden = false;
    public string m_ShaderOutputName;
    public int m_StageCapability = 3;
    public string[] m_Labels;
    public SerializedTextureGUID m_Texture;
}

/*[Serializable]
public class SlotClassTexture
{
    public int m_SGVersion = 0;
    public string m_Type = "UnityEditor.ShaderGraph.Vector1MaterialSlot";
    public string m_ObjectId;
    public int m_Id;
    public string m_DisplayName;
    public int m_SlotType;
    public bool m_Hidden = false;
    public string m_ShaderOutputName;
    public int m_StageCapability = 3;
    public string[] m_Labels;
    public SerializedTextureGUID m_Texture;
}*/
