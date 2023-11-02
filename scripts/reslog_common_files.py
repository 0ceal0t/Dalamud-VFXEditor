import re


racial_pattern = re.compile("^chara/human/c(.*).mtrl")
uld_pattern = re.compile("^ui/uld/(.*).uld$")
shpk_pattern = re.compile("^shader/shpk/(.*).shpk$") # make sure to exclude shader/sm5/shpk...
shcd_pattern = re.compile("^shader/(posteffect|shcd)/(.*).shcd$")
vfx_pattern = re.compile("^(bgcommon|vfx/live)(.*).avfx$")

racial = []
uld = []
shpk = []
shcd = []
vfx = []

# chara/human/c0801/obj/face/f0207/material/mt_c0801f0207_fac_a.mtrl
# chara/human/c0701/obj/hair/h0109/material/v0001/mt_c0701h0109_acc_b.mtrl
# chara/human/c1501/obj/tail/t0001/material/v0005/mt_c1501t0001_a.mtrl
# chara/human/c1701/obj/zear/z0002/material/mt_c1701z0002_fac_a.mtrl
# chara/human/c1301/obj/body/b0101/material/v0001/mt_c1301b0101_a.mtrl

with open("CurrentPathList") as f:
    for line in f:
        path = line.strip()

        if racial_pattern.match(path):
            racial.append(path)
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


with open('common_racial', 'w') as outFile:
    outFile.write('\n'.join(racial))

with open('common_uld', 'w') as outFile:
    outFile.write('\n'.join(uld))

with open('common_shpk', 'w') as outFile:
    outFile.write('\n'.join(shpk))

with open('common_shcd', 'w') as outFile:
    outFile.write('\n'.join(shcd))

with open('common_vfx', 'w') as outFile:
    outFile.write('\n'.join(vfx))