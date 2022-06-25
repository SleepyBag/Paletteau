import re
import json
import sys

table = """Open a new window	Ctrl + n
Open a new window in Incognito mode	Ctrl + Shift + n
Open a new tab, and jump to it	Ctrl + t
Reopen previously closed tabs in the order they were closed	Ctrl + Shift + t
Jump to the next open tab	Ctrl + Tab
Jump to the previous open tab	Ctrl + Shift + Tab
Jump to the 1st tab	Ctrl + 1
Jump to the 2nd tab	Ctrl + 2
Jump to the 3rd tab	Ctrl + 3
Jump to the 4th tab	Ctrl + 4
Jump to the 5th tab	Ctrl + 5
Jump to the 6th tab	Ctrl + 6
Jump to the 7th tab	Ctrl + 7
Jump to the 8th tab	Ctrl + 8
Jump to the rightmost tab	Ctrl + 9
Open the previous page from your browsing history in the current tab	Alt + Left arrow
Open the next page from your browsing history in the current tab	Alt + Right arrow
Close the current tab	Ctrl + w
Close the current window	Ctrl + Shift + w
Minimize the current window	Alt + Space then n
Maximize the current window	Alt + Space then x
Quit Google Chrome	Alt + f then x
Move tabs right or left	Ctrl + Shift + PgUp
Open the Chrome menu	Alt + f
Show or hide the Bookmarks bar	Ctrl + Shift + b
Open the Bookmarks Manager	Ctrl + Shift + o
Open the History page in a new tab	Ctrl + h
Open the Downloads page in a new tab	Ctrl + j
Open the Chrome Task Manager	Shift + Esc
Set focus on the first item in the Chrome toolbar	Shift + Alt + t
Set focus on the rightmost item in the Chrome toolbar	F10 
Switch focus to unfocused dialog (if showing) and all toolbars	F6
Open the Find Bar to search the current page	Ctrl + f
Jump to the next match to your Find Bar search	Ctrl + g
Jump to the previous match to your Find Bar search	Ctrl + Shift + g
Open Developer Tools	Ctrl + Shift + j
Open the Clear Browsing Data options	Ctrl + Shift + Delete
Open the Chrome Help Center in a new tab	F1
Log in a different user or browse as a Guest	Ctrl + Shift + m
Open a feedback form	Alt + Shift + i
Turn on caret browsing	F7
Skip to web contents	Ctrl + F6
Focus on inactive dialogs	Alt + Shift + a
Jump to the address bar	Ctrl + l
Search from anywhere on the page	Ctrl + k
Remove predictions from your address bar	Shift + Delete
Move cursor to the address bar	Control + F5
Open options to print the current page	Ctrl + p
Open options to save the current page	Ctrl + s
Reload the current page	F5
Reload the current page, ignoring cached content	Shift + F5
Stop the page loading	Esc
Browse clickable items moving forward	Tab
Browse clickable items moving backward	Shift + Tab
Open a file from your computer in Chrome	Ctrl + o
Display non-editable HTML source code for the current page	Ctrl + u
Save your current webpage as a bookmark	Ctrl + d
Save all open tabs as bookmarks in a new folder	Ctrl + Shift + d
Turn full-screen mode on or off	F11
Make everything on the page bigger	Ctrl and =
Make everything on the page smaller	Ctrl and -
Scroll down a webpage, a screen at a time	PgDn
Scroll up a webpage, a screen at a time	PgUp
Go to the top of the page	Home
Go to the bottom of the page	End
Move your cursor to the beginning of the previous word in a text field	Ctrl + Left arrow
Move your cursor to the next word	Ctrl + Right arrow
Delete the previous word in a text field	Ctrl + Backspace
Open the Home page in the current tab	Alt + Home
Reset page zoom level	Ctrl + 0"""

table = [line.split('\t') for line in table.split('\n')]
table = [(line[1], line[0]) for line in table]

from collections import OrderedDict
mapping = OrderedDict()
mapping['then'] = '|'
mapping['on item'] = ''
mapping[','] = '|'

mapping['+'] = ' '
mapping['and'] = ' '

mapping['ctrl'] = '^'
mapping['control'] = '^'
mapping['ctr'] = '^'
mapping['alt'] = '%'
mapping['shift'] = '+'
mapping['shft'] = '+'

mapping['pagedown'] = '{PGDN}'
mapping['pgdn'] = '{PGDN}'
mapping['pageup'] = '{PGUP}'
mapping['pgup'] = '{PGUP}'
mapping['home'] = '{HOME}'
mapping['end'] = '{END}'
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
mapping['right arrow'] = '{RIGHT}'
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