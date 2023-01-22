bl_info = {
    "name": "Blender to Unity Node Exporter",
    "description": "Export shader nodes to Unity.",
    "author": "Warwlock",
    "version": (1, 0),
    "blender": (3, 1, 0),
    "location": "Node Editor Toolbar",
    "doc_url": "https://github.com/Warwlock/blender-nodes-subgraph/wiki",
    "category": "Node",
}

import bpy
import json
from bpy_extras.io_utils import ExportHelper
from bpy.props import StringProperty, BoolProperty, EnumProperty, FloatVectorProperty, PointerProperty
from bpy.types import Operator, PropertyGroup

class AdditionalProperties(PropertyGroup):
    use_simple_noise: BoolProperty(
        name = "Use Simple Noise Texture",
        description = "Use simple noise texture in Unity",
        default = True
        )
    
    show_unsupported: BoolProperty(
        name = "Show Unsupported Nodes",
        description = "Import unsupported nodes as sticky note in Unity",
        default = True
        )


class node_exporter_unity_main_panel(bpy.types.Panel):
    
    bl_label = "Node Exporter"
    bl_idname = "node_exporter_unity_main_panel"
    
    bl_space_type = 'NODE_EDITOR'
    bl_region_type = 'UI'
    bl_category = 'Node Exporter'

    def draw(self, context):
        layout = self.layout
        scene = context.scene
        mytool = scene.add_tools

        layout.operator("node.blender_unity_exporter")
        layout.prop(mytool, "use_simple_noise")
        layout.prop(mytool, "show_unsupported")
    
class Object:
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, 
            sort_keys=True, indent=4)

