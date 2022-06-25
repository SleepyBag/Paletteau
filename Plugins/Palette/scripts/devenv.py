import re
import json
import sys

table = """Build solution	Ctrl+Shift+B	Build.BuildSolution
Cancel	Ctrl+Break	Build.Cancel
Compile	Ctrl+F7	Build.Compile
Run code analysis on solution	Alt+F11	Build.RunCodeAnalysisonSolution
Break all	Ctrl+Alt+Break	Debug.BreakAll
Delete all breakpoints	Ctrl+Shift+F9	Debug.DeleteAllBreakpoints
Exceptions	Ctrl+Alt+E	Debug.Exceptions
Quick watch	Ctrl+Alt+Q 	Debug.QuickWatch
Restart	Ctrl+Shift+F5	Debug.Restart
Run to cursor	Ctrl+F10	Debug.RunToCursor
Set next statement	Ctrl+Shift+F10	Debug.SetNextStatement
Start	F5	Debug.Start
Start without debugging	Ctrl+F5	Debug.StartWithoutDebugging
Step into	F11	Debug.StepInto
Step out	Shift+F11	Debug.StepOut
Stop debugging	Shift+F5	Debug.StopDebugging
Toggle breakpoint	F9	Debug.ToggleBreakpoint
Collapse to definitions	Ctrl+M, Ctrl+O 	Edit.CollapseToDefinitions [Text Editor]
Comment selection	Ctrl+K, Ctrl+C 	Edit.CommentSelection [Text Editor]
Complete word	Alt+Right Arrow  	Edit.CompleteWord
Copy	Ctrl+C 	Edit.Copy
Cut	Ctrl+X 	Edit.Cut
Find	Ctrl+F	Edit.Find
Find all references	Shift+F12	Edit.FindAllReferences
Find in files	Ctrl+Shift+F	Edit.FindinFiles
Find next	F3	Edit.FindNext
Find next selected	Ctrl+F3	Edit.FindNextSelected
Format document	Ctrl+K, Ctrl+D 	Edit.FormatDocument [Text Editor]
Format selection	Ctrl+K, Ctrl+F 	Edit.FormatSelection [Text Editor]
Go to	Ctrl+G	Edit.GoTo
Go to declaration	Ctrl+F12	Edit.GoToDeclaration
Go to definition	F12	Edit.GoToDefinition
Go to next location	F8	Edit.GoToNextLocation
Insert snippet	Ctrl+K, Ctrl+X	Edit.InsertSnippet
Line cut	Ctrl+L 	Edit.LineCut [Text Editor]
Line down extend column	Shift+Alt+Down Arrow 	Edit.LineDownExtendColumn [Text Editor]
Line open above	Ctrl+Enter 	Edit.LineOpenAbove [Text Editor]
List members	Ctrl+J 	Edit.ListMembers
Navigate to	Ctrl+,	Edit.NavigateTo
Open file	Ctrl+Shift+G	Edit.OpenFile
Overtype mode	Insert 	Edit.OvertypeMode [Text Editor]
Parameter info	Ctrl+Shift+Spacebar 	Edit.ParameterInfo
Paste	Ctrl+V 	Edit.Paste
Peek definition	Alt+F12 	Edit.PeekDefinition [Text Editor]
Redo	Ctrl+Y 	Edit.Redo
Replace	Ctrl+H	Edit.Replace
Select all	Ctrl+A	Edit.SelectAll
Select current word	Ctrl+W 	Edit.SelectCurrentWord [Text Editor]
Surround with	Ctrl+K, Ctrl+S 	Edit.SurroundWith (available only in Visual Studio 2019 and earlier)
Tab left	Shift+Tab 	Edit.TabLeft [Text Editor, Report Designer, Windows Forms Editor]
Toggle all outlining	Ctrl+M, Ctrl+L 	Edit.ToggleAllOutlining [Text Editor]
Toggle bookmark	Ctrl+K, Ctrl+K 	Edit.ToggleBookmark [Text Editor]
Toggle completion mode	Ctrl+Alt+Space 	Edit.ToggleCompletionMode [Text Editor]
Toggle outlining expansion	Ctrl+M, Ctrl+M 	Edit.ToggleOutliningExpansion [Text Editor]
Uncomment selection	Ctrl+K, Ctrl+U 	Edit.UncommentSelection [Text Editor]
Undo	Ctrl+Z 	Edit.Undo
Word delete to end	Ctrl+Delete 	Edit.WordDeleteToEnd [Text Editor]
Word delete to start	Ctrl+Backspace 	Edit.WordDeleteToStart [Text Editor]
Exit	Alt+F4	File.Exit
New file	Ctrl+N	File.NewFile
New project	Ctrl+Shift+N	File.NewProject
New web site	Shift+Alt+N	File.NewWebSite
Open file	Ctrl+O	File.OpenFile
Open project	Ctrl+Shift+O	File.OpenProject
Open web site	Shift+Alt+O	File.OpenWebSite
Rename	F2 	File.Rename [Team Explorer]
Save all	Ctrl+Shift+S	File.SaveAll
Save selected items	Ctrl+S	File.SaveSelectedItems
View in browser	Ctrl+Shift+W	File.ViewinBrowser
Add existing item	Shift+Alt+A	Project.AddExistingItem
Add new item	Ctrl+Shift+A	Project.AddNewItem
Extract method	Ctrl+R, Ctrl+M	Refactor.ExtractMethod
Attach to process	Ctrl+Alt+P	Tools.AttachtoProcess
Class view	Ctrl+Shift+C	View.ClassView
Error list	Ctrl+\, Ctrl+E 	View.ErrorList
Navigate backward	Ctrl+-	View.NavigateBackward
Navigate forward	Ctrl+Shift+-	View.NavigateForward
Object browser	Ctrl+Alt+J	View.ObjectBrowser
Output	Ctrl+Alt+O	View.Output
Properties window	F4	View.PropertiesWindow
Server explorer	Ctrl+Alt+S	View.ServerExplorer
Show smart tag	Ctrl+.  	View.ShowSmartTag
Solution explorer	Ctrl+Alt+L	View.SolutionExplorer
TFS Team Explorer	Ctrl+\, Ctrl+M	View.TfsTeamExplorer
Toolbox	Ctrl+Alt+X	View.Toolbox
View designer	Shift+F7 	View.ViewDesigner [HTML Editor Source View]
Activate document window	Esc	Window.ActivateDocumentWindow
Close document window	Ctrl+F4	Window.CloseDocumentWindow
Next document window	Ctrl+F6	Window.NextDocumentWindow
Next document window nav	Ctrl+Tab	Window.NextDocumentWindowNav
Next split pane	F6	Window.NextSplitPane
Navigate backward	Shift+Alt+3	Analyze.NavigateBackward
Navigate forward	Shift+Alt+4	Analyze.NavigateForward
New diagram	Ctrl+\, Ctrl+N	Architecture.NewDiagram
Retry mobile service script operation	Ctrl+Num *, Ctrl+R	WindowsAzure.RetryMobileServiceScriptOperation
Show mobile service script error details	Ctrl+Num *, Ctrl+D	WindowsAzure.ShowMobileServiceScriptErrorDetails
Build selection	Ctrl+B 	Build.BuildSelection (Visual Studio 2019)
Build solution	Ctrl+Shift+B	Build.BuildSolution
Cancel	Ctrl+Break	Build.Cancel
Compile	Ctrl+F7	Build.Compile
Run code analysis on solution	Alt+F11	Build.RunCodeAnalysisonSolution
Properties	Alt+Enter	ClassViewContextMenus.ClassViewMultiselectProjectreferencesItems.Properties
Apply code changes	Alt+F10	Debug.ApplyCodeChanges
Autos	Ctrl+Alt+V, A	Debug.Autos
Break all	Ctrl+Alt+Break	Debug.BreakAll
Breakpoints	Ctrl+Alt+B	Debug.Breakpoints
Call stack	Ctrl+Alt+C	Debug.CallStack
Delete all breakpoints	Ctrl+Shift+F9	Debug.DeleteAllBreakpoints
Launch	Alt+F2	Debug.DiagnosticsHub.Launch
Disassembly	Ctrl+Alt+D	Debug.Disassembly
Dom explorer	Ctrl+Alt+V, D	Debug.DOMExplorer
Enable breakpoint	Ctrl+F9	Debug.EnableBreakpoint
Exceptions	Ctrl+Alt+E	Debug.Exceptions
Function breakpoint	Ctrl+K, B	Debug.FunctionBreakpoint
Go to previous call or IntelliTrace event	Ctrl+Shift+F11	Debug.GoToPreviousCallorIntelliTraceEvent
Start diagnostics	Alt+F5	Debug.Graphics.StartDiagnostics
Immediate	Ctrl+Alt+I	Debug.Immediate
IntelliTrace calls	Ctrl+Alt+Y, T	Debug.IntelliTraceCalls
IntelliTrace events	Ctrl+Alt+Y, F	Debug.IntelliTraceEvents
JavaScript console	Ctrl+Alt+V, C	Debug.JavaScriptConsole
Locals	Ctrl+Alt+V, L	Debug.Locals
Process combo	Ctrl+5	Debug.LocationToolbar.ProcessCombo
Stack frame combo	Ctrl+7	Debug.LocationToolbar.StackFrameCombo
Thread combo	Ctrl+6	Debug.LocationToolbar.ThreadCombo
Toggle current thread flagged state	Ctrl+8	Debug.LocationToolbar.ToggleCurrentThreadFlaggedState
Toggle flagged threads	Ctrl+9	Debug.LocationToolbar.ToggleFlaggedThreads
Memory 1	Ctrl+Alt+M, 1	Debug.Memory1
Memory 2	Ctrl+Alt+M, 2	Debug.Memory2
Memory 3	Ctrl+Alt+M, 3	Debug.Memory3
Memory 4	Ctrl+Alt+M, 4	Debug.Memory4
Modules	Ctrl+Alt+U	Debug.Modules
Parallel stacks	Ctrl+Shift+D, S	Debug.ParallelStacks
Parallel watch 1	Ctrl+Shift+D, 1	Debug.ParallelWatch1
Parallel watch 2	Ctrl+Shift+D, 2	Debug.ParallelWatch2
Parallel watch 3	Ctrl+Shift+D, 3	Debug.ParallelWatch3
Parallel watch 4	Ctrl+Shift+D, 4	Debug.ParallelWatch4
Processes	Ctrl+Alt+Z	Debug.Processes
Quick watch	Shift+F9	Debug.QuickWatch
Re attach to process	Shift+Alt+P	Debug.ReattachtoProcess
Refresh windowsapp	Ctrl+Shift+R	Debug.RefreshWindowsapp
Registers	Ctrl+Alt+G	Debug.Registers
Restart	Ctrl+Shift+F5	Debug.Restart
Run to cursor	Ctrl+F10	Debug.RunToCursor
Set next statement	Ctrl+Shift+F10	Debug.SetNextStatement
Show call stack on code map	Ctrl+Shift+`	Debug.ShowCallStackonCodeMap
Show next statement	Alt+Num *	Debug.ShowNextStatement
Start	F5	Debug.Start
Start windows phone application analysis	Alt+F1	Debug.StartWindowsPhoneApplicationAnalysis
Start without debugging	Ctrl+F5	Debug.StartWithoutDebugging
Step into	F11	Debug.StepInto
Step into current process	Ctrl+Alt+F11	Debug.StepIntoCurrentProcess
Step into specific	Shift+Alt+F11	Debug.StepIntoSpecific
Step out	Shift+F11	Debug.StepOut
Step out current process	Ctrl+Shift+Alt+F11	Debug.StepOutCurrentProcess
Step over	F10 	Debug.StepOver (When not debugging: Starts debugging and stops on the first line of user code)
Step over current process	Ctrl+Alt+F10	Debug.StepOverCurrentProcess
Stop debugging	Shift+F5	Debug.StopDebugging
Stop performance analysis	Shift+Alt+F2	Debug.StopPerformanceAnalysis
Tasks	Ctrl+Shift+D, K	Debug.Tasks
Threads	Ctrl+Alt+H	Debug.Threads
Toggle breakpoint	F9	Debug.ToggleBreakpoint
Toggle disassembly	Ctrl+F11	Debug.ToggleDisassembly
Watch 1	Ctrl+Alt+W, 1	Debug.Watch1
Watch 2	Ctrl+Alt+W, 2	Debug.Watch2
Watch 3	Ctrl+Alt+W, 3	Debug.Watch3
Watch 4	Ctrl+Alt+W, 4	Debug.Watch4
Delete	Alt+F9, D	DebuggerContextMenus.BreakpointsWindow.Delete
Go to disassembly	Alt+F9, A	DebuggerContextMenus.BreakpointsWindow.GoToDisassembly
Go to source code	Alt+F9, S	DebuggerContextMenus.BreakpointsWindow.GoToSourceCode
Stop collection	Ctrl+Alt+F2	DiagnosticsHub.StopCollection
Copy	Ctrl+C	Edit.Copy
Cut	Ctrl+X	Edit.Cut
Cycle clipboard ring	Ctrl+Shift+V	Edit.CycleClipboardRing
Delete	Delete	Edit.Delete
Duplicate	Ctrl+D	Edit.Duplicate
Find	Ctrl+F	Edit.Find
Find all references	Shift+F12	Edit.FindAllReferences
Find in files	Ctrl+Shift+F	Edit.FindinFiles
Find next	F3	Edit.FindNext
Find next selected	Ctrl+F3	Edit.FindNextSelected
Find previous	Shift+F3	Edit.FindPrevious
Find previous selected	Ctrl+Shift+F3	Edit.FindPreviousSelected
Generate method	Ctrl+K, Ctrl+M	Edit.GenerateMethod
Go to	Ctrl+G	Edit.GoTo
Go to all	Ctrl+T	Edit.GoToAll
Go to declaration	Ctrl+F12	Edit.GoToDeclaration
Go to definition	F12	Edit.GoToDefinition
Go to member	Ctrl+1, Ctrl+M	Edit.GoToMember
Go to next location	F8	Edit.GoToNextLocation
Go to prev location	Shift+F8	Edit.GoToPrevLocation
Insert snippet	Ctrl+K, Ctrl+X	Edit.InsertSnippet
Move control down	Ctrl+Down Arrow	Edit.MoveControlDown
Move control down grid	Down Arrow	Edit.MoveControlDownGrid
Move control left	Ctrl+Left Arrow	Edit.MoveControlLeft
Move control left grid	Left Arrow	Edit.MoveControlLeftGrid
Move control right	Ctrl+Right Arrow	Edit.MoveControlRight
Move control right grid	Right Arrow	Edit.MoveControlRightGrid
Move control up	Ctrl+Up Arrow	Edit.MoveControlUp
Move control up grid	Up Arrow	Edit.MoveControlUpGrid
Next bookmark	Ctrl+K, Ctrl+N	Edit.NextBookmark
Next bookmark in folder	Ctrl+Shift+K, Ctrl+Shift+N	Edit.NextBookmarkInFolder
Open file	Ctrl+Shift+G (Opens the file name under the cursor)	Edit.OpenFile
Paste	Ctrl+V	Edit.Paste
Previous bookmark	Ctrl+K, Ctrl+P	Edit.PreviousBookmark
Previous bookmark in folder	Ctrl+Shift+K, Ctrl+Shift+P	Edit.PreviousBookmarkInFolder
Quick find symbol	Shift+Alt+F12	Edit.QuickFindSymbol
Redo	Ctrl+Y	Edit.Redo
Refresh remote references	Ctrl+Shift+J	Edit.RefreshRemoteReferences
Replace	Ctrl+H	Edit.Replace
Replace in files	Ctrl+Shift+H	Edit.ReplaceinFiles
Select all	Ctrl+A	Edit.SelectAll
Select next control	Tab	Edit.SelectNextControl
Select previous control	Shift+Tab	Edit.SelectPreviousControl
Show tile grid	Enter	Edit.ShowTileGrid
Size control down	Ctrl+Shift+Down Arrow	Edit.SizeControlDown
Size control down grid	Shift+Down Arrow	Edit.SizeControlDownGrid
Size control left	Ctrl+Shift+Left Arrow	Edit.SizeControlLeft
Size control left grid	Shift+Left Arrow	Edit.SizeControlLeftGrid
Size control right	Ctrl+Shift+Right Arrow	Edit.SizeControlRight
Size control right grid	Shift+Right Arrow	Edit.SizeControlRightGrid
Size control up	Ctrl+Shift+Up Arrow	Edit.SizeControlUp
Size control up grid	Shift+Up Arrow	Edit.SizeControlUpGrid
Stop search	Alt+F3, S	Edit.StopSearch
Undo	Ctrl+Z	Edit.Undo
Breakpoint conditions	Alt+F9, C	EditorContextMenus.CodeWindow.Breakpoint.BreakpointConditions
Breakpoint edit labels	Alt+F9, L	EditorContextMenus.CodeWindow.Breakpoint.BreakpointEditlabels
Insert temporary breakpoint	Shift+Alt+F9, T	EditorContextMenus.CodeWindow.Breakpoint.InsertTemporaryBreakpoint
Show item	Ctrl+`	EditorContextMenus.CodeWindow.CodeMap.ShowItem
Execute	Ctrl+Alt+F5	EditorContextMenus.CodeWindow.Execute
Go to view	Ctrl+M, Ctrl+G	EditorContextMenus.CodeWindow.GoToView
Toggle header code file	Ctrl+K, Ctrl+O 	EditorContextMenus.CodeWindow.ToggleHeaderCodeFile
View call hierarchy	Ctrl+K, Ctrl+T	EditorContextMenus.CodeWindow.ViewCallHierarchy
Exit	Alt+F4	File.Exit
New file	Ctrl+N	File.NewFile
New project	Ctrl+Shift+N	File.NewProject
New web site	Shift+Alt+N	File.NewWebSite
Open file	Ctrl+O 	File.OpenFile
Open project	Ctrl+Shift+O 	File.OpenProject
Open web site	Shift+Alt+O 	File.OpenWebSite
Print	Ctrl+P	File.Print
Save all	Ctrl+Shift+S	File.SaveAll
Save selected items	Ctrl+S	File.SaveSelectedItems
View in browser	Ctrl+Shift+W	File.ViewinBrowser
Add and remove help content	Ctrl+Alt+F1	Help.AddandRemoveHelpContent
F1 help	F1	Help.F1Help
View help	Ctrl+F1	Help.ViewHelp
Window help	Shift+F1	Help.WindowHelp
Jump to counter pane	Ctrl+R, Q	LoadTest.JumpToCounterPane
Add new diagram	Insert	OtherContextMenus.MicrosoftDataEntityDesignContext.AddNewDiagram
Add existing item	Shift+Alt+A	Project.AddExistingItem
Add new item	Ctrl+Shift+A	Project.AddNewItem
Class wizard	Ctrl+Shift+X	Project.ClassWizard
Override	Ctrl+Alt+Ins	Project.Override
Preview changes	Alt+; then Alt+C	Project.Previewchanges
Publish selected files	Alt+; then Alt+P	Project.Publishselectedfiles
Replace selected files from server	Alt+; then Alt+R	Project.Replaceselectedfilesfromserver
Move down	Alt+Down Arrow	ProjectandSolutionContextMenus.Item.MoveDown
Move up	Alt+Up Arrow	ProjectandSolutionContextMenus.Item.MoveUp
Encapsulate field	Ctrl+R, Ctrl+E	Refactor.EncapsulateField
Extract interface	Ctrl+R, Ctrl+I	Refactor.ExtractInterface
Extract method	Ctrl+R, Ctrl+M	Refactor.ExtractMethod
Remove parameters	Ctrl+R, Ctrl+V	Refactor.RemoveParameters
Rename	Ctrl+R, Ctrl+R	Refactor.Rename
Reorder parameters	Ctrl+R, Ctrl+O 	Refactor.ReorderParameters
Open files filter	Ctrl+[, O	SolutionExplorer.OpenFilesFilter
Pending changes filter	Ctrl+[, P	SolutionExplorer.PendingChangesFilter
Sync with active document	Ctrl+[, S	SolutionExplorer.SyncWithActiveDocument
Go to git branches	Ctrl+0 , Ctrl+N	Team.Git.GoToGitBranches
Go to git changes	Ctrl+0 , Ctrl+G	Team.Git.GoToGitChanges
Go to git commits	Ctrl+0 , Ctrl+O	Team.Git.GoToGitCommits
Team explorer search	Ctrl+'	Team.TeamExplorerSearch
Go to builds	Ctrl+0 , Ctrl+B	TeamFoundationContextMenus.Commands.GoToBuilds
Go to connect	Ctrl+0 , Ctrl+C	TeamFoundationContextMenus.Commands.GoToConnect
Go to documents	Ctrl+0 , Ctrl+D	TeamFoundationContextMenus.Commands.GoToDocuments
Go to home	Ctrl+0 , Ctrl+H	TeamFoundationContextMenus.Commands.GoToHome
Go to my work	Ctrl+0 , Ctrl+M	TeamFoundationContextMenus.Commands.GoToMyWork
Go to pending changes	Ctrl+0 , Ctrl+P	TeamFoundationContextMenus.Commands.GoToPendingChanges
Go to reports	Ctrl+0 , Ctrl+R	TeamFoundationContextMenus.Commands.GoToReports
Go to settings	Ctrl+0 , Ctrl+S	TeamFoundationContextMenus.Commands.GoToSettings
Go to web access	Ctrl+0 , Ctrl+A	TeamFoundationContextMenus.Commands.GoToWebAccess
Go to work items	Ctrl+0 , Ctrl+W	TeamFoundationContextMenus.Commands.GoToWorkItems
Use coded ui test builder	Ctrl+\, Ctrl+C	Test.UseCodedUITestBuilder
Use existing action recording	Ctrl+\, Ctrl+A	Test.UseExistingActionRecording
Debug all tests	Ctrl+R, Ctrl+A	TestExplorer.DebugAllTests
Debug all tests in context	Ctrl+R, Ctrl+T	TestExplorer.DebugAllTestsInContext
Debug last run	Ctrl+R, D	TestExplorer.DebugLastRun
Repeat last run	Ctrl+R, L	TestExplorer.RepeatLastRun
Run all tests	Ctrl+R, A	TestExplorer.RunAllTests
Run all tests in context	Ctrl+R, T	TestExplorer.RunAllTestsInContext
Open tab	Ctrl+E, L	LiveUnitTesting.OpenTab
Code coverage results	Ctrl+E, C	Test.CodeCoverageResults
Attach to process	Ctrl+Alt+P	Tools.AttachtoProcess
Code snippets manager	Ctrl+K, Ctrl+B	Tools.CodeSnippetsManager
Force gc	Ctrl+Shift+Alt+F12, Ctrl+Shift+Alt+F12	Tools.ForceGC
All windows	Shift+Alt+M	View.AllWindows
Architecture explorer	Ctrl+\, Ctrl+R	View.ArchitectureExplorer
Backward	Alt+Left Arrow 	View.Backward (Functions differently from View.NavigateBackward in Text Editor)
Bookmark window	Ctrl+K, Ctrl+W	View.BookmarkWindow
Browse next	Ctrl+Shift+1	View.BrowseNext
Browse previous	Ctrl+Shift+2	View.BrowsePrevious
Call hierarchy	Ctrl+Alt+K	View.CallHierarchy
Class view	Ctrl+Shift+C	View.ClassView
Class view go to search combo	Ctrl+K, Ctrl+V	View.ClassViewGoToSearchCombo
Code definition window	Ctrl+\, D	View.CodeDefinitionWindow
Command window	Ctrl+Alt+A	View.CommandWindow
Data sources	Shift+Alt+D	View.DataSources
Document outline	Ctrl+Alt+T	View.DocumentOutline
Edit label	F2	View.EditLabel
Error list	Ctrl+\, E	View.ErrorList
F# interactive	Ctrl+Alt+F	View.F#Interactive
Find symbol results	Ctrl+Alt+F12	View.FindSymbolResults
Forward	Alt+Right Arrow 	View.Forward (Functions differently from View.NavigateForward in Text Editor)
Forward browse context	Ctrl+Shift+7	View.ForwardBrowseContext
Full screen	Shift+Alt+Enter	View.FullScreen
Navigate backward	Ctrl+-	View.NavigateBackward
Navigate forward	Ctrl+Shift+-	View.NavigateForward
Next error	Ctrl+Shift+F12	View.NextError
Notifications	Ctrl+W, N	View.Notifications
Object browser	Ctrl+Alt+J	View.ObjectBrowser
Object browser go to search combo	Ctrl+K, Ctrl+R	View.ObjectBrowserGoToSearchCombo
Output	Ctrl+Alt+O 	View.Output
Pop browse context	Ctrl+Shift+8 	View.PopBrowseContext (C++ only)
Properties window	F4	View.PropertiesWindow
Property pages	Shift+F4	View.PropertyPages
Resource view	Ctrl+Shift+E	View.ResourceView
Server explorer	Ctrl+Alt+S	View.ServerExplorer
Show smart tag	Shift+Alt+F10	View.ShowSmartTag
Solution explorer	Ctrl+Alt+L	View.SolutionExplorer
SQL Server object explorer	Ctrl+\, Ctrl+S	View.SQLServerObjectExplorer
Task list	Ctrl+\, T	View.TaskList
TFS team explorer	Ctrl+\, Ctrl+M	View.TfsTeamExplorer
Toolbox	Ctrl+Alt+X	View.Toolbox
UML model explorer	Ctrl+\, Ctrl+U	View.UMLModelExplorer
View code	F7	View.ViewCode
Web browser	Ctrl+Alt+R	View.WebBrowser
Zoom in	Ctrl+Shift+.	View.ZoomIn
Zoom out	Ctrl+Shift+,	View.ZoomOut
Show Test Explorer	Ctrl+E, T	TestExplorer.ShowTestExplorer
Activate document window	Esc	Window.ActivateDocumentWindow
Add tab to selection	Ctrl+Shift+Alt+Space	Window.AddTabtoSelection
Close document window	Ctrl+F4	Window.CloseDocumentWindow
Close tool window	Shift+Esc	Window.CloseToolWindow
Keep tab open	Ctrl+Alt+Home	Window.KeepTabOpen
Move to navigation bar	Ctrl+F2	Window.MovetoNavigationBar
Next document window	Ctrl+F6	Window.NextDocumentWindow
Next document window nav	Ctrl+Tab	Window.NextDocumentWindowNav
Next pane	Alt+F6	Window.NextPane
Next split pane	F6	Window.NextSplitPane
Next tab	Ctrl+Alt+PgDn	Window.NextTab
Next tab and add to selection	Ctrl+Shift+Alt+PgDn	Window.NextTabandAddtoSelection
Next tool window nav	Alt+F7	Window.NextToolWindowNav
Previous document window	Ctrl+Shift+F6	Window.PreviousDocumentWindow
Previous document window nav	Ctrl+Shift+Tab	Window.PreviousDocumentWindowNav
Previous pane	Shift+Alt+F6	Window.PreviousPane
Previous split pane	Shift+F6	Window.PreviousSplitPane
Previous tab	Ctrl+Alt+PgUp	Window.PreviousTab
Previous tab and add to selection	Ctrl+Shift+Alt+PgUp	Window.PreviousTabandAddtoSelection
Previous tool window nav	Shift+Alt+F7	Window.PreviousToolWindowNav
Quick launch	Ctrl+Q	Window.QuickLaunch
Quick launch previous category	Ctrl+Shift+Q	Window.QuickLaunchPreviousCategory
Show dock menu	Alt+-	Window.ShowDockMenu
Show Ex MDI file list	Ctrl+Alt+Down Arrow	Window.ShowEzMDIFileList
Solution explorer search	Ctrl+;	Window.SolutionExplorerSearch
Window search	Alt+`	Window.WindowSearch"""


table = [line.split("\t") for line in table.split("\n")]
for line in table:
    if len(line) < 3:
        print(line)
table = [(key, description) for _, key, description in table]

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

mapping['pagedown'] = '{PGDN}'
mapping['pgdn'] = '{PGDN}'
mapping['pageup'] = '{PGUP}'
mapping['pgup'] = '{PGUP}'
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
mapping['left arrow'] = '{LEFT}'
mapping['numpadadd'] = '{PLUS}'
mapping['right'] = '{RIGHT}'
mapping['right arrow'] = '{RIGHT}'
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
mapping['num *'] = "{MULTIPLY}"
mapping['break'] = "{BREAK}"
mapping['insert'] = "{INSERT}"

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

key_to_function = {}
for key, desc in table:
    key_before = key
    key = process_key(key)
    item = {'action': key, 'description': desc, 'type': 'key'}

    if key not in key_to_function or desc != key_to_function[key]:
        print(json.dumps(item), end=',\n')

    if key not in key_to_function or desc == key_to_function[key]:
        key_to_function[key] = desc
    else:
        print('Duplicate key:', key, file=sys.stderr)
        print('Function:', key_to_function[key], file=sys.stderr)
        print('New Function:', desc, file=sys.stderr)