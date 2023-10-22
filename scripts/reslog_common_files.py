import re


faces_pattern = re.compile("^chara/human/c(.*)/skeleton/face/f(.*).sklb$")
uld_pattern = re.compile("^ui/uld/(.*).uld$")
shpk_pattern = re.compile("^shader/shpk/(.*).shpk$") # make sure to exclude shader/sm5/shpk...
shcd_pattern = re.compile("^shader/(posteffect|shcd)/(.*).shcd$")
vfx_pattern = re.compile("^(bgcommon|vfx/live)(.*).avfx$")

faces = []
uld = []
shpk = []
shcd = []
vfx = []

with open("CurrentPathList") as f:
    for line in f:
        path = line.strip()

        if faces_pattern.match(path):
            faces.append(path)
            continue

        if uld_pattern.match(path):
            uld.append(path)
            continue
        
        if shpk_pattern.match(path):
            shpk.append(path)
            continue

        if shcd_pattern.match(path):
            shcd.append(path)
            continue

        if vfx_pattern.match(path):
            vfx.append(path)
            continue


with open('common_faces', 'w') as outFile:
    outFile.write('\n'.join(faces))

with open('common_uld', 'w') as outFile:
    outFile.write('\n'.join(uld))

with open('common_shpk', 'w') as outFile:
    outFile.write('\n'.join(shpk))

with open('common_shcd', 'w') as outFile:
    outFile.write('\n'.join(shcd))

with open('common_vfx', 'w') as outFile:
    outFile.write('\n'.join(vfx))