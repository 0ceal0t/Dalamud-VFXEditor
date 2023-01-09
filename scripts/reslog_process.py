import json

#common_vfx = []
#with open("export.csv") as f:
#    for line in f:
#        split = line.split(',')
#        path = split[2].strip()
#        if (path.startswith("bgcommon") or path.startswith("vfx/live") ) and path.endswith(".avfx"):
#            common_vfx.append(path)
#
#with open('vfx_misc.txt', 'w') as out_file:
#    out_file.write('\n'.join(common_vfx))

npc_output = {}

with open("CurrentPathList") as f:
    for line in f:
        path = line.strip()

        _id = ""
        _type = ""

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

        if 'gimmick' in _id:
            continue

        if _id not in npc_output:
            npc_output[_id] = { # default for row
                "vfx": [],
                "tmb": [],
                "pap": []
            }
        npc_output[_id][_type].append(path)

with open("npc_files.json", "w") as outfile: 
    json.dump(npc_output, outfile)