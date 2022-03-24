import re

prefixes = ['bgcommon','vfx/live']
f = open('vfx_misc_in.txt', 'r')
vfxMisc = f.read()
f.close()

allMatches = []
for prefix in prefixes:
    allMatches += re.findall(prefix + '\/[a-zA-Z0-9_\/]+\.avfx', vfxMisc)

with open('vfx_misc.txt', 'w') as outFile:
    outFile.write('\n'.join(allMatches))