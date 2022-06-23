import re
import json
import sys

table = """Ctrl+W

Close Windows Explorer

F5

Refresh Explorer Window

F11

Toggle Full Screen

Alt+P

Toggle preview pane.

Alt+D

Jump/ focus Address Bar with content selected

Alt+D, then Ctrl+C

Copy current path

Alt+D, then Alt+Down Arrow

Show previous locations

Alt+Up Arrow

Jump one level up

Alt+D, Tab, Tab

Jump/ focus Navigation Pane

Alt+D, Tab, Tab, Tab

Jump/ focus Folder Content Pane

Alt+Arrow Left

Go to previous location

Alt+Arrow Right

Go to next location

Alt+Enter on item

Show properties of file or folder

Shift+F10 on item

Open context menu of file or folder

Ctrl+Shift+N

Create new folder

Ctrl+Z

Undo an action

Ctrl+Y

Redo an action

Delete

Delete an item and place it into the Recycle Bin

Shift+Delete

Delete an item permanently without placing it into the Recycle Bin

F2

Edit Item. Select name excluding file extension

Ctrl+Tab

Jump to next file name to edit (while in editing mode).

Ctrl+Shift+Tab

Jump to previous file name to edit (while in editing mode).

Ctrl+Shift+1

Change View to Extra Large Icons

Ctrl+Shift+2

Change View to Large Icons

Ctrl+Shift+3

Change View to Medium Icons

Ctrl+Shift+4

Change View to Small Icons

Ctrl+Shift+5

Change View to List View

Ctrl+Shift+6

Change View to Details View

Ctrl+Shift+7

Change View to Tiles View

Ctrl+Shift+8

Change View to Content View

Alt+D, Tab, Tab, Tab, Tab

Jump/ focus Sort Bar (Content Area Table Header)
"""

table = table.split("\n")[::2]
table = [(key, description) for key, description in zip(table[::2], table[1::2])]

from collections import OrderedDict
mapping = OrderedDict()
mapping['then'] = ''
mapping['on item'] = ''
mapping[','] = '|'

mapping['+'] = ' '

mapping['ctrl'] = '^'
mapping['ctr'] = '^'
mapping['alt'] = '%'
mapping['shift'] = '+'
mapping['shft'] = '+'

mapping['pagedown'] = '{PGDN}'
mapping['pageup'] = '{PGUP}'
mapping['home'] = '{HOME}'
mapping['backspace'] = '{BS}'
mapping['backspace key'] = '{BS}'
mapping['esc'] = '{ESC}'
mapping['tab'] = '{TAB}'
mapping['del'] = '{DEL}'
mapping['delete'] = '{DEL}'
mapping['enter'] = '{ENTER}'
mapping['left arrow'] = '{LEFT}'
mapping['arrow left'] = '{LEFT}'
mapping['left'] = '{LEFT}'
mapping['numpadadd'] = '{PLUS}'
mapping['right'] = '{RIGHT}'
mapping['arrow right'] = '{RIGHT}'
mapping['up'] = '{UP}'
mapping['up arrow'] = '{UP}'
mapping['down'] = '{DOWN}'
mapping['down arrow'] = '{DOWN}'
for i in range(1, 10):
    name = 'f' + str(i)
    mapping[name] = '{' + name + '}'
mapping['f10'] = '{fa}'
mapping['f11'] = '{fb}'
mapping['f12'] = '{fc}'
mapping['{fa}'] = '{F10}'
mapping['{fb}'] = '{F11}'
mapping['{fc}'] = '{F12}'
mapping['numpad  (plus)'] = "{PLUS}"
mapping['numpad - (minus)'] = "{SUBTRACT}"
mapping['numpadadd'] = "{PLUS}"
mapping['numpadsub'] = "{SUBTRACT}"
mapping['numpad add'] = "{PLUS}"
mapping['numpad sub'] = "{SUBTRACT}"
mapping['numpad / (divide)'] = "{DIVIDE}"
mapping['numpad * (multiply)'] = "{MULTIPLY}"

post_mapping = OrderedDict()
post_mapping['space'] = ' '

def process_key(key):
    key = key.lower()

    cmapping = mapping.copy()
    while cmapping:
        for k in cmapping:
            if any([(k in k2) for k2 in cmapping if k2 != k]):
                continue
            key = key.replace(k, cmapping[k])
            del cmapping[k]
            break

    key = key.replace(' ', '')
    # key = re.sub(r'\(.*\)', '', key)

    cmapping = post_mapping.copy()
    while cmapping:
        for k in cmapping:
            if any([(k in k2) for k2 in cmapping if k2 != k]):
                continue
            key = key.replace(k, cmapping[k])
            del cmapping[k]
            break
    return key

key_to_function = {}
for key, desc in table:
    key_before = key
    key = process_key(key)
    item = {'action': key, 'description': desc, 'type': 'key'}
    # print(key, key_before, sep='\t')
    print(json.dumps(item), end=',\n')

    if key not in key_to_function:
        key_to_function[key] = desc
    else:
        print('Duplicate key:', key, file=sys.stderr)
        print('Function:', key_to_function[key], file=sys.stderr)
        print('New Function:', desc, file=sys.stderr)