import sqlite3
import pandas as pd
import json

db = sqlite3.connect('hashlist.db')
db.text_factory = bytes
cursor = db.cursor()

folderdict = {}
filedict = {}
_fullPaths = []

cursor.execute("select hash, path from folders;")
folders = cursor.fetchall()
for folders in folders:
    folder_hash = folders[0]
    folder_path = folders[1]
    folderdict[folder_hash] = folder_path

cursor.execute("select hash, name from filenames;")
filenames = cursor.fetchall()
for filename in filenames:
    file_hash = filename[0]
    file_path = filename[1]
    if isinstance(file_path, int):
        continue
    filedict[file_hash] = file_path

cursor.execute("select folderhash, filehash from fullpaths;")
fullpaths = cursor.fetchall()
for fullpath in fullpaths:
    _f1 = fullpath[0]
    _f2 = fullpath[1]
    if (_f1 in folderdict) and (_f2 in filedict):
        f = ""
        if b'/' in filedict[_f2]:
            f = filedict[_f2] # already has a path for some reason
            #print(f)
        else:
            f = folderdict[_f1] + b'/' + filedict[_f2]
        _fullPaths.append(f)

npc_output = {}
for fullpath in _fullPaths:
    if not (b'monster' in fullpath and b'avfx' in fullpath):
        continue
    
    _id = fullpath.split(b"/")[2].decode("utf-8") 
    if _id not in npc_output:
        npc_output[_id] = []
    npc_output[_id].append(fullpath.decode("utf-8") )

with open("monster_vfx.json", "w") as outfile: 
    json.dump(npc_output, outfile)

cursor.close()
db.close()