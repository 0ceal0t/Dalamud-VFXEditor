import pandas as pd
import numpy as np

data = np.empty((0,2))
last_id = -1

skip = ["Investigate", "NONE", "[[RESERVED]]", "Empty entry","???","nan","Unknown"]

xl_file = pd.ExcelFile("Model Spreadsheet.xlsx")
for sheet_name in xl_file.sheet_names:
    if "-" not in sheet_name: # skip if sheet name is not like: 500-100
        continue

    data_frame = xl_file.parse(sheet_name)
    for index, row in data_frame.iterrows():
        current_id = row[0]
        current_name = str(row[1])

        try: # some weird id formatting, just skip it
            if np.isnan(current_id):
                continue
        except:
            print("malformed id: ", current_id)
            continue
            
        if current_name in skip:
            continue

        current_id = int(current_id)
        if current_id == last_id: # avoid duplicates
            continue
        last_id = current_id

        current_name = current_name.replace('\"', '').replace('\'', '')

        data = np.append(data, np.array([[current_id, current_name]]), axis=0)

df = pd.DataFrame(data, columns= ['ModelType', 'Name'])
df.to_csv ('npc.csv', index = False, header=True)