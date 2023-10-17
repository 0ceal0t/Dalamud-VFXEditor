import sqlite3
import pandas as pd
import json
import re

npc_output = {}

pattern = re.compile("^[md]([0-9]){4}$")

# csv comes from https://rl2.perchbird.dev/Downloads
# bnpc.json comes from https://gubal.hasura.app/api/rest/bnpc
# or https://raw.githubusercontent.com/ffxiv-teamcraft/ffxiv-teamcraft/staging/libs/data/src/lib/json/gubal-bnpcs-index.json

# ======= RESLOGGER =========

with open("CurrentPathList") as f:
    for line in f:
        path = line.strip()

        _id = ""
        _type = ""

        if (not path.startswith('vfx/')) and (not path.startswith('chara/')):
            continue

        if ('vfx/monster' in path or 'vfx/demihuman' in path) and '.avfx' in path:
            _id = path.split("/")[2]
            _type = "vfx"
        elif ('chara/monster' in path or 'chara/demihuman' in path) and '.pap' in path:
            _id = path.split("/")[2]
            _type = "pap"
        elif 'chara/action/mon_sp' in path and '.tmb' in path:
            _id = path.split("/")[3]
            _type = "tmb"
        else:
            continue

        if not pattern.match(_id):
            continue

        if _id not in npc_output:
            npc_output[_id] = { # default for row
                "vfx": [],
                "tmb": [],
                "pap": []
            }
        npc_output[_id][_type].append(path)

# ====== DB ============

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
        else:
            f = folderdict[_f1] + b'/' + filedict[_f2]
        _fullPaths.append(f)

delta = []

for fullpath in _fullPaths:
    _id = "" # d1024 or something
    _type = "" # vfx, tmp, pap

    path = fullpath.decode("ascii")

    if (not path.startswith('vfx/')) and (not path.startswith('chara/')):
        continue

    if ('monster' in path or 'demihuman' in path) and '.avfx' in path:
        _id = path.split("/")[2]
        _type = "vfx"
    elif ('monster' in path or 'demihuman' in path) and '.pap' in path:
        _id = path.split("/")[2]
        _type = "pap"
    elif 'mon_sp' in path and '.tmb' in path:
        _id = path.split("/")[3]
        _type = "tmb"
    else:
        continue

    if not pattern.match(_id):
        continue

    if _id not in npc_output:
        npc_output[_id] = { # default for row
            "vfx": [],
            "tmb": [],
            "pap": []
        }

    if path not in npc_output[_id][_type]:
        delta.append(path + "\n")
        npc_output[_id][_type].append(path)

cursor.close()
db.close()

# ======= OUTPUT ==========

with open("npc_files.json", "w") as outfile: 
    json.dump(npc_output, outfile)

with open("delta.txt", "w") as outfile: 
    outfile.writelines(delta)