#---------------------------- Math
def mathNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [nnode.operation, str(nnode.use_clamp), str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Value
def valueInputNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.outputs[0].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- RGB
def RGBInputNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.outputs[0].default_value[0]), 
        str(nnode.outputs[0].default_value[1]),
        str(nnode.outputs[0].default_value[2]), str(nnode.outputs[0].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Noise
def noiseTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [nnode.noise_dimensions, str(nnode.inputs[1].default_value), 
        str(nnode.inputs[2].default_value), str(nnode.inputs[3].default_value), 
        str(nnode.inputs[4].default_value), str(nnode.inputs[5].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Brick
def brickTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [nnode.offset, nnode.offset_frequency, nnode.squash, nnode.squash_frequency, 
        str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[1].default_value[3]),
        
        str(nnode.inputs[2].default_value[0]), str(nnode.inputs[2].default_value[1]),
        str(nnode.inputs[2].default_value[2]), str(nnode.inputs[2].default_value[3]),
        
        str(nnode.inputs[3].default_value[0]), str(nnode.inputs[3].default_value[1]),
        str(nnode.inputs[3].default_value[2]), str(nnode.inputs[3].default_value[3]),
        
        str(nnode.inputs[4].default_value), str(nnode.inputs[5].default_value),
        str(nnode.inputs[6].default_value), str(nnode.inputs[7].default_value), 
        str(nnode.inputs[8].default_value), str(nnode.inputs[9].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Checker
def checkerTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[1].default_value[3]),
        str(nnode.inputs[2].default_value[0]), str(nnode.inputs[2].default_value[1]),
        str(nnode.inputs[2].default_value[2]), str(nnode.inputs[2].default_value[3]),
        str(nnode.inputs[3].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Gradient
def gradientTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.gradient_type)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Image
def imageTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Magic
def magicTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.turbulence_depth),
        str(nnode.inputs[1].default_value),
        str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Musgrave
def musgraveTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.musgrave_dimensions), str(nnode.musgrave_type),
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value),
        str(nnode.inputs[3].default_value), str(nnode.inputs[4].default_value),
        str(nnode.inputs[5].default_value), str(nnode.inputs[6].default_value),
        str(nnode.inputs[7].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Voronoi
def voronoiTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.voronoi_dimensions), str(nnode.feature), str(nnode.distance),
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value),
        str(nnode.inputs[3].default_value), str(nnode.inputs[4].default_value),
        str(nnode.inputs[5].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections2Name = [0] * len(nnode.outputs[2].links)
    myNode.connections2NodeName = [0] * len(nnode.outputs[2].links)
    for l in nnode.outputs[2].links:
        myNode.connections2Name[i] = l.to_socket.identifier
        myNode.connections2NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections3Name = [0] * len(nnode.outputs[3].links)
    myNode.connections3NodeName = [0] * len(nnode.outputs[3].links)
    for l in nnode.outputs[3].links:
        myNode.connections3Name[i] = l.to_socket.identifier
        myNode.connections3NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections4Name = [0] * len(nnode.outputs[4].links)
    myNode.connections4NodeName = [0] * len(nnode.outputs[4].links)
    for l in nnode.outputs[4].links:
        myNode.connections4Name[i] = l.to_socket.identifier
        myNode.connections4NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Wave
def waveTextureNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.wave_type), str(nnode.bands_direction),
        str(nnode.rings_direction), str(nnode.wave_profile),
        str(nnode.inputs[1].default_value),
        str(nnode.inputs[2].default_value),
        str(nnode.inputs[3].default_value),
        str(nnode.inputs[4].default_value),
        str(nnode.inputs[5].default_value),
        str(nnode.inputs[6].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Bright/Contrast
def brightcontrastNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value[0]), 
        str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2]), str(nnode.inputs[0].default_value[3]),
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Gamma
def gammaNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value[0]), 
        str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2]), str(nnode.inputs[0].default_value[3]),
        str(nnode.inputs[1].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- HueSaturation
def huesaturationNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value),
        str(nnode.inputs[3].default_value),
        str(nnode.inputs[4].default_value[0]), str(nnode.inputs[4].default_value[1]),
        str(nnode.inputs[4].default_value[2]), str(nnode.inputs[4].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Invert
def invertNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[1].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- MixRGB
def mixrgbNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.blend_type),
        str(nnode.use_clamp), str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[1].default_value[3]),
        str(nnode.inputs[2].default_value[0]), str(nnode.inputs[2].default_value[1]),
        str(nnode.inputs[2].default_value[2]), str(nnode.inputs[2].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- RGBCurves
def rgbcurveNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    
    cString = ""
    rString = ""
    gString = ""
    bString = ""
    
    for c in nnode.mapping.curves[3].points:
        cString += str(c.location.x) + "-" + str(c.location.y) + "|"
    
    for r in nnode.mapping.curves[0].points:
        rString += str(r.location.x) + "-" + str(r.location.y) + "|"
    
    for g in nnode.mapping.curves[1].points:
        gString += str(g.location.x) + "-" + str(g.location.y) + "|"
    
    for b in nnode.mapping.curves[2].points:
        bString += str(b.location.x) + "-" + str(b.location.y) + "|"
    
    myNode.properties = [cString, rString, gString, bString,
        str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[1].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Bump
def bumpNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.invert),
        str(nnode.inputs[0].default_value), str(nnode.inputs[1].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Mapping
def mappingNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.vector_type),
        str(nnode.inputs[0].default_value[0]), str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2]), str(nnode.inputs[1].default_value[0]),
        str(nnode.inputs[1].default_value[1]), str(nnode.inputs[1].default_value[2]),
        str(nnode.inputs[2].default_value[0]), str(nnode.inputs[2].default_value[1]),
        str(nnode.inputs[2].default_value[2]), str(nnode.inputs[3].default_value[0]),
        str(nnode.inputs[3].default_value[1]), str(nnode.inputs[3].default_value[2])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- NormalMap
def normalmapNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[1].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- ColorRamp
def colorrampNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    
    stopsString = ""
    
    for s in nnode.color_ramp.elements:
        stopsString += str(s.color[0]) + "-" + str(s.color[1]) + "-" + str(s.color[2]) + "-" + str(s.color[3]) + "-" + str(s.position) + "|"    
    
    myNode.properties = [stopsString, str(nnode.color_ramp.color_mode), 
        str(nnode.color_ramp.interpolation), str(nnode.color_ramp.hue_interpolation),
        str(nnode.inputs[0].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- CombineHSV
def combinehsvNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value), 
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- CombineRGB
def combinergbNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value), 
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- CombineXYZ
def combinexyzNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value), 
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- MapRange
def maprangeNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.interpolation_type), str(nnode.clamp),
        str(nnode.inputs[0].default_value), str(nnode.inputs[1].default_value),
        str(nnode.inputs[2].default_value), str(nnode.inputs[3].default_value),
        str(nnode.inputs[4].default_value), str(nnode.inputs[5].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- RGBToBW
def rgbtobwNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value[0]), 
        str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2]),
        str(nnode.inputs[0].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- SeparateHSV
def separatehsvNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value[0]), 
        str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2]),
        str(nnode.inputs[0].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections2Name = [0] * len(nnode.outputs[2].links)
    myNode.connections2NodeName = [0] * len(nnode.outputs[2].links)
    for l in nnode.outputs[2].links:
        myNode.connections2Name[i] = l.to_socket.identifier
        myNode.connections2NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- SeparateRGB
def separatergbNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value[0]), 
        str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2]),
        str(nnode.inputs[0].default_value[3])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections2Name = [0] * len(nnode.outputs[2].links)
    myNode.connections2NodeName = [0] * len(nnode.outputs[2].links)
    for l in nnode.outputs[2].links:
        myNode.connections2Name[i] = l.to_socket.identifier
        myNode.connections2NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- SeparateXYZ
def separatexyzNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value[0]), 
        str(nnode.inputs[0].default_value[1]),
        str(nnode.inputs[0].default_value[2])]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections2Name = [0] * len(nnode.outputs[2].links)
    myNode.connections2NodeName = [0] * len(nnode.outputs[2].links)
    for l in nnode.outputs[2].links:
        myNode.connections2Name[i] = l.to_socket.identifier
        myNode.connections2NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- VectorMath
def vectormathNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.operation), str(nnode.inputs[0].default_value[0]),
        str(nnode.inputs[0].default_value[1]), str(nnode.inputs[0].default_value[2]),
        str(nnode.inputs[1].default_value[0]), str(nnode.inputs[1].default_value[1]),
        str(nnode.inputs[1].default_value[2]), str(nnode.inputs[2].default_value[0]),
        str(nnode.inputs[2].default_value[1]), str(nnode.inputs[2].default_value[2]),
        str(nnode.inputs[3].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- TextureCoordinate
def texcoordNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    
    i = 0
    myNode.connections0Name = [0] * (len(nnode.outputs[0].links) + len(nnode.outputs[1].links))
    myNode.connections0NodeName = [0] * (len(nnode.outputs[0].links) + len(nnode.outputs[1].links))
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    for l in nnode.outputs[1].links:
        myNode.connections0Name[i + len(nnode.outputs[0].links)] = l.to_socket.identifier
        myNode.connections0NodeName[i + len(nnode.outputs[0].links)] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[2].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[2].links)
    for l in nnode.outputs[2].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections2Name = [0] * len(nnode.outputs[3].links)
    myNode.connections2NodeName = [0] * len(nnode.outputs[3].links)
    for l in nnode.outputs[3].links:
        myNode.connections2Name[i] = l.to_socket.identifier
        myNode.connections2NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections3Name = [0] * len(nnode.outputs[4].links)
    myNode.connections3NodeName = [0] * len(nnode.outputs[4].links)
    for l in nnode.outputs[4].links:
        myNode.connections3Name[i] = l.to_socket.identifier
        myNode.connections3NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections4Name = [0] * len(nnode.outputs[5].links)
    myNode.connections4NodeName = [0] * len(nnode.outputs[5].links)
    for l in nnode.outputs[5].links:
        myNode.connections4Name[i] = l.to_socket.identifier
        myNode.connections4NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Geometry
def geometryNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections1Name = [0] * len(nnode.outputs[1].links)
    myNode.connections1NodeName = [0] * len(nnode.outputs[1].links)
    for l in nnode.outputs[1].links:
        myNode.connections1Name[i] = l.to_socket.identifier
        myNode.connections1NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    i = 0
    myNode.connections2Name = [0] * len(nnode.outputs[4].links)
    myNode.connections2NodeName = [0] * len(nnode.outputs[4].links)
    for l in nnode.outputs[4].links:
        myNode.connections2Name[i] = l.to_socket.identifier
        myNode.connections2NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
    
    i = 0
    myNode.connections3Name = [0] * len(nnode.outputs[6].links)
    myNode.connections3NodeName = [0] * len(nnode.outputs[6].links)
    for l in nnode.outputs[6].links:
        myNode.connections3Name[i] = l.to_socket.identifier
        myNode.connections3NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Blackbody
def blackbodyNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [str(nnode.inputs[0].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#---------------------------- Clamp
def clampNodeGet(nnode):
    myNode = Object()
    myNode.name = nnode.bl_idname + "." + nnode.name
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.properties = [nnode.clamp_type, str(nnode.inputs[0].default_value),
        str(nnode.inputs[1].default_value), str(nnode.inputs[2].default_value)]
    
    i = 0
    myNode.connections0Name = [0] * len(nnode.outputs[0].links)
    myNode.connections0NodeName = [0] * len(nnode.outputs[0].links)
    for l in nnode.outputs[0].links:
        myNode.connections0Name[i] = l.to_socket.identifier
        myNode.connections0NodeName[i] = l.to_socket.node.bl_idname + "." + l.to_socket.node.name
        i+=1
        
    return myNode

#----------------------------
#---------------------------- NULL
def emptyNodeGet(nnode):
    myNode = Object()
    myNode.location = [nnode.location.x, nnode.location.y]
    myNode.name = "NULL" + "." + nnode.bl_idname + "." + nnode.name
    #myNode.name = nnode.bl_idname
    
    return myNode

#----------------------------
#---------------------------- EXTRA
def extraNodeGet(context):
    myNode = Object()
    myNode.name = "EXTRA"
    myNode.properties = [str(context.scene.add_tools.use_simple_noise),
        str(context.scene.add_tools.show_unsupported)]
    #myNode.name = nnode.bl_idname
    
    return myNode

#----------------------------
#----------------------------
class NodeOperator(bpy.types.Operator, ExportHelper):
    
    bl_idname = "node.blender_unity_exporter"
    bl_label = "Export Node Graph"
    bl_description = "Export nodes to Json text file"
    
    filename_ext = ".json"

    @classmethod
    def poll(cls, context):
        space = context.space_data
        if(space.node_tree != None):
            return space.type == 'NODE_EDITOR' and space.node_tree.bl_idname == "ShaderNodeTree"
        return space.node_tree != None

    def execute(self, context):
        tree = context.space_data.node_tree

        i = 0
        newJson = Object()
        newJson.nodeDatas = [0] * (len(tree.nodes) + 1)
        for n in tree.nodes:
            if(n.bl_idname == "ShaderNodeMath"):
                newJson.nodeDatas[i] = mathNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexNoise"):
                newJson.nodeDatas[i] = noiseTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeValue"):
                newJson.nodeDatas[i] = valueInputNodeGet(n)
            elif(n.bl_idname == "ShaderNodeRGB"):
                newJson.nodeDatas[i] = RGBInputNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexBrick"):
                newJson.nodeDatas[i] = brickTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeBlackbody"):
                newJson.nodeDatas[i] = blackbodyNodeGet(n)
            elif(n.bl_idname == "ShaderNodeClamp"):
                newJson.nodeDatas[i] = clampNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexChecker"):
                newJson.nodeDatas[i] = checkerTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexGradient"):
                newJson.nodeDatas[i] = gradientTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexImage"):
                newJson.nodeDatas[i] = imageTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexMagic"):
                newJson.nodeDatas[i] = magicTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexMusgrave"):
                newJson.nodeDatas[i] = musgraveTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexVoronoi"):
                newJson.nodeDatas[i] = voronoiTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexWave"):
                newJson.nodeDatas[i] = waveTextureNodeGet(n)
            elif(n.bl_idname == "ShaderNodeBrightContrast"):
                newJson.nodeDatas[i] = brightcontrastNodeGet(n)
            elif(n.bl_idname == "ShaderNodeGamma"):
                newJson.nodeDatas[i] = gammaNodeGet(n)
            elif(n.bl_idname == "ShaderNodeHueSaturation"):
                newJson.nodeDatas[i] = huesaturationNodeGet(n)
            elif(n.bl_idname == "ShaderNodeInvert"):
                newJson.nodeDatas[i] = invertNodeGet(n)
            elif(n.bl_idname == "ShaderNodeMixRGB"):
                newJson.nodeDatas[i] = mixrgbNodeGet(n)
            elif(n.bl_idname == "ShaderNodeRGBCurve"):
                newJson.nodeDatas[i] = rgbcurveNodeGet(n)
            elif(n.bl_idname == "ShaderNodeBump"):
                newJson.nodeDatas[i] = bumpNodeGet(n)
            elif(n.bl_idname == "ShaderNodeMapping"):
                newJson.nodeDatas[i] = mappingNodeGet(n)
            elif(n.bl_idname == "ShaderNodeNormalMap"):
                newJson.nodeDatas[i] = normalmapNodeGet(n)
            elif(n.bl_idname == "ShaderNodeValToRGB"):
                newJson.nodeDatas[i] = colorrampNodeGet(n)
            elif(n.bl_idname == "ShaderNodeCombineHSV"):
                newJson.nodeDatas[i] = combinehsvNodeGet(n)
            elif(n.bl_idname == "ShaderNodeCombineRGB"):
                newJson.nodeDatas[i] = combinergbNodeGet(n)
            elif(n.bl_idname == "ShaderNodeCombineXYZ"):
                newJson.nodeDatas[i] = combinexyzNodeGet(n)
            elif(n.bl_idname == "ShaderNodeMapRange"):
                newJson.nodeDatas[i] = maprangeNodeGet(n)
            elif(n.bl_idname == "ShaderNodeRGBToBW"):
                newJson.nodeDatas[i] = rgbtobwNodeGet(n)
            elif(n.bl_idname == "ShaderNodeSeparateHSV"):
                newJson.nodeDatas[i] = separatehsvNodeGet(n)
            elif(n.bl_idname == "ShaderNodeSeparateRGB"):
                newJson.nodeDatas[i] = separatergbNodeGet(n)
            elif(n.bl_idname == "ShaderNodeSeparateXYZ"):
                newJson.nodeDatas[i] = separatexyzNodeGet(n)
            elif(n.bl_idname == "ShaderNodeVectorMath"):
                newJson.nodeDatas[i] = vectormathNodeGet(n)
            elif(n.bl_idname == "ShaderNodeTexCoord"):
                newJson.nodeDatas[i] = texcoordNodeGet(n)
            elif(n.bl_idname == "ShaderNodeNewGeometry"):
                newJson.nodeDatas[i] = geometryNodeGet(n)
            else:
                newJson.nodeDatas[i] = emptyNodeGet(n)
            i+=1
        
        newJson.nodeDatas[len(tree.nodes)] = extraNodeGet(context)
        
        write_some_data(newJson.toJSON(), self.filepath)
        self.report({'INFO'}, 'Node Graph Exported')

        return {'FINISHED'}


def write_some_data(context, filepath):
    f = open(filepath, 'w', encoding='utf-8')
    f.write(context)
    f.close()

    return {'FINISHED'}

classes = [AdditionalProperties, node_exporter_unity_main_panel, NodeOperator]

def register():
    for cls in classes:
        bpy.utils.register_class(cls)
        
    bpy.types.Scene.add_tools = PointerProperty(type=AdditionalProperties)


def unregister():
    for cls in reversed(classes):
        bpy.utils.unregister_class(cls)
        
    del bpy.types.Scene.add_tools


if __name__ == "__main__":
    register()
