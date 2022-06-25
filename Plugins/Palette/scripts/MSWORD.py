import re
import json
import sys

table = """Open a document.

Ctrl+O

Create a new document.

Ctrl+N

Save the document.

Ctrl+S

Close the document.

Ctrl+W

Cut the selected content to the Clipboard.

Ctrl+X

Copy the selected content to the Clipboard.

Ctrl+C

Paste the contents of the Clipboard.

Ctrl+V

Select all document content.

Ctrl+A

Apply bold formatting to text.

Ctrl+B

Apply italic formatting to text.

Ctrl+I

Apply underline formatting to text.

Ctrl+U

Decrease the font size by 1 point.

Ctrl+Left bracket ([)

Increase the font size by 1 point.

Ctrl+Right bracket (])

Center the text.

Ctrl+E

Align the text to the left.

Ctrl+L

Align the text to the right.

Ctrl+R

Undo the previous action.

Ctrl+Z

Redo the previous action, if possible.

Ctrl+Y

Adjust the zoom magnification.

Alt+W, Q

Split the document window.

Ctrl+Alt+S

Remove the document window split.

Alt+Shift+C
Move to the Tell Me or Search field on the Ribbon to search for assistance or Help content.

Alt+Q

Open the File page to use Backstage view.

Alt+F

Open the Home tab to use common formatting commands, paragraph styles, and the Find tool.

Alt+H

Open the Insert tab to insert tables, pictures and shapes, headers, or text boxes.

Alt+N

Open the Design tab to use themes, colors, and effects, such as page borders.

Alt+G

Open the Layout tab to work with page margins, page orientation, indentation, and spacing.

Alt+P

Open the References tab to add a table of contents, footnotes, or a table of citations.

Alt+S

Open the Mailings tab to manage Mail Merge tasks and to work with envelopes and labels.

Alt+M

Open the Review tab to use Spell Check, set proofing languages, and to track and review changes to your document.

Alt+R

Open the View tab to choose a document view or mode, such as Read Mode or Outline view. You can also set the zoom magnification and manage multiple document windows.

Alt+W

Select the active tab on the ribbon and activate the access keys.

F10

Show the tooltip for the ribbon element currently in focus.

Ctrl+Shift+F10

Open the menu for the selected button.

Alt+Down arrow key

Expand or collapse the ribbon.

Ctrl+F1

Open the context menu.

Shift+F10

Move to the submenu when a main menu is open or selected.

Left arrow key

Move the cursor one word to the left.

Ctrl+Left arrow key

Move the cursor one word to the right.

Ctrl+Right arrow key

Move the cursor up by one paragraph.

Ctrl+Up arrow key

Move the cursor down by one paragraph.

Ctrl+Down arrow key

Move the cursor to the top of the screen.

Ctrl+Alt+Page up

Move the cursor to the bottom of the screen.

Ctrl+Alt+Page down

Move the cursor by scrolling the document view up by one screen.

Page up

Move the cursor by scrolling the document view down by one screen.

Page down

Move the cursor to the top of the next page.

Ctrl+Page down

Move the cursor to the top of the previous page.

Ctrl+Page up

Move the cursor to the end of the document.

Ctrl+End

Move the cursor to the beginning of the document.

Ctrl+Home

Move the cursor to the location of the previous revision.

Shift+F5

Cycle through floating shapes, such as text boxes or images.

Ctrl+Alt+5

Display the Navigation task pane, to search within the document content.

Ctrl+F

Display the Go To dialog box, to navigate to a specific page, bookmark, footnote, table, comment, graphic, or other location.

Ctrl+G

Cycle through the locations of the four previous changes made to the document.

Ctrl+Alt+Z

Open the list of browse options to define the type of object to browse by.

Ctrl+Alt+Home

Print the document.

Ctrl+P

Switch to print preview.

Ctrl+Alt+I

Select the word to the left.

Ctrl+Shift+Left arrow key

Select the word to the right.

Ctrl+Shift+Right arrow key

Select from the current position to the beginning of the current line.

Shift+Home

Select from the current position to the end of the current line.

Shift+End

Select from the current position to the beginning of the current paragraph.

Ctrl+Shift+Up arrow key

Select from the current position to the end of the current paragraph.

Ctrl+Shift+Down arrow key

Select from the current position to the top of the screen.

Shift+Page up

Select from the current position to the bottom of the screen.

Shift+Page down

Select from the current position to the beginning of the document.

Ctrl+Shift+Home

Select from the current position to the end of the document.

Ctrl+Shift+End

Select from the current position to the bottom of the window.

Ctrl+Alt+Shift+Page down

Select all document content.

Ctrl+A

Expand the selection. Repeated to expand the selection to the entire word, sentence, paragraph, section, and document.

F8

Reduce the selection.

Shift+F8

Select a vertical block of text.

Ctrl+Shift+F8

Delete one word to the left.

Ctrl+Backspace

Delete one word to the right.

Ctrl+Delete

Open the Clipboard task pane and enable the Office Clipboard, which allows you to copy and paste content between Microsoft Office apps.

Alt+H, F, O

Cut the selected content to the Clipboard.

Ctrl+X

Copy the selected content to the Clipboard.

Ctrl+C

Paste the contents of the Clipboard.

Ctrl+V

Move the selected content to a specific location.

F2

Copy the selected content to a specific location.

Shift+F2

Define an AutoText block with the selected content.

Alt+F3

Cut the selected content to the Spike.

Ctrl+F3

Paste the contents of the Spike.

Ctrl+Shift+F3

Copy the selected formatting.

Ctrl+Shift+C

Paste the selected formatting.

Ctrl+Shift+V

Copy the header or footer used in the previous section of the document.

Alt+Shift+R

Display the Replace dialog box, to find and replace text, specific formatting, or special items.

Ctrl+H

Display the Object dialog box, to insert a file object into the document.

Alt+N, J, J

Insert a SmartArt graphic.

Alt+N, M

Insert a WordArt graphic.

Alt+N, W

Justify the paragraph.

Ctrl+J

Indent the paragraph.

Ctrl+M

Remove a paragraph indent.

Ctrl+Shift+M

Create a hanging indent.

Ctrl+T

Remove a hanging indent.

Ctrl+Shift+T

Remove paragraph formatting.

Ctrl+Q

Apply single spacing to the paragraph.

Ctrl+1

Apply double spacing to the paragraph.

Ctrl+2

Apply 1.5-line spacing to the paragraph.

Ctrl+5

Add or remove space before the paragraph.

Ctrl+0 (zero)

Enable AutoFormat.

Ctrl+Alt+K

Apply the Heading 1 style.

Ctrl+Alt+1

Apply the Heading 2 style.

Ctrl+Alt+2

Apply the Heading 3 style.

Ctrl+Alt+3

Display the Apply Styles task pane.

Ctrl+Shift+S

Display the Styles task pane.

Ctrl+Alt+Shift+S

Display the Font dialog box.

Ctrl+D

Increase the font size.

Ctrl+Shift+Right angle bracket (>)

Decrease the font size.

Ctrl+Shift+Left angle bracket (<)

Increase the font size by 1 point.

Ctrl+Right bracket (])

Decrease the font size by 1 point.

Ctrl+Left bracket ([)

Switch the text between upper case, lower case, and title case.

Shift+F3

Change the text to all upper case.

Ctrl+Shift+A

Hide the selected text.

Ctrl+Shift+H

Apply underline formatting to the words, but not the spaces.

Ctrl+Shift+W

Apply double-underline formatting.

Ctrl+Shift+D

Apply small caps formatting.

Ctrl+Shift+K

Apply subscript formatting.

Ctrl+Equal sign ( = )

Apply superscript formatting.

Ctrl+Shift+Plus sign (+)

Remove manual character formatting.

Ctrl+Spacebar

Change the selected text to the Symbol font.

Ctrl+Shift+Q

Display all nonprinting characters.

Ctrl+Shift+8

Display the Reveal Formatting task pane.

Shift+F1

Insert a line break.

Shift+Enter

Insert a page break.

Ctrl+Enter

Insert a column break.

Ctrl+Shift+Enter

Insert an em dash (—).

Ctrl+Alt+Minus sign (on the numeric keypad)

Insert an en dash (–).

Ctrl+Minus sign (on the numeric keypad)

Insert an optional hyphen.

Ctrl+Hyphen (-)

Insert a nonbreaking hyphen.

Ctrl+Shift+Hyphen (-)

Insert a nonbreaking space.

Ctrl+Shift+Spacebar

Insert a copyright symbol (©).

Ctrl+Alt+C

Insert a registered trademark symbol (®).

Ctrl+Alt+R

Insert a trademark symbol (™).

Ctrl+Alt+T

Insert an ellipsis (…)

Ctrl+Alt+Period (.)

Insert a hyperlink.

Ctrl+K

Go back one page.

Alt+Left arrow key

Go forward one page.

Alt+Right arrow key

Refresh the page.

F9

Move to the first cell in the row.

Alt+Home

Move to the last cell in the row.

Alt+End

Move to the first cell in the column.

Alt+Page up

Move to the last cell in the column.

Alt+Page down

Move to the previous row.

Up arrow key

Move to the next row.

Down arrow key

Move one row up.

Alt+Shift+Up arrow key

Move one row down.

Alt+Shift+Down arrow key

Select the content in the next cell.

Tab key

Select the content in the previous cell.

Shift+Tab

Insert a new paragraph in a cell.

Enter

Insert a tab character in a cell.

Ctrl+Tab

Insert a comment.

Ctrl+Alt+M

Turn change tracking on or off.

Ctrl+Shift+E

Close the Reviewing Pane.

Alt+Shift+C

Mark a table of contents entry.

Alt+Shift+O

Mark a table of authorities entry (citation).

Alt+Shift+I

Choose citation options.

Alt+Shift+F12, Spacebar

Mark an index entry.

Alt+Shift+X

Insert a footnote.

Ctrl+Alt+F

Insert an endnote.

Ctrl+Alt+D

Go to the next footnote.

Alt+Shift+Right angle bracket (>)

Go to the previous footnote.

Alt+Shift+Left angle bracket (<)

Preview the mail merge.

Alt+Shift+K

Merge a document.

Alt+Shift+N

Print the merged document.

Alt+Shift+M

Edit a mail-merge data document.

Alt+Shift+E

Insert a merge field.

Alt+Shift+F

Insert a DATE field.

Alt+Shift+D

Insert a LISTNUM field.

Ctrl+Alt+L

Insert a PAGE field.

Alt+Shift+P

Insert a TIME field.

Alt+Shift+T

Insert an empty field.

Ctrl+F9

Update the linked information in a Word source document.

Ctrl+Shift+F7

Update the selected fields.

F9

Unlink a field.

Ctrl+Shift+F9

Switch between a selected field code and its result.

Shift+F9

Switch between all field codes and their results.

Alt+F9

Run GOTOBUTTON or MACROBUTTON from a field displaying field results.

Alt+Shift+F9

Go to the next field.

F11

Go to the previous field.

Shift+F11

Lock a field.

Ctrl+F11

Unlock a field.

Ctrl+Shift+F11

Display the Language dialog box to set the proofing language.

Alt+R, U, L

Set default languages.

Alt+R, L

Switch to the Read Mode view.

Alt+W, F

Switch to the Print Layout view.

Ctrl+Alt+P

Switch to the Outline view.

Ctrl+Alt+O

Switch to the Draft view.

Ctrl+Alt+N

Promote a paragraph.

Alt+Shift+Left arrow key

Demote a paragraph.

Alt+Shift+Right arrow key

Demote the paragraph to body text.

Ctrl+Shift+N

Expand the text under a heading.

Alt+Shift+Plus sign (+)

Collapse the text under a heading.

Alt+Shift+Minus sign (-)

Expand or collapse all text or headings.

Alt+Shift+A

Hide or display the character formatting.

Forward slash (/) (on the numeric keypad)

Switch between showing the first line of body text and showing all body text.

Alt+Shift+L

Show all headings with the Heading 1 style.

Alt+Shift+1

Move to the beginning of the document.

Home

Move to the end of the document.

End

Exit Read Mode.

Esc"""


