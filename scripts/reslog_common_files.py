import re


racial_pattern = re.compile("^chara/human/c(.*).(mdl|mtrl)")
uld_pattern = re.compile("^ui/uld/(.*).uld$")
shpk_pattern = re.compile("^shader/sm5/shpk/(.*).shpk$") # only sm5 exists in current builds. originally excluded
shcd_pattern = re.compile("^shader/sm5/(posteffect|shcd)/(.*).shcd$")
vfx_pattern = re.compile("^(vfx/channeling|vfx/lockon|vfx/omen|bgcommon|vfx/live)(.*).avfx$")
pap_pattern = re.compile("^chara/human/c(.*)/animation/a(.*)/bt_common/(event|event_base|gs|human_sp|idle_sp|music|normal|pc_contentsaction)/(.*).pap$")

tmb_pattern = re.compile("chara/action/(.*).tmb")
tmb_exceptionpattern = re.compile("chara/action/(ability|emote|emote_ajust|emote_sp|eureka|facial|human_sp|magic|mon_sp|mount_sp|rol_common|weapon|ws)/(.*).tmb") # checking against paths existing in other tabs. still has a few repeats
tmb_pattern_weapon = re.compile("chara/action/weapon/(craft|event_base|fishing|gun_action|music|pc_contentsaction|special)(.*).tmb$")
tmb_pattern_loop_benchmark = re.compile("^chara/action/(.*)(loop|_bm)(.*).tmb$") # loop TMBs are mostly unaccounted for, so including those until the handling exists. benchmark is just because

racial = []
uld = []
shpk = []
shcd = []
vfx = []
pap = []
tmb = []

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
            uld.sort(key=lambda y: y.lower()) # normally sorts by case
            continue
        
        if shpk_pattern.match(path):
            shpk.append(path)
            shpk.sort()
            continue

        if shcd_pattern.match(path):
            shcd.append(path)
            shcd.sort()
            continue

        if vfx_pattern.match(path):
            vfx.append(path)
            vfx.sort()
            continue

        if pap_pattern.match(path):
            pap.append(path)
            pap.sort()
            continue

        if not tmb_exceptionpattern.match(path):
            if tmb_pattern.match(path) or tmb_pattern_weapon.match(path) or tmb_pattern_loop_benchmark.match(path):
                tmb.append(path)
                tmb.sort()
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

with open('common_pap', 'w') as outFile:
    outFile.write('\n'.join(pap))

with open('common_tmb', 'w') as outFile:
    outFile.write('\n'.join(tmb))