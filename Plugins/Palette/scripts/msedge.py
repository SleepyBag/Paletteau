# this script process the documentation copied from
# https://support.microsoft.com/en-us/microsoft-edge/keyboard-shortcuts-in-microsoft-edge-50d3edab-30d9-c7e4-21ce-37fe2713cfad#ID0EBBD=Windows
# NOTICE that there are still some manual work left
import re
import json

edge = """Ctrl + Shift + B

Show or hide the favorites bar

Alt + Shift + B

Set focus on the first item in the favorites bar

Ctrl + D

Save the current tab as a favorite

Ctrl + Shift + D

Save all open tabs as favorites in a new folder

Alt + D

Select the URL in the address bar to edit

Ctrl + E

Open a search query in the address bar

Alt + E

Open the Settings and more "..." menu

Ctrl + F

Find on page

Alt + F

Open the Settings and more "..." menu

Ctrl + G

Jump to the next match to your Find Bar search

Ctrl + Shift + G

Jump to the previous match to your Find Bar search

Ctrl + H

Open History in a new tab (web UI)

Ctrl + Shift + I

Open Developer Tools

Alt + Shift + I

Open the Send feedback dialog

Ctrl + J

Open Downloads in a new tab (web UI)

Ctrl + K

Open a search query in the address bar

Ctrl + Shift + K

Duplicate the current tab

Ctrl + L

Select the URL in the address bar to edit

Ctrl + Shift + L

Paste and search or Paste and go (if it's a URL)

Ctrl + M

Mute the current tab (toggle)

Ctrl + Shift + M

Sign in as a different user or browse as a Guest

Ctrl + N

Open a new window

Ctrl + Shift + N

Open a new InPrivate window

Ctrl + O

Open a file from your computer in Edge

Ctrl + Shift + O

Open Favorites management

Ctrl + P

Print the current page

Ctrl + Shift + P

Print using the system dialog

Ctrl + R

Reload the current page

Ctrl + Shift + R

Reload the current page, ignoring cached content

Ctrl + S

Save the current page

Ctrl + T

Open a new tab and switch to it

Ctrl + Shift + T

Reopen the last closed tab, and switch to it

Alt + Shift + T

Set focus on the first item in the toolbar

Ctrl + U

View source

Ctrl + Shift + U

Start or stop Read Aloud

Ctrl + Shift + V

Paste without formatting

Ctrl + W

Close the current tab

Ctrl + Shift + W

Close the current window

Ctrl + Shift + Y

Open Collections

Ctrl + 0 (zero)

Reset zoom level

Ctrl + 1, 2, ... 8

Switch to a specific tab

Ctrl + 9

Switch to the last tab

Ctrl + Enter

Add www. to the beginning and .com to the end of text typed in the address bar

Ctrl + Tab

Switch to the next tab

Ctrl + Shift + Tab

Switch to the previous tab

Ctrl + Plus (+)

Zoom in

Ctrl + Minus (-)

Zoom out

Ctrl + \ (in a PDF)

Toggle PDF between fit to page / fit to width

Ctrl + [ (in a PDF)

Rotate PDF counter-clockwise 90*

Ctrl + ] (in a PDF)

Rotate PDF clockwise 90*

Ctrl + Shift + Delete

Open clear browsing data options

Alt

Set focus on the Settings and more "…" button

Alt + Left arrow

Go back

Alt + Right arrow

Go forward

Alt + Home

Open your home page in the current tab

Alt + F4

Close the current window

F1

Open Help

F3

Find in the current tab

F4

Select the URL in the address bar

Ctrl + F4

Close the current page in the current tab

F5

Reload the current tab

Shift + F5

Reload the current tab, ignoring cached content

F6

Switch focus to next pane

Shift + F6

Switch focus to previous pane

F7

Turn caret browsing on or off

F9

Enter or exit Immersive Reader

F10

Set focus on the Settings and more "…" button

F10 + Enter

Open Setting and more "…" menu

Shift + F10

Open context menu

F11

Enter full screen (toggle)

F12

Open Developer Tools

Esc

Stop loading page; close dialog or pop-up

Spacebar

Scroll down webpage, one screen at a time

Shift + Spacebar

Scroll up webpage, one screen at a time

PgDn

Scroll down webpage, one screen at a time

Ctrl + PgDn

Switch to the next tab

PgUp

Scroll up webpage, one screen at a time

Ctrl + PgUp

Switch to the previous tab

Home

Go to the top of the page, Move keyboard focus to first item of pane

End

Go to the bottom of the page, Move keyboard focus to last item of pane

Tab

Go to next tab stop

Shift + Tab

Go to previous tab stop"""
edgelines = edge.split('\n\n')

mapping = {
    'Enter': '{ENTER}',
    'Tab': '{TAB}',
    'Plus': '{+}',
    'Minus': '-',
    'Delete': '{DELETE}',
    'Leftarrow': '{LEFT}',
    'Rightarrow': '{RIGHT}',
    'Home': '{HOME}',
    'End': '{END}',
    'PgDn': '{PGDN}',
    'PgUp': '{PGUP}',
    'Spacebar': ' ',
}
for i in range(1, 10):
    name = 'F' + str(i)
    mapping[name] = '{' + name + '}'
mapping['{F1}0'] = '{F10}'
mapping['{F1}1'] = '{F11}'
mapping['{F1}2'] = '{F12}'

def process_key(key):
    key = key.replace('+', '')
    key = key.replace('Ctrl', '^')
    key = key.replace('Alt', '%')
    key = key.replace('Shift', '+')
    key = key.replace(' ', '')
    key = re.sub(r'\(.*\)', '', key)
    for k in mapping:
        key = key.replace(k, mapping[k])
    return key

for key, desc in zip(edgelines[::2], edgelines[1::2]):
    item = {'action': key, 'description': desc, 'type': 'key'}
    print(json.dumps(item))