import bpy
import sys
import time
import argparse
import os
import platform
import shutil
import re
 
def get_args():
  parser = argparse.ArgumentParser()
 
  # get all script args
  _, all_arguments = parser.parse_known_args()
  double_dash_index = all_arguments.index('--')
  script_args = all_arguments[double_dash_index + 1: ]
 
  # add parser rules
  parser.add_argument('-in', '--inm', help="Original Model")
  parser.add_argument('-out', '--outm', help="Rendered thumbnail")
  parsed_script_args, _ = parser.parse_known_args(script_args)
  return parsed_script_args

def render_view(view, obb):
  obb.select = True
  path = re.sub(r"\.(?=\w+)","_" + view + ".", render_output_path)
  bpy.ops.view3d.viewnumpad(context, 'EXEC_DEFAULT', type=view)
  bpy.ops.view3d.camera_to_view(context, 'EXEC_DEFAULT')
  bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  #bpy.ops.view3d.camera_to_view_selected(context, 'EXEC_DEFAULT')
  
  #bpy.ops.object.select_all(action='DESELECT')
  obb.select = False
  obb.show_wire = True
  obb.show_all_edges = True

  bpy.ops.render.opengl(write_still=True)
  bpy.data.images['Render Result'].save_render(path)

args = get_args()

input_model = str(args.inm)
print(input_model)

render_output_path = str(args.outm)
print(render_output_path)

print('\nImporting the input 3D model, please wait.......')

if input_model.endswith("stl"):
  bpy.ops.import_mesh.stl(filepath=input_model,axis_forward='-Z', axis_up='Y')
elif input_model.endswith("obj"):
  bpy.ops.import_scene.obj(filepath=input_model)

#select imported obj
for o in bpy.data.objects:
    if o.type == 'MESH':
        o.select = True
    else:
        o.select = False

mat = bpy.data.materials.new(name="Material")
mat.diffuse_color = (1, 1, 1) #change color
mat.diffuse_intensity = 1.0 #change intensity 
o.data.materials.append(mat) #assign material

ob = bpy.context.object
obs = bpy.context.scene.objects

if ob is None:
    for o in obs:
        if o.type == 'MESH':
            ob = o
            break


#if ob is not None:
#    for ob in obs:
#        if ob.type == 'MESH':
#            ob.show_wire = True

#define context (for whatever reason blender requires this for the 'viewnumpad' command to work)
for area in bpy.context.screen.areas:
    if area.type == "VIEW_3D":
        break

for region in area.regions:
    if region.type == "WINDOW":
        break

space = area.spaces[0]

context = bpy.context.copy()
context['area'] = area
context['region'] = region
context['space_data'] = space

angles = ['FRONT', 'BACK', 'TOP', 'BOTTOM', 'RIGHT', 'LEFT']

for angle in angles:
  render_view(angle,ob)

#close blender
sys.exit(1)
O.wm.quit_blender()