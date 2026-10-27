# CS_Editor

First new project done using VS 2022 and .net core 6.0.  Usually would've used 4.7 or 4.8 windows forms frameworks,
but forgot to install before starting project.

Program is a simple character editor for the game Colony Ship (as of Oct 2022, still in EA status).  Can edit attributes,
skills (exp and tagged or not), HPs and XP for created character and companions.  Editor has not been extensively tested,
so 'USE AT YOUR OWN RISK'.  There is help with the editor, but is sorely lacking in details at this time.  Will flesh it
out as time wears on.

Note, added functionality to allow one to edit certain inventory amounts when editing stats for the main, created PC.
Allow editing amounts of credits, med kits and ammo, if have any in inventory of the save game editing.  Again, editor
has not been extensively tested, so USE AT YOUR OWN RISK (and back up saves before using).

With version 0.9.13 of the game, the skills increased in size by 1.  Instead of 4 integers representing each
skill, there are now 5 integers.  Not sure what the other value represents for the skill, but had to update
editor for v0.9.13 to read the skills appropriately.

Feats can be edited too (add or remove, 'Feats' button), including the heroic feats normally only available at
character creation.  The feat list is not a fixed size, so changing it changes the size of the save file.  The
SaveFeats class parses the save file's Unreal (GVAS) properties to update every size value containing the
feat list.  Note, the first 18 bytes of each character's serialized data are scrambled (XOR with a fixed key),
the class unscrambles them to parse the data and scrambles them again when saving.