table = [line for line in table.split('\n') if line]
table = [(key, description) for key, description in zip(table[1::2], table[::2])]

from collections import OrderedDict
mapping = OrderedDict()
mapping['then'] = ''
mapping['on item'] = ''
mapping[', '] = '|'

mapping['+'] = ' '

mapping['ctrl'] = '^'
mapping['ctr'] = '^'
mapping['alt'] = '%'
mapping['shift'] = '+'
mapping['shft'] = '+'

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
mapping['numpad  (plus)'] = "{PLUS}"
mapping['numpad - (minus)'] = "{SUBTRACT}"
mapping['minus sign (on the numeric keypad)'] = "{SUBTRACT}"
mapping['minus sign (-)'] = "-"
mapping['hyphen (-)'] = "-"
mapping['plus sign ( )'] = "="
mapping['equal sign ( = )'] = "="
mapping['numpadadd'] = "{PLUS}"
mapping['numpadsub'] = "{SUBTRACT}"
mapping['numpad add'] = "{PLUS}"
mapping['numpad sub'] = "{SUBTRACT}"
mapping['numpad / (divide)'] = "{DIVIDE}"
mapping['forward slash (/) (on the numeric keypad)'] = "{DIVIDE}"
mapping['numpad * (multiply)'] = "{MULTIPLY}"
mapping['num *'] = "{MULTIPLY}"
mapping['break'] = "{BREAK}"
mapping['insert'] = "{INSERT}"
mapping['end'] = "{END}"

post_mapping = OrderedDict()
post_mapping['space'] = ' '
post_mapping['spacebar'] = ' '

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