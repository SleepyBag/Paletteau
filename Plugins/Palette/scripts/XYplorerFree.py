import re
import json
import sys

table = """
File Menu
Ctrl+P	Copy Item full name (ie with path) to clipboard either of all selected items in the List if it got the focus, or of selected Tree item (if focus is on Tree, Catalog, etc
Ctrl+Shft+P	Copy name(s) to clipboard (of all selected items in the List)
Ctrl+Shft+Alt+P	Copy path(s) to clipboard (of all selected items in the List)
Ctrl+I	Copy Compact File Info to clipboard
Shft+F2	Opens Batch Rename dialog for selected items in the List
Ctrl+Enter	Open Focused Item
Ctrl+D	Copy the selected item to current location and automatically renames it with a suffix number. This number will be added before the item's extension (even if it is a folder) unless one was already present, if so it will be incremented.
Ctrl+Shft+Alt+D	Copy the selected item to current location and automatically renames it with a suffix based on the current date. This suffix will be added before the item's extension (even if it is a folder). If an item with that name already exist, a suffix number will be automatically added.
Ctrl+Shft+D	Copy the selected item to current location and automatically renames it with a suffix based on the item's last modified date. This suffix will be added before the item's extension (even if it is a folder). If an item with that name already exist, a suffix number will be automatically added.
Ctrl+S	Copy the selected item on the current location and renames it on-the-fly.
F2	Rename
Ctrl+Q	Opens the Preview tab and show the Panel if needed, and starts the preview for the focused item if selected. If there is currently a preview in progress, it will be stopped and the file will be released. Note that if the Panel is visible, the preview tab active, and no files are selected/previewed then Panel will be hidden.
Ctrl+Alt+F4	Exit without Saving
Alt+F4	Exit (with or without Saving according to your Configuration)
Edit Menu
Ctrl+X	Cut
Ctrl+C	Copy
Ctrl+V	Paste
Ctrl+Shft+V	Paste Here to New Subfolder...
Shft+F7	Move To
Ctrl+F7	Copy To
Ctrl+Shft+F7	Backup To
Ctrl+N	Create New Folder
Ctrl+Shft+N	Create new Text file
Ctrl+Alt+N	Create New Path
Ctrl+A	Select All items currently listed
Ctrl+Shft+A	Deselect All all items currently listed
Ctrl+Shft+I	Invert selection
Ctrl+M	Selection Filter
Ctrl+Alt+M	Select all items of the same type(s) as the selected item(s)
Ctrl+Shft+Alt+M	Select all files, not folders
Ctrl+F	Find Files: Show the panel if hidden and activates the Find Files tab
F3	Find Now: Start File Find operation with the current parameter settings
Ctrl+F3	Show All Items In Branch
Shft+F3	Repeat Last Find Files
Ctrl+F9	Open the Search Templates window
View Menu
Ctrl+Shft+F5	Sort Again
Ctrl+Shft+T	Tab List...
Alt+Home	Go Home
Ctrl+L	Lock Tab
Ctr+Shft+W	Close All Other Unlocked Tabs
Ctrl+J	Set Visual Filter...
Ctrl+Shft+J	Toggle No/Last Visual Filter
Ctrl+Alt+J	Filter to only show items with the same extension(s) as the List's selected item(s). If no items are selected, then extension of the focused item is used instead
Ctrl+Shft+Alt+J	Filter to hide all items with the same extension as one of the List's selected item(s). If no items are selected, then extension of the focused item is used instead
Ctrl+NumPadAdd Autosize all columns
Ctrl+Shft+NumPadAdd	Grows the Name column by 5 pixels
Ctrl+Shft+NumPadSub	stractShrinks the Name column by 5 pixels
Ctrl+Shft+E	Toggle Age (date display format)
Ctrl+R	Instantly desactivate any auto-refreshing for the current location temporarily, without affecting any (Auto-Refresh) settings. The auto-refresh will be reactivated as soon as you change location, or if you manually reactivate it.
F4	Refresh Tree
Shft+F4	Refresh Current Folder
Ctrl+Shft+F4	Rebuild Tree: rebuilds the whole folder tree and then carries you back to the location where you were before, while closing all other open branches
F5	Refresh File List: update the current list data, but keep any selections and scroll position.
Ctrl+F5	Refresh and Reset File List: update the data, scroll back to top, set focus to the first item (if any), unselect any selections.
Shft+F5	Calculate Folder Sizes: calculate selected folder sizes if any, otherwise calculate all folders sizes
Ctrl+Shft+O	Same as the option Show folders in list.
Go Menu
F7	Previous Location: jump to the previous location (useful to zap back and fore between two locations)
Ctrl+Alt+F7	Last Target: jump to the most recent target of a move/copy operation that was performed within XY's current session
Backspace	Go up to the parent folder
Shft+Backspace	Go down to the most recently browsed subfolder of the current location (if any) (see Breadcrumb for more)
Ctrl+Backspace	Pop up the Breadcrumb menu. Note that Ctrl+Alt+Backspace (and therefore AltGr+Backspace) is also available for this.
Alt+Left	Go Back (History related)
Alt+Right	Go Forward (History related)
Ctrl+H	Pop up the Hotlist
Ctrl+G	Go to...: enter/paste a location to jump to
Ctrl+Shft+G	Go to from Here...: enter/paste a location to jump to (preset to current path)
Ctrl+Shft+L	Go to Line...: enter/paste a line number to jump to
Favorites Menu
Ctrl+B	Toggle Favorite Folder: toggle favorite status of current folder
Ctrl+Shft+B	Toggle Highlighted Folder: toggle highlighted status of current folder
Ctrl+Alt+B	Toggle Boxed Branch: toggle boxed branch status of current folder
Tools Menu
F9	Opens the Configuration window
Shft+F9	Opens the Customize Keyboard Shortcuts window
Window Menu
Ctrl+Shft+F12	Show Address Bar
Ctrl+F12	Show Toolbars
F8	Show Catalog
F12	Show Info Panel: toggle the Info Panel's visibility
Alt+F12	Last Size/Minimize Info Panel
Shft+F12	Maximize/Minimize Info Panel
Help Menu
F1	Contents and Index: display this help file
Tree
[alphanum. key]	Select the next visible folder whose name starts with that letter or number
Numpad - (minus)	Collapse selected node
Numpad + (plus)	Expand selected node
Numpad / (divide)	Fully collapse selected node
Numpad * (multiply)	Fully expand selected node
[arrow keys etc.]	All common navigation keys just like Explorer
Alt+Enter	Show Properties dialog of the focused item (AltGr+Enter to avoid beep sound)
Ctrl+Home	Go to the top folder (Drive, \\Server, Desktop, MyDocuments) of the current folder
List
[alphanum. key]	Select the next file whose name starts with that letter or number
[arrow keys]	Unique function in XY on Rename only: Up/Down keys unselect any selection inside the edit box by a single keystroke without moving the caret one position to the left or right. This is useful for example when the selection is set to the base excluding the extension (to do this, check Configuration | Advanced | Exclude extension on rename). Note that using the Up key will move the caret to the left end of the selection; using the Down key will move it to the right end.
Del	Delete all currently selected files (Recycle Bin)
Shft+Del	Delete all currently selected files (NO Recycle Bin, ie. they are nuked!)
Ctrl+Del	Delete (Skip Locked) (Recycle bin)
Ctrl+Shft+Del	Delete (Skip Locked) (no recycle bin)
Ctrl+Left Arrow	(find results only) jump to currently focused file in its folder
Ctrl+Numpad Add	Adjust all column widths
Shft+F6	Put the selected item into view, keeping any selection untouched
Esc	Abort running operation (eg. creating thumbnails, File Find, Calculate Folder Sizes, ...)
Catalog
Tabs
Ctrl+T	Open new tab
Ctrl+W	Close current tab
Ctrl+Tab	Cycle Tabs Forward
Ctrl+Shft+Tab	Cycle Tabs Backward
Ctrl+PageDown	Cycle Tabs Forward, Delay Browsing until all keys are up
Ctrl+PageUp	Cycle Tabs Backward, Delay Browsing until all keys are up
Ctrl+Shft+Left	Shift current tab to left
Ctrl+Shft+Right	Shift current tab to right
Info Panel
Ctrl+1	Jump to the 1st Panel tabs
Ctrl+2	Jump to the 2nd Panel tabs
Ctrl+3	Jump to the 3rd Panel tabs
Ctrl+4	Jump to the 4th Panel tabs
Ctrl+5	Jump to the 5th Panel tabs
Ctrl+6	Jump to the 6th Panel tabs
Other
F6	Move focus among Tree, Address Bar, and List
Shft+F11	Open focused web file in new browser window
Ctrl+Up	In lists with shiftable positions: shift selected item up.
Ctrl+Down	In lists with shiftable positions: shift selected item down.
Ctrl+Space	Toggle selected state of focused list item
Ctrl+Shft+H	Show/Hide hidden files and folders
Ctrl+T followed by Alt+Left arrow	Brings back any accidentally closed tab
"""

table = table.split("\n")
table = [line.split("\t") for line in table if "\t" in line]
remove_keywords = ["Any other", "followed", "["]
table = [line for line in table if all([keyword not in line[0] for keyword in remove_keywords])]

from collections import OrderedDict
mapping = OrderedDict()
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
mapping['enter'] = '{ENTER}'
mapping['left arrow'] = '{LEFT}'
mapping['numpadadd'] = '{PLUS}'
mapping['left'] = '{LEFT}'
mapping['right'] = '{RIGHT}'
mapping['up'] = '{UP}'
mapping['down'] = '{DOWN}'
for i in range(1, 10):
    name = 'f' + str(i)
    mapping[name] = '{' + name + '}'
mapping['f10'] = '{FA}'
mapping['f11'] = '{FB}'
mapping['f12'] = '{FC}'
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
        