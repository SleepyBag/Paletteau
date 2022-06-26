table = """Ctrl + W	Close the Everything window.
F1	Show the Everything help.
Alt + D	Focus and highlight the search edit.
F5	Reload icons, file sizes, dates and attributes.
F11	Toggle fullscreen.
Ctrl + Tab	Go to the next window.
Ctrl + Shift + Tab	Go to the previous window.
Shift + Tab	Cycle between search edit and results view.
Escape	Close the current window.
Alt + 1	Resize the window to 512 x 398.
Alt + 2	Resize the window to 640 x 497.
Alt + 3	Resize the window to 768 x 597.
Alt + 4	Auto size the window.
Alt + P	Toggle the preview pane.
Ctrl + Shift + 1	Toggle extra large thumbnails.
Ctrl + Shift + 2	Toggle large thumbnails.
Ctrl + Shift + 3	Toggle medium thumbnails.
Ctrl + Shift + 6	Toggle detail view.
Ctrl + Alt + +	Increase thumbnail size.
Ctrl + Alt + -	Decrease thumbnail size.
Ctrl + `	Toggle debug console.
Ctrl + F1	Show About Everything.
Ctrl + 1	Sort by name.
Ctrl + 2	Sort by path.
Ctrl + 3	Sort by size.
Ctrl + 4	Sort by extension.
Ctrl + 5	Sort by type.
Ctrl + 6	Sort by date modified.
Ctrl + 7	Sort by date created.
Ctrl + 8	Sort by attributes.
Ctrl + 9	Sort by date recently changed.
Ctrl + B	Toggle match whole word.
Ctrl + D	Bookmark the current search.
Ctrl + I	Toggle match case.
Ctrl + M	Toggle match diacritics.
Ctrl + N	Open a new search window.
Ctrl + O	Open an Everything file list.
Ctrl + P	Show the Options window.
Ctrl + Q	Exit Everything.
Ctrl + R	Toggle Regex.
Ctrl + S	Export the current results to an Everything file list, csv or txt file.
Ctrl + T	Toggle always on top.
Ctrl + U	Toggle match path.
Ctrl + +	Increase text size.
Ctrl + -	Decrease text size.
Ctrl + 0	Reset text size to Normal.
Ctrl + Mouse Wheel Up
Ctrl + Mouse Wheel Down	Change view.
Alt + Home	Go to the home search.
Alt + Left Arrow	Go back to the previous search.
Alt + Right Arrow	Go forward to the next search.
Ctrl + Shift + F	Organize filters.
Ctrl + Shift + B	Organize bookmarks.
Ctrl + Shift + H	Show all search history."""


import json
import re
import sys

table = [line.split('\t') for line in table.split('\n')]
# table = [(key, description) for key, description in zip(table[1::2], table[::2])]

from collections import OrderedDict
mapping = OrderedDict()
mapping['then'] = ''
mapping['on item'] = ''
mapping[', '] = '|'

mapping['ctrl'] = '^'
mapping['ctrl +'] = '^'
mapping['ctr'] = '^'
mapping['alt'] = '%'
mapping['alt +'] = '%'
mapping['shift'] = '$'
mapping['shift +'] = '$'
mapping['shft'] = '$'

mapping['0 (zero)'] = '0'
mapping['left angle bracket (<)'] = ','
mapping['right angle bracket (>)'] = '.'
mapping['period (.)'] = '.'
mapping['left bracket ([)'] = '['
mapping['right bracket (])'] = ']'
mapping['pagedown'] = '{PGDN}'
mapping['page down'] = '{PGDN}'
mapping['pgdn'] = '{PGDN}'
mapping['pageup'] = '{PGUP}'
mapping['page up'] = '{PGUP}'
mapping['pgup'] = '{PGUP}'
mapping['home'] = '{HOME}'
mapping['backspace'] = '{BS}'
mapping['backspace key'] = '{BS}'
mapping['esc'] = '{ESC}'
mapping['escape'] = '{ESC}'
mapping['tab'] = '{TAB}'
mapping['tab key'] = '{TAB}'
mapping['del'] = '{DEL}'
mapping['delete'] = '{DEL}'
mapping['enter'] = '{ENTER}'
mapping['left arrow'] = '{LEFT}'
mapping['left arrow key'] = '{LEFT}'
mapping['left key'] = '{LEFT}'
mapping['arrow left'] = '{LEFT}'
mapping['left'] = '{LEFT}'
mapping['numpadadd'] = '{PLUS}'
mapping['right'] = '{RIGHT}'
mapping['right key'] = '{RIGHT}'
mapping['right arrow'] = '{RIGHT}'
mapping['right arrow key'] = '{RIGHT}'
mapping['arrow right'] = '{RIGHT}'
mapping['up'] = '{UP}'
mapping['up key'] = '{UP}'
mapping['up arrow'] = '{UP}'
mapping['up arrow key'] = '{UP}'
mapping['down'] = '{DOWN}'
mapping['down arrow'] = '{DOWN}'
mapping['down arrow key'] = '{DOWN}'
for i in range(1, 10):
    name = 'f' + str(i)
    mapping[name] = '{' + name + '}'
mapping['f10'] = '{fa}'
mapping['f11'] = '{fb}'
mapping['f12'] = '{fc}'
mapping['{fa}'] = '{F10}'
mapping['{fb}'] = '{F11}'
mapping['{fc}'] = '{F12}'
mapping['numpad  (plus)'] = '{PLUS}'
mapping['numpad - (minus)'] = '{SUBTRACT}'
mapping['minus sign (on the numeric keypad)'] = '{SUBTRACT}'
mapping['minus sign (-)'] = '-'
mapping['hyphen (-)'] = '-'
mapping['plus sign ( )'] = '='
mapping['equal sign ( = )'] = '='
mapping['+'] = '='
mapping['numpadadd'] = '{PLUS}'
mapping['numpadsub'] = '{SUBTRACT}'
mapping['numpad add'] = '{PLUS}'
mapping['numpad sub'] = '{SUBTRACT}'
mapping['numpad / (divide)'] = '{DIVIDE}'
mapping['forward slash (/) (on the numeric keypad)'] = '{DIVIDE}'
mapping['numpad * (multiply)'] = '{MULTIPLY}'
mapping['num *'] = '{MULTIPLY}'
mapping['break'] = '{BREAK}'
mapping['insert'] = '{INSERT}'
mapping['end'] = '{END}'

post_mapping = OrderedDict()
post_mapping['space'] = ' '
post_mapping['spacebar'] = ' '
post_mapping['$'] = '+'

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

def formatted(key):
    return re.match('^([\^\+%]*(.|\{.*\}))(\|[\^\+%]*(.|\{.*\}))*$', key) is not None

key_to_function = {}
for key, desc in table:
    key_before = key
    key = process_key(key)
    item = {'action': key, 'description': desc, 'type': 'key'}

    if key not in key_to_function or desc != key_to_function[key]:
        print(json.dumps(item), end=',\n')

    if not formatted(key):
        print(key, 'not formatted!', file=sys.stderr)
    if key not in key_to_function or desc == key_to_function[key]:
        key_to_function[key] = desc
    else:
        print('Duplicate key:', key, file=sys.stderr)
        print('Function:', key_to_function[key], file=sys.stderr)
        print('New Function:', desc, file=sys.stderr)